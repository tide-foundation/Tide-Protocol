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

import { AESKey, Hash, Utils, C25519Point, C25519Key, ed25519Key, ed25519Point } from "cryptide";
import Tags from "../tags";
import DAuthFlow from "./DAuthFlow";
import IdGenerator from "../IdGenerator";
import DCryptFlow from "./DCryptFlow";
import VendorClient from "../VendorClient";
import KeyClientSet from "./keyClientSet";
import KeyStore from "../keyStore";
import Cipher from "../Cipher";
import Guid from "../guid";
import DnsClient from "./DnsClient";
import bigInt from "big-integer";

export default class DAuthV2Flow {
  /** @param {string|Guid} user */
  constructor(user, newCvk = false) {
    /** @type {string} */
    this.homeUrl = null;

    /** @type {string[]} */
    this.cmkUrls = null;

    /** @type {string[]} */
    this.cvkUrls = null;

    /** @type {string} */
    this.vendorUrl = null;

    /** @type {bigInt.BigInteger} */
    this.cmk = null;

    /** @type {AESKey} */
    this.cvkAuth = null;

    /** @type {Guid} */
    this.vuid = null;

    /** @type {CP256Key} */
    this.vendorPub = null;

    this.userid = typeof user === "string" ? Guid.seed(user) : user;
  }

  /** @param {AESKey} key */
  _genVuid() {
    if (!this.cvkAuth) throw new Error("cvkAuth is needed");
    this.vuid = IdGenerator.seed(this.userid.buffer, this.cvkAuth).guid;
  }

  /**
   * @param {string} password
   * @param {string|string[]} email
   * @param {number} threshold
   * @returns {Promise<{ vuid: Guid; cvk: ed25519Key; auth: AESKey; }>}
   */
  async signUp(password, email, threshold) {
    if (!this.vendorPub) throw new Error("vendorPub must not be empty");

    try {   
      // get CMK Orks details
      const pre_flowCmk = this._getCmkFlow(true);
      const venPnt = ed25519Point.fromString(this.vendorPub.y.toArray());
      const flowCmk = await pre_flowCmk;

      // create cmk shards
      const {vuid, gCMKAuth, gPRISMAuth, timestampCMK, ciphersCMK, gCMK} = await flowCmk.GenShardCMK(password, venPnt);

      // getCVK Ork details
      this.vuid = vuid.guid;
      const flowCvk = await this._getCvkFlow(true);
      
      // Aggregate shards
      const pre_SetCMK = await flowCmk.SetCMK(ciphersCMK, timestampCMK);

      // create cvk shards
      const {timestampCVK , ciphersCVK, gCVK} = await flowCvk.GenShardCVK(venPnt,venPnt);

      //Aggredate shards
      const pre_SetCVK = await flowCvk.SetCVK(ciphersCVK, timestampCVK, gCMKAuth);

      const pre_CommitCMK = await flowCmk.PreCommit(pre_SetCMK.gTests, pre_SetCMK.gCMKR2, pre_SetCMK.state , pre_SetCMK.randomKey, timestampCMK, gPRISMAuth, email, gCMK);

      const {cvkPubPem} = await flowCvk.PreCommit(pre_SetCVK.gTests, pre_SetCVK.gCVKR2, pre_SetCVK.state,  pre_SetCVK.randomKey, vuid, timestampCVK, gCMKAuth, gCVK);
     
      return { vuid: this.vuid, cvkPub: cvkPubPem };
       
    } catch (err) {
      return Promise.reject(err);
    }
  }

  /** @param {string} password */
  async logIn(password) {
    try {
      
      const pre_flowCmk = this._getCmkFlow();
      const venPnt = ed25519Point.fromString(this.vendorPub.y.toArray());
      const flowCmk = await pre_flowCmk;

      const {tideJWT, cvkPubPem}  = await flowCmk.logIn2(password, venPnt); 

      return {jwt: tideJWT, cvkPub: cvkPubPem}; // return vuid + jwt signature
    } catch (err) {
      return Promise.reject(err);
    }
  }

  async Recover() {
    await (await this._getCmkFlow()).Recover();
  }

  /**
   * @param {string} textShares
   * @param {string} newPass
   * @param {number} threshold
   */
  async Reconstruct(textShares, newPass = null, threshold = null) {
    await (await this._getCmkFlow()).Reconstruct(textShares, newPass, threshold);
  }

  /**
   * @param {string} pass
   * @param {string} newPass
   * @param {number} threshold
   */
  async changePass(pass, newPass, threshold) {
    //await (await this._getCmkFlow()).changePass(pass, newPass, threshold);
    try {
      // get CMK Orks details
      const pre_flowCmk = this._getCmkFlow(true);
      const venPnt = ed25519Point.fromString(this.vendorPub.y.toArray());
      const flowCmk = await pre_flowCmk;

      const [, decryptedResponses, VERIFYi, , ] = await flowCmk.doConvert( pass, venPnt);  

      // create shards
      const {gPRISMAuth, ciphers, timestamp} = await flowCmk.GenShard(newPass);

      const set_PRISM = await flowCmk.SetPRISM(ciphers, timestamp);

      await flowCmk.CommitPRISM(set_PRISM.gPRISMtest, set_PRISM.state, decryptedResponses, gPRISMAuth, VERIFYi);
     
    } catch (err) {
      return Promise.reject(err);
    }
  }
  
  /** @private */
  async _setCvkUrlFromDns() {
    if (this.cvkUrls && this.cvkUrls.length > 0)
      return;

    const dnsCln = new DnsClient(this.cmkUrls[0], this.vuid);
    const [cvkUrls] = await dnsCln.getInfoOrks();
    this.cvkUrls = cvkUrls;
  }

  _getVendorClient() {
    if (this.vendorUrl === null || this.vendorUrl.length === 0) throw new Error("vendorUrl must not be empty");

    if (this._vendorClient === undefined) this._vendorClient = new VendorClient(this.vendorUrl);

    return this._vendorClient;
  }

  async _getCmkFlow(memory = false) {
    if (this._cmkFlow) return this._cmkFlow;

    if (this.homeUrl && (!this.cmkUrls || !this.cmkUrls.length)) {
      const dnsCln = new DnsClient(this.homeUrl, this.userid);
      const [cmkUrls] = await dnsCln.getInfoOrks();
      this.cmkUrls = cmkUrls;
    }

    if (this.cmkUrls && this.cmkUrls.length > 0)
      return this._cmkFlow = new DAuthFlow(this.cmkUrls, this.userid, memory);

    throw new Error("cmkUrls or homeUrl must be provided");
  }

  async _getCvkFlow(memory = false) {
    if (!this.vuid) throw new Error("vuid must not be empty");

    if (this._cvkFlow) return this._cvkFlow;

    if ((!this.cvkUrls || !this.cvkUrls.length) && (this.homeUrl || (this.cmkUrls && this.cmkUrls.length))) {
      const dnsCln = new DnsClient(this.homeUrl || this.cmkUrls[0], this.vuid);
      const [cvkUrls] = await dnsCln.getInfoOrks();
      this.cvkUrls = cvkUrls;
    }

    if (this.cvkUrls && this.cvkUrls.length > 0)
      return this._cvkFlow = new DCryptFlow(this.cvkUrls, this.vuid, memory);

    throw new Error("cvkUrls must not be empty");
  }
}
