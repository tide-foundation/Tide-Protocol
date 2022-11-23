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

export default class DAuthV3Flow {
  /** @param {string|Guid} user */
  constructor(user) {
    /** @type {string} */
    this.homeUrl = null;

    /** @type {string[]} */
    this.cmkUrls = null;

    /** @type {string[]} */
    this.cvkUrls = null;

    /** @type {string} */
    this.vendorUrl = null;

    /** @type {Guid} */
    this.vuid = null;

    this.userid = typeof user === "string" ? Guid.seed(user) : user;
  }

  /** @param {AESKey} key */
  _genVuid() {
    if (!this.cvkAuth) throw new Error("cvkAuth is needed");
    this.vuid = IdGenerator.seed(this.userid.buffer, this.cvkAuth).guid; // change this -----------------------------------
  }

  /**
   * @param {string} password
   * @param {string|string[]} email
   * @param {number} threshold
   * @returns {Promise<{ vuid: Guid; cvk: ed25519Key; auth: AESKey; }>}
   */
  async signUp(password, email, threshold) {
    try {
      const vendorCln = this._getVendorClient();
      const { vendorPubKey } = await vendorCln.configuration();

      const flowCmk = await this._getCmkFlow();
      const flowCvk = await this._getCvkFlow();

      const keyCln = new KeyClientSet(this.cmkUrls);

      const vendorPubStore = new KeyStore(vendorPubKey); // come back to here for GUID stuff
      await keyCln.setOrUpdate(vendorPubStore); // will require a authorization token one day. This adds the vendor's public key to the ORK's keystore

      // register cvk account
      const bufferVid = IdGenerator.seed(vendorPubKey.toArray()).buffer; // maybe keep this
      const vuidAuth = AESKey.seed(cvk.toArray()).derive(bufferVid); // change this to something that doesn't include cvk
      const [vendorToken, signatures] = await vendorCln.signup(this.vuid, vuidAuth, this.cvkUrls);

      // create cmk shards
      const {vuid, gCMKAuth, gPRISMAuth, timestampCMK, ciphersCMK} = await flowCmk.GenShardCMK(password, vendorPubKey.y);
      this.vuid = vuid.guid;
      const pre_SetCMK = flowCmk.SetCMK(ciphersCMK, timestampCMK);

      // create cvk shards
      const {timestampCVK , ciphersCVK} = await flowCvk.GenShardCVK(_, _); // voucher in paramter. consider removing 

      //Aggredate shards
      const SetCVK_state = await flowCvk.SetCVK(ciphersCVK, timestampCVK, gCMKAuth);
      const SetCMK_state = await pre_SetCMK;

      /// -------- Test sign in + decryption --------------
      const {jwt, cvkPub} = await this.logIn(password);  
      await vendorCln.signin(this.vuid, vuidAuthTag); // vuidAuthTag will be replaces by signed JWT

      //user encrypt vendor token
      const hashToken = Hash.shaBuffer(vendorToken.toArray());
      const cipher = Cipher.encrypt(hashToken, Tags.vendor, cvkPub);

      const dcryptOk = await vendorCln.testCipher(this.vuid, vendorToken, cipher);
      if (!dcryptOk || vuidAuth.toString() !== vuidAuthTag.toString()) return Promise.reject(new Error("Error in the verification workflow"));
      /// ----------------------------------

      // this will probably change
      const pre_CommitCMK = flowCmk.Commit(SetCMK_state.gTests, SetCMK_state.gCMKR2, SetCMK_state.state, timestampCMK, gPRISMAuth, email);
      const pre_CommitCVK = flowCvk.Commit(SetCVK_state.gTests, SetCVK_state.gCMKR2, SetCVK_state.state, timestampCVK, gPRISMAuth, email); // will also require signatures from above ^^




      await Promise.all([flowCmk.confirm(), flowCvk.confirm()]);

      return { vuid: this.vuid, cvk, auth: vuidAuth };
    } catch (err) {
      return Promise.reject(err);
    }
  }

  async logIn(password){
    try {
      const vendorCln = this._getVendorClient();
      const { pubKey } = await vendorCln.configuration();

      const pre_flowCmk = this._getCmkFlow();
      const flowCmk = await pre_flowCmk;

      // TODO: Use _getCvkFlow()

      // keep in mind logIn2 gets the CVK urls from the DNSEntry, that won't exist at this point in time TODO: change how we get the CVK urls
      const [tideJWT, cvkPubPem] = await flowCmk.logIn2(password, pubKey.y);  // this includes both the cvk and cmk flows for sign in. TODO: split them later
      
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
    await (await this._getCmkFlow()).changePass(pass, newPass, threshold);
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
