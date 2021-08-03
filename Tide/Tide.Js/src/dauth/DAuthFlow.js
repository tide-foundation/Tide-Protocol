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

export default class DAuthFlow {
  /**
   * @param {string[]} urls
   * @param {string|Guid} user
   */
  constructor(urls, user, memory = false) {
    this.clients = urls.map((url) => new DAuthClient(url, user, memory));
  }

  /**
   * @param {string} password
   * @param {string} email
   * @param {number} threshold
   * @returns {Promise<AESKey>}
   */
  async signUp(password, email, threshold, cmk = random()) {
    try {
      var prism = random();
      var g = C25519Point.fromString(password);

      var prismAuth = AESKey.seed(g.times(prism).toArray());
      var cmkAuth = AESKey.seed(Buffer.from(cmk.toArray(256).value));

      var ids = await Promise.all(this.clients.map((c) => c.getClientId()));
      var idBuffers = await Promise.all(this.clients.map((c) => c.getClientBuffer()));
      var prismAuths = idBuffers.map((buff) => prismAuth.derive(buff));
      var cmkAuths = idBuffers.map((buff) => cmkAuth.derive(buff));
      var [, cmks] = SecretShare.shareFromIds(cmk, ids, threshold, C25519Point.n);
      var [, prisms] = SecretShare.shareFromIds(prism, ids, threshold, C25519Point.n);

      var signatures = await Promise.all(this.clients.map((cli, i) => cli.signUp(prisms[i], cmks[i], prismAuths[i], cmkAuths[i], email)));
      await this.addDns(signatures, C25519Key.private(cmk));

      return cmkAuth;
    } catch (err) {
      return Promise.reject(err);
    }
  }

  /** @private 
   * @param {{orkid: string, sign: string}[]} signatures 
   * @param {C25519Key} key */
  addDns(signatures, key) {
    const cln = this.clients[Math.floor(Math.random() * this.clients.length)];
    const dnsCln = new DnsClient(cln.baseUrl, cln.userGuid);
    var entry = new DnsEntry();
    
    entry.id = cln.userGuid;
    entry.public = key.public()
    entry.signatures = signatures.map(sig => sig.sign);
    entry.orks = signatures.map(sig => sig.orkid);
    entry.sign(key);

    return dnsCln.addDns(entry);
  }

  /**
   * @param {string} password 
   * @param {C25519Point} point */
  async logIn(password, point) {
    try {
      var [prismAuth, token] = await this.getPrismAuth(password);
      var idBuffers = await Promise.all(this.clients.map((c) => c.getClientBuffer()));
      var prismAuths = idBuffers.map((buff) => prismAuth.derive(buff));

      const tranid = new Guid();
      var tokens = this.clients.map((c, i) => token.copy().sign(prismAuths[i], c.userBuffer));
      var ciphers = await Promise.all(this.clients.map((cli, i) => cli.signIn(tranid, tokens[i], point)));

      var cvkAuths = prismAuths.map((auth, i) => auth.decrypt(ciphers[i])).map(shr => C25519Point.from(shr));

      var ids = await Promise.all(this.clients.map((c) => c.getClientId()));

      /** @type {C25519Point} */
      var cvkAuth = ids.map(id => SecretShare.getLi(id, ids, C25519Point.n))
          .map((li, i) => cvkAuths[i].times(li))
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
      var r = random();
      var n = C25519Point.n;
      var g = C25519Point.fromString(pass);
      var gR = g.multiply(r);

      var ids = await Promise.all(this.clients.map((c) => c.getClientId()));
      var lis = ids.map((id) => SecretShare.getLi(id, ids, n));

      var gRPrismis = await Promise.all(this.clients.map((cli) => cli.ApplyPrism(gR)));
      var gRPrism = gRPrismis.map(([ki], i) => ki.times(lis[i])).reduce((rki, sum) => rki.add(sum));
      var gPrism = gRPrism.times(r.modInv(n));

      return [AESKey.seed(gPrism.toArray()), gRPrismis[0][1]];
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

  confirm() {
    return Promise.all(this.clients.map((c) => c.confirm()));
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
