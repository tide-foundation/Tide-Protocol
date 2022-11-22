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
import GenShardShareResponse from "./GenShardShareResponse";

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

  async GenShardCVK(singi,gVoucher){
    try{
      const n = bigInt(ed25519Point.order.toString());
      const mIdORKs = await this.clienSet.all(c => c.getClientUsername());
      
      
      const genShardResp = await this.clienSet.all(dAuthClient => dAuthClient.genShard(mIdORKs,  2, singi , gVoucher));

      const gCVK = genShardResp.map(a =>  a[0]).reduce((sum, point) => sum.add(point), ed25519Point.infinity);
   
      const timestamp = median(genShardResp.values.map(resp => resp[2])); 
      
      const mergeShare=(share) =>{
        return share.map(p => GenShardShareResponse.from(p));
      }
      const shareEncrypted = genShardResp.values.map(a =>  a[1]).map(s => mergeShare(s));
      const sortedShareArray = sorting(shareEncrypted);

      return {timestampCVK : timestamp, ciphersCVK : sortedShareArray}

    }catch(err){
      Promise.reject(err);
    }
    
  }
  async SetCVK(ciphers, timestamp, gCMKAuth){
    try{
      const mIdORKs = await this.clienSet.all(c => c.getClientUsername());

      const pre_setCVKResponse = this.clienSet.all((DAuthClient, i) => DAuthClient.setCVK(filtering(ciphers.filter(element => element.orkId === mIdORKs.get(i))), timestamp, mIdORKs));

      const idGens = await this.clienSet.all(c => c.getClientGenerator()); // implement method to only use first 14 orks that reply
      const ids = idGens.map(idGen => idGen.id);
      const lis = ids.map(id => SecretShare.getLi(id, ids.values, bigInt(ed25519Point.order.toString()))); 
      
      const AA = lis.map(li => li.toString());

      const setCVKResponse = await pre_setCVKResponse;

      const gCVKtest = setCVKResponse.values.reduce((sum, next, i) => sum.add(next[0]).times(lis.get(i)), ed25519Point.infinity); // Does Sum ( gCVKtesti ) * li . Li here works because of ordered indexes
      const gCVK2test = setCVKResponse.values.reduce((sum, next, i) => sum.add(next[1]).times(lis.get(i)), ed25519Point.infinity);
      const gCVKR2 = setCVKResponse.values.reduce((sum, next, i) => sum.add(next[2]), ed25519Point.infinity); // Does Sum (gCMKR2)
      const encryptedStatei = setCVKResponse.values.map(resp => resp[3]);

      return {gTests : [gCVKtest, gCVK2test], gCVKR2 : gCVKR2, state : encryptedStatei};
    }catch(err){
      Promise.reject(err);
    }
    
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
 
      const cvkPub = randoms.values.map(rdm => rdm.cvkPub).reduce((sum, cvki) => cvki.add(sum),ed25519Point.infinity);
      const cvk2Pub = randoms.values.map(rdm => rdm.cvk2Pub).reduce((sum, cvk2i) => cvk2i.add(sum),ed25519Point.infinity);
      const cvkis = randoms.map(rdm => rdm.cvki_noThreshold);
      const cvk2is = randoms.map(rdm => rdm.cvk2i_noThreshold);
      const orkIDs =randoms.map(rdm => rdm.ork_UserName);
    
      const partialPubs = randoms.map(p => randoms.values.map(p => p.cvkPub).reduce((sum, cvkPubi) => { return cvkPubi.isEqual(p.cvkPub) ? sum : cvkPubi.add(sum)}, ed25519Point.infinity));
      const partialPub2s = randoms.map(p => randoms.values.map(p => p.cvk2Pub).reduce((sum, cvkPubi) => { return cvkPubi.isEqual(p.cvk2Pub) ? sum : cvkPubi.add(sum)}, ed25519Point.infinity));

      const shares = randoms.map((_, key) => randoms.map(rdm => rdm.shares[Number(key)]).values);
      const cvkAuths = idBuffers.map(buff => cmkAuth.derive(concat(buff, this.user.buffer)));

      const entry = this.prepareDnsEntry(cvkPub, orkIDs);

      const randReq = randoms.map((_, key) => new CVKRandRegistrationReq(cvkAuths.get(key), shares.get(key),entry,cvkis.get(key),cvk2is.get(key)));
     // const signatures = await this.clienSet.map(lis, (cli, li, i) => cli.signEntry(tokens.get(i), tranid, entry, partialPubs.get(i),partialCvk2Pub.get(i), li));

      const lis = ids.map(id => SecretShare.getLi(id, ids.values, bigInt(ed25519Point.order.toString())));
      const randSignUpResponses = await this.clienSet.map(lis, (cli, _, key,i) => cli.randomSignUp(randReq.get(key), partialPubs.get(key),partialPub2s.get(key),lis.get(key)));

      // const partialPubs = randSignUpResponses.map(e => e[2]).map(p => randSignUpResponses.values.map(e => e[2]).reduce((sum, cvkPubi) => { return cvkPubi.isEqual(p) ? sum : cvkPubi.add(sum)} ,ed25519Point.infinity)); 
      // const partialPub2s = randSignUpResponses.map(e => e[3]).map(p => randSignUpResponses.values.map(e => e[3]).reduce((sum, cvk2Pubi) => { return cvk2Pubi.isEqual(p) ? sum : cvk2Pubi.add(sum)} ,ed25519Point.infinity)); 
      const tokens = randSignUpResponses.map(e => e[1]).map((cipher, i) => TranToken.from(cvkAuths.get(i).decrypt(cipher))); // works
      const orkSigns = randSignUpResponses.map(e => e[0]);
      const s = randSignUpResponses.values.map(e => e[4]).reduce((sum, sig) => (sum + sig) % ed25519Point.order); // todo: add proper mod function here without it being messy

      await Promise.all([this.addDns(orkSigns, cvk2Pub, entry,s),
        this.ruleCln.setOrUpdate(Rule.allow(this.user, Tags.vendor, signedKeyId), orkSigns.keys)]);

      return cvk;
    } catch (err) {
      throw err;
    }
  }

  /**
   * 
   * @param {ed25519Point} cvkPub 
   * @param {Dictionary<string>} orkIds 
   * @returns {DnsEntry}
   */
   prepareDnsEntry(cvkPub, orkIds){
    const cln = this.clienSet.get(0); // chnage this later
    const dnsCln = new DnsClient(cln.baseUrl, cln.userGuid); // you have to choose the same ork as the cln later

    const entry = new DnsEntry();
    entry.id = cln.userGuid;
    entry.Public = new ed25519Key(0, cvkPub);
    entry.orks = orkIds.values;

    return entry;
  }


  /**
   * @private 
   * @typedef {import("./DAuthClient").OrkSign} OrkSign
   * @param {OrkSign[] | import("../Tools").Dictionary<OrkSign>} signatures 
   * @param {ed25519Point} cvk2Pub
   * @param {DnsEntry} entry
   * @param {bigint} s*/
   async addDns(signatures,cvk2Pub, entry,s) {
    const keys = Array.isArray(signatures) ? Array.from(signatures.keys()).map(String) : signatures.keys;
    const index = keys[Math.floor(Math.random() * keys.length)];
    const cln = this.clienSet.get(index);
    const dnsCln = new DnsClient(cln.baseUrl, cln.userGuid);

    if (Array.isArray(signatures)) {
      entry.signatures = signatures.map(sig => sig.sign);
      entry.orks = signatures.map(sig => sig.orkid);
    }
    else {
      entry.signatures = signatures.values.map(val => val.sign);
      entry.orks = signatures.values.map(val => val.orkid);
    }
    const signature = ed25519Key.createSig(cvk2Pub, s); 

    entry.signature = Buffer.from(signature).toString('base64');
    return dnsCln.addDns(entry);
   
  }
/**
 * 
 * @param {DnsEntry} entry 
 * @param {import("../Tools").Dictionary<TranToken>} tokens
 * @param {import("../Tools").Dictionary<ed25519Point>} partialPubs
 * @param {import("../Tools").Dictionary<ed25519Point>} partialCvk2Pub
 * @param {ed25519Point} cvk2Pub
 */
  async signEntry(entry, tokens, partialPubs,partialCvk2Pub,cvk2Pub){
    const idGens = await this.clienSet.all(c => c.getClientGenerator())
    const ids = idGens.map(idGen => idGen.id);
    const lis = ids.map(id => SecretShare.getLi(id, ids.values, bigInt(ed25519Point.order.toString())));

    const tranid = new Guid();
    const signatures = await this.clienSet.map(lis, (cli, li, i) => cli.signEntry(tokens.get(i), tranid, entry, partialPubs.get(i),partialCvk2Pub.get(i), li));

    const s = signatures.values.reduce((sum, sig) => (sum + sig) % ed25519Point.order); // todo: add proper mod function here without it being messy

    const signature = ed25519Key.createSig(cvk2Pub, s);
    return signature;
 
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
function median(numbers) {
  const sorted = Array.from(numbers).sort((a, b) => a - b);
  const middle = Math.floor(sorted.length / 2);

  if (sorted.length % 2 === 0) {
      return (sorted[middle - 1]+(sorted[middle])/(2));
  }

  return sorted[middle];
}


//The array  is a combined list from all the orks returns
function sorting(shareEncrypted){
  const shareArray = shareEncrypted.flat() ; 
  let sortedShareArray = shareArray.sort((a, b) => a.to.localeCompare(b.to) || a.from.localeCompare(b.from) ); //Sorting shareEncrypted based on 'to' and then 'from'
  let newarray =[];
  for(let i=0 ; i < sortedShareArray.length ; i++){
    let e={
      "orkId" : sortedShareArray[i].to,
      "data" : JSON.stringify({To: sortedShareArray[i].to, From: sortedShareArray[i].from, EncryptedData: sortedShareArray[i].encryptedData})  
    }
    newarray.push(e);
  }
  return newarray;
}

function filtering(array){
  let results = array.map(a => a.data);
  return results;
}
