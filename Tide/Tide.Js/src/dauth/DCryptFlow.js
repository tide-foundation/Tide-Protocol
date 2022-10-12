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

import DCryptClient from "./DCryptClient";
import { C25519Point, AESKey, C25519Key, C25519Cipher, BnInput, SecretShare, AesSherableKey, ed25519Key, ed25519Point } from "cryptide";
import KeyStore from "../keyStore";
import Cipher from "../Cipher";
import Guid from "../guid";
import { concat } from "../Helpers";
import DnsEntry from "../DnsEnrty";
import DnsClient from "./DnsClient";
import RuleClientSet from "./RuleClientSet";
import Rule from "../rule";
import Tags from "../tags";
import SetClient from "./SetClient";
import bigInt from "big-integer";

export default class DCryptFlow {
  /**
   * @param {string[]} urls
   * @param {Guid} user
   */
  constructor(urls, user, memory = false) {
    this.clients = urls.map((url) => new DCryptClient(url, user, memory));
    this.clienSet = new SetClient(this.clients);
    this.user = user;
    this.ruleCln = new RuleClientSet(urls, user);
  }

  /**
   * @param {AESKey} cmkAuth
   * @param {number} threshold
   * @param {Guid} signedKeyId
   * @param {Uint8Array[]} signatures
   */
  async signUp(cmkAuth, threshold, signedKeyId, signatures, cvk = ed25519Key.generate()) {
    try {
      if (!signatures && signatures.length != this.clienSet.length)
        throw new Error("Signatures are required");

      const ids = await this.clienSet.all(c => c.getClientId());
      const listCvk = cvk.share(threshold, ids.values, true);
      const cvks = ids.map((_, __, i) => listCvk[i]);

      const idBuffers = await this.clienSet.map(ids, c => c.getClientBuffer());
      const cvkAuths = idBuffers.map(buff => cmkAuth.derive(concat(buff, this.user.buffer)));

      const orkSigns = await this.clienSet.map(cvkAuths, (cli, _, key) => 
        cli.register(cvk.public(), cvks.get(key).x, cvkAuths.get(key), signedKeyId, signatures[key]));

      await Promise.all([this.addDns(orkSigns, cvk),
        this.ruleCln.setOrUpdate(Rule.allow(this.user, Tags.vendor, signedKeyId), orkSigns.keys)]);

      return cvk;
    } catch (err) {
      throw err;
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

  /** @param {AESKey} cmkAuth */
  async getKey(cmkAuth, noPublic = false) {
    const idGens = await this.clienSet.all(c => c.getClientGenerator());
    
    const tranid = new Guid();
    const ids = idGens.map(idGen => idGen.id);
    const lis = ids.map(id => SecretShare.getLi(id, ids.values, bigInt(ed25519Point.order.toString())));
    const cvkAuths = idGens.map(idGen => concat(idGen.buffer, this.user.buffer)).map(buff => cmkAuth.derive(buff));
    const cipherCvks = this.clienSet.map(lis,(c, li, i) => c.getCvk(tranid, cvkAuths.get(i), li));

    const cvk = await cipherCvks.map((cvki, i) => BnInput.getBig(cvkAuths.get(i).decrypt(cvki)))
      .reduce((sum, cvki) => sum.add(cvki).mod(bigInt(ed25519Point.order.toString())), BnInput.getBig(0));

    return ed25519Key.private(cvk, noPublic);
  }

  /**
   * @param {Uint8Array[]} ciphers
   * @param {ed25519Key} prv
   * @returns {Promise<Uint8Array[]>}
   */
  async decryptBulk(ciphers, prv) {
    try {
      const keyId = new KeyStore(prv.public()).keyId;
      const challenges = await Promise.all(this.clients.map((cli) => cli.challenge(keyId)));

      const asymmetrics = ciphers.map(cph => Cipher.asymmetric(cph));
      const sessionKeys = challenges.map((ch) => prv.decryptKey(ch.challenge));
      const signs = sessionKeys.map(key => key.hash(concat(...asymmetrics)));

      const cipherPartials = await Promise.all(this.clients.map((cli, i) => cli.decryptBulk(asymmetrics, keyId, challenges[i].token, signs[i])));

      const ciphs = asymmetrics.map(asy => Cipher.cipherFromAsymmetric(asy));
      const ids = await Promise.all(this.clients.map(c => c.getClientId()));

      /** @type {Uint8Array[]} */
      const plains = new Array(ciphers.length);
      for (let j = 0; j < ciphers.length; j++) {
        const partials = cipherPartials.map((cph, i) => C25519Point.from(sessionKeys[i].decrypt(cph[j]))) /////////////// potential catastrophic bug
          .map(pnt => new C25519Cipher(pnt, ciphs[j].c2));
        const plain = C25519Cipher.decryptShares(partials, ids);
  
        const symmetric = Cipher.symmetric(ciphers[j]);
        if (symmetric.length == 0) {
          plains[j] = C25519Cipher.unpad(plain);
          continue;
        }

        const symmetricKey = AesSherableKey.from(plain);
        plains[j] = symmetricKey.decrypt(symmetric);
      }

      return plains;
    } catch (err) {
      return Promise.reject(err);
    }
  }

  /**
   * @param {Uint8Array} cipher
   * @param {ed25519Key} prv
   * @returns {Promise<Uint8Array>}
   */
  async decrypt(cipher, prv) {
    return (await this.decryptBulk([cipher], prv))[0];
  }

  async confirm() {
    await this.clienSet.all(c => c.confirm());
  }
}
