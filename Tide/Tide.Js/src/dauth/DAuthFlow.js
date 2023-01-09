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

import bigInt, { fromArray } from "big-integer";
import DAuthClient from "./DAuthClient";
import DAuthShare from "./DAuthShare";
import { SecretShare, Utils, AESKey, ed25519Key, ed25519Point, Hash } from "cryptide";
import TranToken from "../TranToken";
import { concat } from "../Helpers";
import { getArray } from "cryptide/src/bnInput";
import DnsEntry from "../DnsEnrty";
import DnsClient from "./DnsClient";
import Guid from "../guid";
import SetClient from "./SetClient";
import { Dictionary } from "../Tools";
import ApplyResponseDecrypted from "./ApplyResponseDecrypted";
import IdGenerator from "../IdGenerator";
import GenShardShareResponse from "./GenShardShareResponse";


export default class DAuthFlow {
  /**
   * @param {string[]} urls
   * @param {string|Guid} user
   */
  constructor(urls, user, memory = false) {
    this.clients = urls.map((url) => new DAuthClient(url, user, memory));
    this.clienSet = new SetClient(this.clients);
    this.userID = typeof user === 'string' ? IdGenerator.seed(user) : new IdGenerator(user); // Needed this out of neccessity
  }

  async GenShardCMK(pass, gVVK){
    try{
      const n = bigInt(ed25519Point.order.toString());
      const mIdORKs = await this.clienSet.all(c => c.getClientUsername());

      const r1 = random();
      const r2 = random();
  
      const gPass = ed25519Point.fromString(pass);
      const gUser = ed25519Point.fromString(this.userID.guid.toString() + gVVK.toArray().toString()) // replace this with proper hmac + point to hash function
      const gBlurUser = gUser.times(r1);
      const gBlurPass = gPass.times(r2);
      const r1Inv = r1.modInv(n);
      const r2Inv = r2.modInv(n);
      const gMul1 = Buffer.from(gBlurUser.toArray()).toString('base64');
      const gMul2 = Buffer.from(gBlurPass.toArray()).toString('base64');
      const multipliers = [gMul1, gMul2];

      const genShardResp = await this.clienSet.all(dAuthClient => dAuthClient.genShard(mIdORKs,  3, multipliers));

      const gCMK = genShardResp.values.map(a =>  a[0]).reduce((sum, point) => sum.add(point), ed25519Point.infinity);

      /**
       * @param {ed25519Point[]} share1 
       * @param {ed25519Point[]} share2 
       */
      const addShare = (share1, share2) => {
        return share1.map((s, i) => s.add(share2[i]))
      }
      const gMultiplied = genShardResp.values.map(p => p[2]).reduce((sum, next) => addShare(sum, next)); // adds all of the respective gMultipliers together

      const gUserCMK = gMultiplied[0].times(r1Inv);
      const gPassPrism = gMultiplied[1].times(r2Inv);

      const hash_gUserCMK = Hash.sha512Buffer(gUserCMK.toArray());
      const CMKmul = bigInt_fromBuffer(hash_gUserCMK.subarray(0, 32)); // first 32 bytes

      const VUID = IdGenerator.seed(hash_gUserCMK.subarray(32, 64)); /// last 32 bytes 
      const gCMKAuth = gCMK.times(CMKmul); 
      const gPRISMAuth = ed25519Point.g.times(bigInt_fromBuffer(Hash.shaBuffer(gPassPrism.toArray()))); 
      const timestamp = median(genShardResp.values.map(resp => resp[3])); 
      
      const mergeShare=(share) =>{
        return share.map(p => GenShardShareResponse.from(p));
      }
      const shareEncrypted = genShardResp.values.map(a =>  a[1]).map(s => mergeShare(s));
      const sortedShareArray = sorting(shareEncrypted);

      return {vuid : VUID, gCMKAuth : gCMKAuth, gPRISMAuth : gPRISMAuth, timestampCMK : timestamp, ciphersCMK : sortedShareArray, gCMK : gCMK}

    }catch(err){
      Promise.reject(err);
    }
  }

  async SetCMK(ciphers, timestamp){
    try{
      const mIdORKs = await this.clienSet.all(c => c.getClientUsername());

      const pre_setCMKResponse = this.clienSet.all((DAuthClient, i) => DAuthClient.setCMK(filtering(ciphers.filter(element => element.orkId === mIdORKs.get(i))), timestamp, mIdORKs));

      const idGens = await this.clienSet.all(c => c.getClientGenerator()); // implement method to only use first 14 orks that reply
      const ids = idGens.map(idGen => idGen.id);
      const lis = ids.map(id => SecretShare.getLi(id, ids.values, bigInt(ed25519Point.order.toString()))); 
    
      const setCMKResponse = await pre_setCMKResponse;

      const gCMKtest = setCMKResponse.values.map(resp => resp[0]).reduce((sum, next, i) => sum.add(next[0].times(lis.get(i))), ed25519Point.infinity);
      const gPRISMtest = setCMKResponse.values.map(resp => resp[0]).reduce((sum, next, i) => sum.add(next[1].times(lis.get(i))), ed25519Point.infinity);
      const gCMK2test = setCMKResponse.values.map(resp => resp[0]).reduce((sum, next, i) => sum.add(next[2].times(lis.get(i))), ed25519Point.infinity);
      const gCMKR2 = setCMKResponse.values.reduce((sum, next, i) => sum.add(next[1]), ed25519Point.infinity); // Does Sum (gCMKR2)

      const encryptedStatei = setCMKResponse.values.map(resp => resp[2]);
      const randomKey = setCMKResponse.values.map(r => r[3]);

      return {gTests : [gCMKtest, gPRISMtest, gCMK2test], gCMKR2 : gCMKR2, state : encryptedStatei, randomKey : randomKey};
    }catch(err){
      Promise.reject(err);
    }
    
  }

  async PreCommit (gTests, gCMKR2, state, randomKey, timestamp, gPrismAuth, email, gCMK){
    try{
      const mIdORKs = await this.clienSet.all(c => c.getClientUsername());
      const pre_commitCMKResponse = await this.clienSet.all((DAuthClient,i) => DAuthClient.preCommit(gTests, gCMKR2, state[i], randomKey[i], gPrismAuth, email, mIdORKs));
      
      const CMKS = pre_commitCMKResponse.values.reduce((sum, s) => (sum + s) % ed25519Point.order); 

      const CMKM = Hash.shaBuffer(Buffer.concat([Buffer.from(gTests[0].toArray()),Buffer.from(timestamp.toString()),Buffer.from(this.userID.guid.toString())])); // TODO: Add point.to_base64 function
      const pubs = await this.clienSet.all(c => c.getPublic()); //works   
      const CMKR = pubs.map(pub => pub.y).reduce((sum, p) => sum.add(p), ed25519Point.infinity).add(gCMKR2);
      const CMKH = Hash.sha512Buffer( Buffer.concat([Buffer.from(CMKR.toArray()),Buffer.from(gTests[0].toArray()), CMKM]));
      
      const CMKH_int = bigInt_fromBuffer(CMKH);
      
      if(!ed25519Point.g.times(CMKS).isEqual(CMKR.add(gTests[0].times(CMKH_int)))) {
        return Promise.reject("Ork Signature Invalid")
      }
      
      const commitCMKResponse = await this.clienSet.all((DAuthClient,i) => DAuthClient.commit(CMKS, state[i], gCMKR2, mIdORKs));
      
      // @ts-ignore
      const entry = await this.addDnsEntry(CMKS.toString(), gCMKR2, timestamp, gCMK, mIdORKs)
  
    }catch(err){
      Promise.reject(err);
    }
   
  }


 /**
   * 
   * @param {ed25519Point} gR 
   * @param {import("../Tools").Dictionary<string>} mIdORKs 
   * @param {string} s
   * @param {string} timestamp
   * @param {ed25519Point} pub
   * //@returns {DnsEntry}
   */
 async addDnsEntry(s, gR, timestamp, pub, mIdORKs){
  const cln = this.clienSet.get(0); // chnage this later

  const entry = new DnsEntry();
  entry.id = cln.userGuid;
  entry.Public = new ed25519Key(0, pub);
  entry.s = s;
  entry.gR = gR;
  entry.timestamp = timestamp; 
  entry.vIdORK = mIdORKs.values.map(id => id.toString());
  const dnsCln = new DnsClient(cln.baseUrl, cln.userGuid);

  return  dnsCln.addDns(entry);
}

 

  /**
   * @param {string} password 
   * @param {ed25519Point} point */
  async logIn(password, point) {
    try {
      const [prismAuth, token] = await this.getPrismAuth(password);

      const idGens = await this.clienSet.all(c => c.getClientGenerator())
      const prismAuths = idGens.map(idGen => prismAuth.derive(idGen.buffer));
      // decrypt(timestampi, certTimei) with PristAuthi
      // Add userId timestampi ,certTimei , prismAuthi to verifyi /tokens
      const tokens = idGens.map((_, i) => token.copy().sign(prismAuths.get(i), this.clienSet.get(i).userBuffer))

      //Calculate the deltaTime median(timestami[])-epochtimeUTC() ;( epochtimeUTC() = timestampi ?)

      const tranid = new Guid();
      const ids = idGens.map(idGen => idGen.id);
      const lis = ids.map(id => SecretShare.getLi(id, ids.values, bigInt(ed25519Point.order.toString())));
      // Pass userId , timestampi ,certTimei, verifyi)
      const pre_ciphers = this.clienSet.map(lis, (cli, li, i) => cli.signIn(tranid, tokens.get(i), point, li));

      const cvkAuth = await pre_ciphers.map((cipher, i) => ed25519Point.from(prismAuths.get(i).decrypt(cipher)))
        .reduce((sum, cvkAuthi) => sum.add(cvkAuthi), ed25519Point.infinity);

      // Add a full flow for cmk
      // return S , VUID,timestamp2 for cvk flow
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
      //return the encryped value 

      return [AESKey.seed(gPrism.toArray()), token];
    } catch (err) {
      return Promise.reject(err);
    }
  }

  async logIn2(password, point){
    try {
      const startTimer = getCSharpTime(Date.now());

      const n = bigInt(ed25519Point.order.toString());

      const [prismAuths, decryptedResponses,VERIFYi, r2Inv, lis] = await this.doConvert(password, point);  //getting r2Inv here is a little messy, but saves a headache
    
      const cln = this.clienSet.get(0); // chnage this later
      const dnsCln = new DnsClient(cln.baseUrl, cln.userGuid);
      const [, cmkpub] = await dnsCln.getInfoOrks(); // pubs is the list of mgOrki
      
      const gUserCMK = decryptedResponses.map((b, i) => b.gBlurUserCMKi.times(lis.get(i))).reduce((sum, gBlurUserCMKi) => sum.add(gBlurUserCMKi), ed25519Point.infinity).times(r2Inv); // check li worked here
      const gCMK2 = decryptedResponses.map((b, i) => b.gCMK2.times(lis.get(i))).reduce((sum, gCMK2) => sum.add(gCMK2), ed25519Point.infinity); //Correct??
      
      const hash_gUserCMK = Hash.sha512Buffer(gUserCMK.toArray());
      const CMKmul = bigInt_fromBuffer(hash_gUserCMK.subarray(0, 32)); // first 32 bytes
      const VUID = IdGenerator.seed(hash_gUserCMK.subarray(32, 64)); /// last 32 bytes
      
      const Sesskey = random();
      const gSesskeyPub = ed25519Point.g.times(Sesskey);

      const deltaTime = median(decryptedResponses.values.map(a => Number(a.certTime.ticks.toString()))) - startTimer;
      const timestamp2 = getCSharpTime(Date.now()) + deltaTime;
      const AAA =  median(decryptedResponses.values.map(a => Number(a.certTime.ticks.toString()))) - timestamp2;
      const certTimei = median(decryptedResponses.values.map(a => a.certTime));
      
      // Begin PreSignInCVK here to save time
      const jwt = createJWT_toSign(VUID.guid, gSesskeyPub, timestamp2); // Tide JWT here 
      const pre_gCVKR = this.clienSet.map(lis, (dAuthClient, li, i) => dAuthClient.PreSignInCVK(VUID.guid, timestamp2, gSesskeyPub, jwt)); 

      const gCMKAuth = cmkpub.y.times(CMKmul); // get gCMK from DNS call at beginning

      const r4 = random();
      const r4Inv = r4.modInv(n);
       
      const M = Hash.shaBuffer(timestamp2.toString() + Buffer.from(gSesskeyPub.toArray()).toString('base64')); // TODO: Add point.to_base64 function

      const H = Hash.shaBuffer( Buffer.concat([Buffer.from(gCMKAuth.toArray()), M]));
      const H_int = bigInt_fromBuffer(H);
      const blurHCMKmul = (bigInt_fromBuffer(H).times(CMKmul).times(r4)).mod(n); // H * CMKmul * r4 % n
     

      const jsonObject = (userID, certTimei, blurHCMKmul) =>  JSON.stringify( { UserId: userID.toString(), CertTime: certTimei.toString(), BlurHCmkMul: blurHCMKmul.toString() } );
      const encAuthRequest = decryptedResponses.map((res, i) => prismAuths.get(i).encrypt(jsonObject(this.userID.guid, res.certTime, blurHCMKmul)).toString('base64'));

      const Encrypted_Auth_Resp = await this.clienSet.map(lis, (dAuthClient, li, i) => dAuthClient.Authenticate(encAuthRequest.get(i), decryptedResponses.get(i).certTime, VERIFYi.get(i))); 
      const Decrypted_json = Encrypted_Auth_Resp.values.map((encryptedSi, i) => JSON.parse(prismAuths.get(i).decrypt(encryptedSi).toString())); // decrypt encrypted json from authenticate req
      
      const S = Decrypted_json.map((s, i) => bigInt_fromBuffer(Buffer.from(s.si, 'base64')).times(lis.get(i))).reduce((sum, s) => sum.add(s)).times(r4Inv).mod(n);  // Sum (Si) * r4Inv % n
      const gRmul = Decrypted_json.map((s, i) => ed25519Point.from(Buffer.from(s.gRi, 'base64')).times(lis.get(i))).reduce((sum, s) => sum.add(s), ed25519Point.infinity).times(r4Inv);  // Sum (gRi) * r4Inv % n 

      const string_hash = bigInt_fromBuffer(Hash.shaBuffer("CMK authentication"));

      const _8N = BigInt(8);

      if(!ed25519Point.g.times(S).times(_8N).isEqual(gRmul.times(_8N).add(gCMKAuth.times(H_int).times(string_hash).times(_8N)))) {
        return Promise.reject("Ork Blind Signature Invalid")
      }

      const cvkDnsCln = new DnsClient(cln.baseUrl, VUID.guid);
      const [vIdORK , cvkPub ] = await cvkDnsCln.getInfoOrks(); // pubs is the list of mgOrki   TODO: Try to get a Dictinairy here
      const orksPubs = await this.clienSet.all(c => c.getPublic()); 

      const ECDHi = orksPubs.map(pub => AESKey.seed(Hash.shaBuffer(pub.y.times(Sesskey).toArray())));

      const enc_gCVKR = await pre_gCVKR;
      const gCVKR = enc_gCVKR.map((enc_gCVKRi, i) => ed25519Point.from(Buffer.from(ECDHi.get(i).decrypt(enc_gCVKRi), 'base64')).times(lis.get(i))).reduce((sum, p) => sum.add(p), ed25519Point.infinity);  //array used. change later
      
      const encrypted_CVKS = await this.clienSet.map(lis, (dAuthClient, li, i) => dAuthClient.SignInCVK(VUID.guid, gRmul, S, timestamp2, gSesskeyPub, jwt, gCMKAuth, gCVKR, cvkPub.y));
      const CVKS = await encrypted_CVKS.values.map((encCVKsigni, i) =>  bigInt_fromBuffer(Buffer.from(ECDHi.get(i).decrypt(encCVKsigni),'base64')).times(lis.get(i))).reduce((sum, p) => sum.add(p)).mod(n);  //array used. change later

      const H_cvk = bigInt_fromBuffer(Hash.sha512Buffer(Buffer.concat([Buffer.from(gCVKR.compress()), Buffer.from(cvkPub.y.compress()), Buffer.from(jwt)])));
      
      if(!ed25519Point.g.times(CVKS).times(_8N).isEqual(gCVKR.times(_8N).add(cvkPub.y.times(H_cvk).times(_8N)))) { // everything good. JWT should verify
        return Promise.reject("Ork CVK Signature Invalid")
      }

      const finalJWT = addSigtoJWT(jwt, gCVKR, CVKS);
      const finalPem = new ed25519Key(0, cvkPub.y).getPemPublic();

      /// IT WORKS! finalJWT can be verified by finalPem with ANY library out there that supports EdDSA!!!!

      return  {tideJWT : finalJWT, cvkPubPem : finalPem, gCMKAuth : gCMKAuth , vuid : VUID};
    } catch (err) {
      return Promise.reject(err);
    }
  }

    /**
     *  @param {string} pass
     *  @param {ed25519Point} gVVK
     *  @returns {Promise<[Dictionary<AESKey>, Dictionary<ApplyResponseDecrypted>, Dictionary<TranToken>, bigInt.BigInteger, Dictionary<bigInt.BigInteger>]>} // Returns gPassprism + encrypted CMK values + r2Inv (to un-blur gBlurPassPrism)
    */
     async doConvert(pass, gVVK) {
      try {
        const n = bigInt(ed25519Point.order.toString());
        const gPass = ed25519Point.fromString(pass);
        const gUser = ed25519Point.fromString(this.userID.guid.toString() + gVVK.toArray().toString()) // replace this with proper hmac + point to hash function

        const r1 = random();
        const r2 = random();

        const gBlurUser = gUser.times(r2);
        const gBlurPass = gPass.times(r1);
       
        const idGens = await this.clienSet.all(c => c.getClientGenerator()); // implement method to only use first 14 orks that reply
        const ids = idGens.map(idGen => idGen.id);
        const lis = ids.map(id => SecretShare.getLi(id, ids.values, bigInt(ed25519Point.order.toString())));
        const pre_Prismis =  this.clienSet.map(lis, (dAuthClient, li) => dAuthClient.Convert(gBlurUser, gBlurPass, li)); // li is not being sent to ORKs. Instead, when gBlurPassPRISM is returned, it is multiplied by li locally
        // would've been neater to do this mutliplication of point * li at gPassRPrism line
                                                                                                               
        const r1Inv = r1.modInv(n);
        const r2Inv = r2.modInv(n);
        const prismResponse = await pre_Prismis;
        const gPassPrism = prismResponse.values.map(a =>  a[0]).reduce((sum, point) => sum.add(point),ed25519Point.infinity).times(r1Inv);// li has already been multiplied above, so no need to do it here
        //const gPassPrism = gPassRPrism; 
        const encryptedResponses = prismResponse.map(a => a[1]);
         //decryption
        const gPRISMAuth = bigInt_fromBuffer(Hash.shaBuffer(gPassPrism.toArray())); 
        const pubs = await this.clienSet.all(c => c.getPublic()); //works
        const prismAuths = pubs.map(pub =>  AESKey.seed(Hash.shaBuffer(pub.y.times(gPRISMAuth).toArray())));
    
        const decryptedResponses = encryptedResponses.map((cipher, i) => ApplyResponseDecrypted.from(prismAuths.get(i).decrypt(cipher))); 
         // functional function to append userID bytes to certTime bytes FAST
        const create_payload = (certTime_bytes) => {
          const newArray = new Uint8Array(this.userID.buffer.length + certTime_bytes.length);
          newArray.set(this.userID.buffer);
          newArray.set(certTime_bytes, this.userID.buffer.length);
          return newArray // returns userID + certTime
        }
     
        const VERIFYi = decryptedResponses.map((response, i) => new TranToken().sign(prismAuths.get(i), create_payload(response.certTime.toArray())));
        
        return [prismAuths, decryptedResponses, VERIFYi, r2Inv, lis];
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
  async GenShard(pass){
    try{
      const n = bigInt(ed25519Point.order.toString());
      const mIdORKs = await this.clienSet.all(c => c.getClientUsername());

      const r = random();
      const gPass = ed25519Point.fromString(pass);
      const gBlurPass = gPass.times(r);
      const rInv = r.modInv(n);
      
      const gMul1 = Buffer.from(gBlurPass.toArray()).toString('base64');
      const multipliers = [gMul1];
      
      const genShardResp = await this.clienSet.all(dAuthClient => dAuthClient.genShard(mIdORKs,  1, multipliers));
   
      /**
       * @param {ed25519Point[]} share1 
       * @param {ed25519Point[]} share2 
       */
      const addShare = (share1, share2) => {
        return share1.map((s, i) => s.add(share2[i]))
      }
      const gMultiplied = genShardResp.values.map(p => p[2]).reduce((sum, next) => addShare(sum, next)); 

      const gPassPrism = gMultiplied[0].times(rInv);

      const gPRISMAuth = ed25519Point.g.times(bigInt_fromBuffer(Hash.shaBuffer(gPassPrism.toArray()))); 
      const timestamp = median(genShardResp.values.map(resp => resp[3])); 
      
      const mergeShare=(share) =>{
        return share.map(p => GenShardShareResponse.from(p));
      }
      const shareEncrypted = genShardResp.values.map(a =>  a[1]).map(s => mergeShare(s));
      const sortedShareArray = sorting(shareEncrypted);

      return { gPRISMAuth : gPRISMAuth, ciphers : sortedShareArray, timestamp : timestamp}

    }catch(err){
      Promise.reject(err);
    }
  }

  async SetPRISM(ciphers, timestamp){
    try{
      const mIdORKs = await this.clienSet.all(c => c.getClientUsername());

      const pre_setCMKResponse = this.clienSet.all((DAuthClient, i) => DAuthClient.setCMK(filtering(ciphers.filter(element => element.orkId === mIdORKs.get(i))), timestamp, mIdORKs));

      const idGens = await this.clienSet.all(c => c.getClientGenerator()); // implement method to only use first 14 orks that reply
      const ids = idGens.map(idGen => idGen.id);
      const lis = ids.map(id => SecretShare.getLi(id, ids.values, bigInt(ed25519Point.order.toString()))); 
    
      const setCMKResponse = await pre_setCMKResponse;

      const gPRISMtest = setCMKResponse.values.map(resp => resp[0]).reduce((sum, next, i) => sum.add(next[0].times(lis.get(i))), ed25519Point.infinity);
    
      const encryptedStatei = setCMKResponse.values.map(resp => resp[2]);

      return {gPRISMtest : gPRISMtest, state : encryptedStatei};
    }catch(err){
      Promise.reject(err);
    }
    
  }
  /**
   * @param {ed25519Point} gPRISMtest
   * @param {string[]} state
   * @param {Dictionary<ApplyResponseDecrypted>} decryptedResponses
   * @param {ed25519Point} gPrismAuth
   * @param {Dictionary<TranToken>} VERIFYi
   */
  async CommitPRISM (gPRISMtest, state, decryptedResponses, gPrismAuth, VERIFYi){
    try{ 
      await this.clienSet.all((DAuthClient,i) => DAuthClient.CommitPrism(state[i], decryptedResponses.get(i).certTime, VERIFYi.get(i), gPRISMtest, gPrismAuth));  
    }catch(err){
      Promise.reject(err);
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

/**
 * 
 * @param {Buffer} buffer 
 * @returns 
 */
function bigInt_fromBuffer(buffer){
  var a = Buffer.from(buffer);
  return bigInt.fromArray(Array.from(a.reverse()), 256, false).mod(bigInt(ed25519Point.order.toString()))
}

function random() {
  return Utils.random(bigInt.one, bigInt((ed25519Point.order - BigInt(1)).toString()));
}

function median(numbers) {
  const sorted = numbers.sort();//Array.from(numbers).sort((a, b) => a - b);
  const middle = Math.floor(sorted.length / 2);

  if (sorted.length % 2 === 0) {
      return (sorted[middle - 1]+(sorted[middle])/(2));
  }

  return sorted[middle];
}

function getCSharpTime(ticks){
  return (ticks * 10000);
}

/**
 * @param {Guid} vuid 
 * @param {Number} expTime
 * @param {ed25519Point} gSessKeyPub 
 * @returns {string}
 */
function createJWT_toSign(vuid, gSessKeyPub, expTime){
  // header = {"typ": "JWT", "alg": "EdDSA", "crv": "Ed25519"} base64urlencoded
  const header = 'eyJ0eXAiOiAiSldUIiwgImFsZyI6ICJFZERTQSIsICJjcnYiOiAiRWQyNTUxOSJ9.'; 
  const payload = Buffer.from(JSON.stringify({vuid: vuid.toString(), exp: expTime, sessionKeyPub: Buffer.from(gSessKeyPub.compress()).toString('base64url')})).toString('base64url');
  return header + payload;
}

/**
* @param {string} jwt 
* @param {ed25519Point} R 
* @param {bigInt.BigInteger} s 
* @returns {string}
*/
function addSigtoJWT(jwt, R, s) {
  const sBuff = s.toArray(256).value;
  while(sBuff.length < 32){sBuff.unshift(0);} // pad if array < 32
  sBuff.reverse(); // to LE

  const RBuff = Array.from(R.compress());
  while(RBuff.length < 32){RBuff.unshift(0);} // pad if array < 32

  var sig = Buffer.alloc(64);
  sig.set(RBuff, 0);
  sig.set(sBuff, 32);
 
  return jwt + '.' + sig.toString('base64url');
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