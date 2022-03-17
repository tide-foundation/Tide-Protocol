// Tide Protocol - Infrastructure for the Personal Data economy
// Copyright (C) 2019 Tide Foundation Ltd
//
// This program is free software and is subject to the terms of
// the Tide Community Open Source License as published by the
// Tide Foundation Limited. You may modify it and redistribute
// it in accordance with and subject to the terms of that License.
// This program is distributed WITHOUT WARRANTY of any kind,
// including without any implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE.
// See the Tide Community Open Source License for more details.
// You should have received a copy of the Tide Community Open
// Source License along with this program.
// If not, see https://tide.org/licenses_tcosl-1-0-en
// @ts-check

import BigInt from "big-integer";
import DAuthClient from "./DAuthClient";
import DAuthShare from "./DAuthShare";
import { SecretShare, Utils, C25519Point, AESKey, C25519Key } from "cryptide";
import TranToken from "../TranToken";
import { concat } from "../Helpers";
import { getArray } from "cryptide/src/bnInput";
import DnsEntry from "../DnsEnrty";
import DnsClient from "./DnsClient";
import Guid from "../guid";
import SetClient from "./SetClient";
import RandRegistrationReq from "./RandRegistrationReq";

export default class DAuthFlow {
  /**
   * @param {string[]} urls
   * @param {string|Guid} user
   */
  constructor(urls, user, memory = false) {
    this.clients = urls.map((url) => new DAuthClient(url, user, memory));
    this.clienSet = new SetClient(this.clients);
  }

  /**
   * @param {string} password
   * @param {string|string[]} email
   * @param {number} threshold
   * @param {BigInt.BigInteger} cmk
   * @param {C25519Point} vendor
   * @returns {Promise<AESKey|Error>}
   */
  async signUp(password, email, threshold, cmk=null, vendor=null) {
    try {
      if (!email) throw new Error("email must have at least one item");
      const emails = typeof email === "string" ? [email] : email;
      const emailIndex = Math.floor(Math.random() * emails.length);

      const g = C25519Point.fromString(password);

      const ids = await this.clienSet.all(cli => cli.getClientId());
      const idBuffers = await this.clienSet.map(ids, cli => cli.getClientBuffer());
      const guids = idBuffers.map(buff => new Guid(buff));

      const randoms = await this.clienSet.map(guids, cli => cli.random(g, guids.values));

      const cmkPub = randoms.values.map(rdm => rdm.cmkPub).reduce((sum, cmki, i) => cmki.add(sum));
      const gPrism = randoms.values.map(rdm => rdm.password).reduce((sum, gPrismi, i)=> gPrismi.add(sum));

      const prismAuth = AESKey.seed(gPrism.toArray());

      const prismAuths = idBuffers.map(buff => prismAuth.derive(buff));
      console.log('prism1:', prismAuths.map(itm => itm.toString()));

      const mails = randoms.map((_, __, i) => emails[(emailIndex + i) % emails.length]);
      const shares = randoms.map((_, key) => randoms.map(rdm => rdm.shares[Number(key)]).values);
      const randReq = randoms.map((_, key) => new RandRegistrationReq(prismAuths.get(key), mails.get(key), shares.get(key))) 

      const signatures = await this.clienSet.map(randoms, (cli, _, key) => cli.randomSignUp(randReq.get(key)));

      await this.addDns(signatures, new C25519Key(0, cmkPub));

      return prismAuth;
    } catch (err) {
      return Promise.reject(err);
    }
  }

  /**
   * @private 
   * @typedef {import("./DAuthClient").OrkSign} OrkSign
   * @param {OrkSign[] | import("../Tools").Dictionary<OrkSign>} signatures 
   * @param {C25519Key} key */
  addDns(signatures, key) {
    const keys = Array.isArray(signatures) ? Array.from(signatures.keys()).map(String) : signatures.keys;
    const index = keys[Math.floor(Math.random() * keys.length)];
    const cln = this.clienSet.get(index);
    const dnsCln = new DnsClient(cln.baseUrl, cln.userGuid);

    const entry = new DnsEntry();
    entry.id = cln.userGuid;
    entry.public = key.public()

    if (Array.isArray(signatures)) {
      entry.signatures = signatures.map(sig => sig.sign);
      entry.orks = signatures.map(sig => sig.orkid);
    }
    else {
      entry.signatures = signatures.values.map(val => val.sign);
      entry.orks = signatures.values.map(val => val.orkid);
    }

    //entry.sign(key);
    return dnsCln.addDns(entry);
  }

  /**
   * @param {string} password 
   * @param {C25519Point} point */
  async logIn(password, point) {
    try {
      const [prismAuth, token] = await this.getPrismAuth(password);

      const idGens = await this.clienSet.all(c => c.getClientGenerator())
      const prismAuths = idGens.map(idGen => prismAuth.derive(idGen.buffer));
      const tokens = idGens.map((_, i) => token.copy().sign(prismAuths.get(i), this.clienSet.get(i).userBuffer))
      console.log('prism2:', prismAuths.map(itm => itm.toString()));


      const tranid = new Guid();
      const ids = idGens.map(idGen => idGen.id);
      const lis = ids.map(id => SecretShare.getLi(id, ids.values, C25519Point.n));
      const pre_ciphers = this.clienSet.map(lis, (cli, li, i) => cli.signIn(tranid, tokens.get(i), point, li));

      const cvkAuth = await pre_ciphers.map((cipher, i) => C25519Point.from(prismAuths.get(i).decrypt(cipher)))
        .reduce((sum, cvkAuthi) => sum.add(cvkAuthi), C25519Point.infinity);

      return AESKey.seed(cvkAuth.toArray());
    } catch (err) {
      return Promise.reject(err);
    }
  }

  /** @param {string} pass
   * @returns {Promise<[AESKey, TranToken]>} */
  async getPrismAuth(pass) {
    try {
      const pre_ids = this.clienSet.all(c => c.getClientId());

      const r = random();
      const n = C25519Point.n;
      const g = C25519Point.fromString(pass);
      const gR = g.multiply(r);

      const ids = await pre_ids;
      const lis = ids.map((id) => SecretShare.getLi(id, ids.values, n));

      const pre_gRPrismis = this.clienSet.map(lis, (cli, li) => cli.ApplyPrism(gR, li));
      const rInv = r.modInv(n);

      const gRPrism = await pre_gRPrismis.map(ki =>  ki[0])
        .reduce((sum, rki) => sum.add(rki), C25519Point.infinity);

      const gPrism = gRPrism.times(rInv);
      const prismAuth = AESKey.seed(gPrism.toArray());

      const [,token] = await pre_gRPrismis.values[0];

      return [AESKey.seed(gPrism.toArray()), token];
    } catch (err) {
      return Promise.reject(err);
    }
  }

  Recover() {
    return Promise.all(this.clients.map((cli) => cli.Recover()));
  }

  /**
   * @param {string} textShares
   * @param {string} newPass
   * @param {number} threshold
   */
  async Reconstruct(textShares, newPass = null, threshold = null) {
    var shares = textShares
      .replace(/( +?)|\[|\]/g, "")
      .split(/\r?\n/)
      .map((key) => DAuthShare.from(key));

    var ids = shares.map((c) => c.id);
    var cmks = shares.map((c) => c.share);

    var cmk = SecretShare.interpolate(ids, cmks, C25519Point.n);
    var cmkAuth = AESKey.seed(Buffer.from(cmk.toArray(256).value));

    if (newPass !== null && threshold !== null) {
      await this.changePassWithKey(cmkAuth, newPass, threshold);
    }

    return cmkAuth;
  }

  /**
   * @param {string} pass
   * @param {string} newPass
   * @param {number} threshold
   */
  async changePass(pass, newPass, threshold) {
    try {
      var [prismAuth] = await this.getPrismAuth(pass);
      await this._changePass(prismAuth, newPass, threshold);
    } catch (err) {
      return Promise.reject(err);
    }
  }

  /**
   * @param {AESKey} cmkAuth
   * @param {string} pass
   * @param {number} threshold
   */
  changePassWithKey(cmkAuth, pass, threshold) {
    return this._changePass(cmkAuth, pass, threshold, true);
  }

  async confirm() {
    await this.clienSet.all(c => c.confirm());
  }

  /**
   * @param {AESKey} keyAuth
   * @param {string} pass
   * @param {number} threshold
   */
  async _changePass(keyAuth, pass, threshold, isCmk = false) {
    try {
      var prism = random();
      var g = C25519Point.fromString(pass);
      var prismAuth = AESKey.seed(g.times(prism).toArray());

      var idBuffers = await Promise.all(this.clients.map((c) => c.getClientBuffer()));
      var prismAuths = idBuffers.map((buff) => prismAuth.derive(buff));
      var keyAuths = idBuffers.map((buff) => keyAuth.derive(buff));

      var ids = await Promise.all(this.clients.map((c) => c.getClientId()));
      var [, prisms] = SecretShare.shareFromIds(prism, ids, threshold, C25519Point.n);

      var tokens = this.clients.map((c, i) => new TranToken().sign(keyAuths[i], concat(c.userBuffer, getArray(prisms[i]), prismAuths[i].toArray())));

      await Promise.all(this.clients.map((cli, i) => cli.changePass(prisms[i], prismAuths[i], tokens[i], isCmk)));
    } catch (err) {
      return Promise.reject(err);
    }
  }
}

function random() {
  return Utils.random(BigInt.one, C25519Point.n.subtract(BigInt.one));
}
