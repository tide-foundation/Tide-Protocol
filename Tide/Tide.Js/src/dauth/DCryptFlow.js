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
import CVKRandRegistrationReq from "./CVKRandRegistrationReq";
import TranToken from "../TranToken";
import { Dictionary } from "../Tools";

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

      const idGens = await this.clienSet.all(c => c.getClientGenerator());
      const ids = idGens.map(idGen => idGen.id);
      const idBuffers = await this.clienSet.map(ids, c => c.getClientBuffer());
      const guids = idBuffers.map(buff => new Guid(buff));
      
      const randoms = await this.clienSet.map(guids, cli => cli.random( guids.values)); 
 
      const cvkPub = randoms.values.map(rdm => rdm.cvkPub).reduce((sum, cvki) => cvki.add(sum));
      const shares = randoms.map((_, key) => randoms.map(rdm => rdm.shares[Number(key)]).values);
      const cvkAuths = idBuffers.map(buff => cmkAuth.derive(concat(buff, this.user.buffer)));

      const randReq = randoms.map((_, key) => new CVKRandRegistrationReq(cvkAuths.get(key), shares.get(key))) 

      const lis = ids.map(id => SecretShare.getLi(id, ids.values, bigInt(ed25519Point.order.toString())));
      const randSignUpResponses = await this.clienSet.map(randoms, (cli, _, key) => cli.randomSignUp(randReq.get(key), lis.get(key)));

      const partialPubs = randSignUpResponses.map(e => e[2]).map(p => randSignUpResponses.values.map(e => e[2]).reduce((sum, cvkPubi) => { return cvkPubi.isEqual(p) ? sum : cvkPubi.add(sum)} ,ed25519Point.infinity)); 
      const partialPub2s = randSignUpResponses.map(e => e[3]).map(p => randSignUpResponses.values.map(e => e[3]).reduce((sum, cvk2Pubi) => { return cvk2Pubi.isEqual(p) ? sum : cvk2Pubi.add(sum)} ,ed25519Point.infinity)); 
      const tokens = randSignUpResponses.map(e => e[1]).map((cipher, i) => TranToken.from(cvkAuths.get(i).decrypt(cipher))) // works
      const orkSigns = randSignUpResponses.map(e => e[0]);

      //await this.addDns(signatures, new ed25519Key(0, cvkPub), partialPubs, tokens);
      console.log(randSignUpResponses.map(e => e[0]));
      await Promise.all([this.addDns(orkSigns,  new ed25519Key(0, cvkPub), partialPubs,partialPub2s, tokens),
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
   * @param {ed25519Key} key 
   * @param {Dictionary<ed25519Point>} partialCvkPubs
   * @param {Dictionary<ed25519Point>} partialCvk2Pub
   * @param {import("../Tools").Dictionary<TranToken>} tokens*/
   addDns(signatures, key, partialCvkPubs, partialCvk2Pub, tokens) {
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

    this.signEntry(entry, tokens, partialCvkPubs,partialCvk2Pub);
    //return dnsCln.addDns(entry);
  }
/**
 * 
 * @param {DnsEntry} entry 
 * @param {import("../Tools").Dictionary<TranToken>} tokens
 * * @param {import("../Tools").Dictionary<ed25519Point>} partialPubs
 * * @param {import("../Tools").Dictionary<ed25519Point>} partialCvk2Pub
 */
  async signEntry(entry, tokens, partialPubs,partialCvk2Pub){
    const idGens = await this.clienSet.all(c => c.getClientGenerator())
    const ids = idGens.map(idGen => idGen.id);
    const lis = ids.map(id => SecretShare.getLi(id, ids.values, bigInt(ed25519Point.order.toString())));

    const tranid = new Guid();
    //var c2 = cmk2Pub.getX().toString();
    const signatures = await this.clienSet.map(lis, (cli, li, i) => cli.signEntry(tokens.get(i), tranid, entry, partialPubs.get(i),partialCvk2Pub.get(i), li));



    //const signature = signatures.reduce((sum, sig) => (sum + sig) % ed25519Point.order);
    //return signature;
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
