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

import bigInt from "big-integer";
import DAuthClient from "./DAuthClient";
import DAuthShare from "./DAuthShare";
import { SecretShare, Utils, AESKey, ed25519Key, ed25519Point } from "cryptide";
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
   * @returns {Promise<AESKey|Error>}
   */
  async signUp(password, email, threshold,  cmk=null, vendor) {
    try {
      if (!email) throw new Error("email must have at least one item");
      const emails = typeof email === "string" ? [email] : email;
      const emailIndex = Math.floor(Math.random() * emails.length);

      const r = random();
      const g = ed25519Point.fromString(password);
      const gR = g.times(r);

      const ids = await this.clienSet.all(cli => cli.getClientId()); 
      const idBuffers = await this.clienSet.map(ids, cli => cli.getClientBuffer());
      const guids = idBuffers.map(buff => new Guid(buff));

      const randoms = await this.clienSet.map(guids, cli => cli.random(gR, vendor, guids.values)); 

      const cmkPub = randoms.values.map(rdm => rdm.cmkPub).reduce((sum, cmki) => cmki.add(sum));
      const gRPrism = randoms.values.map(rdm => rdm.password).reduce((sum, gPrismi)=> gPrismi.add(sum));
      const vendorCMK = randoms.values.map(rdm => rdm.vendorCMK).reduce((sum, gPrismi)=> gPrismi.add(sum));
      const cvkAuth = AESKey.seed(vendorCMK.toArray());

      const rInv = r.modInv(bigInt(ed25519Point.order.toString()));
      const gPrism = gRPrism.times(rInv);
      const prismAuth = AESKey.seed(gPrism.toArray());

     // const prismAuth = AESKey.seed(g.times(prism).toArray());
      //const cmkAuth = AESKey.seed(Buffer.from(cmk.toArray(256).value));

      const prismAuths = idBuffers.map(buff => prismAuth.derive(buff)); 
      
      
      const mails = randoms.map((_, __, i) => emails[(emailIndex + i) % emails.length]);
      const shares = randoms.map((_, key) => randoms.map(rdm => rdm.shares[Number(key)]).values);
      const randReq = randoms.map((_, key) => new RandRegistrationReq(prismAuths.get(key), mails.get(key), shares.get(key))) 

      const signatures = await this.clienSet.map(randoms, (cli, _, key) => cli.randomSignUp(randReq.get(key)));

      // Make apply request to test if 1st ran num gen == threshold rand num
      const idGens = await this.clienSet.all(c => c.getClientGenerator())
      const idss = idGens.map(idGen => idGen.id);
      const lis = ids.map(id => SecretShare.getLi(id, idss.values, bigInt(ed25519Point.order.toString())));

      const pre_gPrism2 = this.clienSet.map(lis, (cli, li) => cli.ApplyPrism(ed25519Point.g, li));
      const gPrism2 = await pre_gPrism2.map(ki =>  ki[0])
        .reduce((sum, rki) => sum.add(rki), ed25519Point.infinity);

      var x = (gPrism2.getX().toString());
      var y = (gPrism2.getY().toString());
      return cvkAuth;
      /// ends HEREEEE

      await this.addDns(signatures, new ed25519Key(0, cmkPub));

      return cvkAuth;
    } catch (err) {
      return Promise.reject(err);
    }
  }

  /**
   * @private 
   * @typedef {import("./DAuthClient").OrkSign} OrkSign
   * @param {OrkSign[] | import("../Tools").Dictionary<OrkSign>} signatures 
   * @param {ed25519Key} key */
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

    entry.sign(key);
    return dnsCln.addDns(entry);
  }

  /**
   * @param {string} password 
   * @param {ed25519Point} point */
  async logIn(password, point) {
    try {
      const [prismAuth, token] = await this.getPrismAuth(password);

      const idGens = await this.clienSet.all(c => c.getClientGenerator())
      const prismAuths = idGens.map(idGen => prismAuth.derive(idGen.buffer));
      const tokens = idGens.map((_, i) => token.copy().sign(prismAuths.get(i), this.clienSet.get(i).userBuffer))

      const tranid = new Guid();
      const ids = idGens.map(idGen => idGen.id);
      const lis = ids.map(id => SecretShare.getLi(id, ids.values, bigInt(ed25519Point.order.toString())));
      const pre_ciphers = this.clienSet.map(lis, (cli, li, i) => cli.signIn(tranid, tokens.get(i), point, li));

      const cvkAuth = await pre_ciphers.map((cipher, i) => ed25519Point.from(prismAuths.get(i).decrypt(cipher)))
        .reduce((sum, cvkAuthi) => sum.add(cvkAuthi), ed25519Point.infinity);

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

      const n = bigInt(ed25519Point.order.toString());
      const g = ed25519Point.fromString(pass);
      const r = random();
      const gR = g.times(r);

      const ids = await pre_ids;
      const lis = ids.map((id) => SecretShare.getLi(id, ids.values, n));

      const pre_gRPrismis = this.clienSet.map(lis, (cli, li) => cli.ApplyPrism(gR, li));
      const rInv = r.modInv(n);

      const gRPrism = await pre_gRPrismis.map(ki =>  ki[0])
        .reduce((sum, rki) => sum.add(rki), ed25519Point.infinity);

      const gPrism = gRPrism.times(rInv);
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

    var cmk = SecretShare.interpolate(ids, cmks, bigInt(ed25519Point.order.toString()));
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
      var g = ed25519Point.fromString(pass);
      var prismAuth = AESKey.seed(g.times(prism).toArray());

      var idBuffers = await Promise.all(this.clients.map((c) => c.getClientBuffer()));
      var prismAuths = idBuffers.map((buff) => prismAuth.derive(buff));
      var keyAuths = idBuffers.map((buff) => keyAuth.derive(buff));

      var ids = await Promise.all(this.clients.map((c) => c.getClientId()));
      var [, prisms] = SecretShare.shareFromIds(prism, ids, threshold, bigInt(ed25519Point.order.toString()));

      var tokens = this.clients.map((c, i) => new TranToken().sign(keyAuths[i], concat(c.userBuffer, getArray(prisms[i]), prismAuths[i].toArray())));

      await Promise.all(this.clients.map((cli, i) => cli.changePass(prisms[i], prismAuths[i], tokens[i], isCmk)));
    } catch (err) {
      return Promise.reject(err);
    }
  }
}

function random() {
  return Utils.random(bigInt.one, bigInt((ed25519Point.order - BigInt(1)).toString()));
}
