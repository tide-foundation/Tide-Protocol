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

import { AESKey, Utils, C25519Point, C25519Key, CP256Key, ed25519Point, ed25519Key } from "cryptide";
import DAuthFlow from "./DAuthFlow";
import IdGenerator from "../IdGenerator";
import DCryptFlow from "./DCryptFlow";
import Guid from "../guid";
import DnsClient from "./DnsClient";
import bigInt from "big-integer";

export default class DAuthJwtFlow {
  /** @param {string|Guid} user */
  constructor(user) {
    /** @type {string} */
    this.homeUrl = null;

    /** @type {string[]} */
    this.cmkUrls = null;

    /** @type {string[]} */
    this.cvkUrls = null;

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

  /** @private */
  _genVuid() {
    if (!this.cvkAuth) throw new Error("cvkAuth is needed");
    this.vuid = IdGenerator.seed(this.userid.buffer, this.cvkAuth).guid;
  }

  /**
   * @param {string} password
   * @param {number} threshold
   * @returns {Promise<{ vuid: Guid; cvk: ed25519Key; auth: AESKey; }>}
   */
  async signUpCVK(password, threshold) {
    if (!this.vendorPub) throw new Error("vendorPub must not be empty");

    try {
      const pre_flowCmk = this._getCmkFlow();
      const venPnt = ed25519Point.fromString(this.vendorPub.y.toArray());
      const flowCmk = await pre_flowCmk;

      this.cvkAuth = await flowCmk.logIn(password, venPnt); 
      this._genVuid();

      const cvk = ed25519Key.generate();
      const flowCvk = await this._getCvkFlow(true);

      //vendor
      const keyId = Guid.seed(this.vendorPub.toArray());
      const vuidAuth = AESKey.seed(cvk.toArray()).derive(keyId.buffer);
      const signatures = flowCvk.clients.map((_) => new Uint8Array());

      // register cvk
      await flowCvk.signUp(this.cvkAuth, threshold, keyId, signatures, cvk);

      //test dauth and dcrypt
      const { cvk: cvkTag } = await this.logIn(password);

      if (cvk.toString() !== cvkTag.toString()) return Promise.reject(new Error("Error in the verification workflow"));

      await flowCvk.confirm();

      return { vuid: this.vuid, cvk: cvk, auth: vuidAuth };
    } catch (err) {
      return Promise.reject(err);
    }
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
      const venPnt = ed25519Point.fromString(this.vendorPub.y.toArray());
     // this.cvkAuth = AESKey.seed(venPnt.times(this.cmk).toArray());
      const cvk = C25519Key.generate();

     // const cvk = ed25519Key.generate();
      const flowCmk = await this._getCmkFlow(true);

      //vendor
      const keyId = Guid.seed(this.vendorPub.toArray());
      const vuidAuth = AESKey.seed(cvk.toArray()).derive(keyId.buffer);

      // register cmk
      this.cvkAuth = await flowCmk.signUp(password, email, threshold, null, venPnt);

      // configure cvk ORKs
      this._genVuid();
      const flowCvk = await this._getCvkFlow(true);
      const signatures = flowCvk.clients.map((_) => new Uint8Array());

      // register cvk
      await flowCvk.signUp(this.cvkAuth, threshold, keyId, signatures, cvk);

      //test dauth and dcrypt
      const { cvk: cvkTag } = await this.logIn(password);

      //if (cvk.toString() !== cvkTag.toString()) return Promise.reject(new Error("Error in the verification workflow"));

      await Promise.all([flowCmk.confirm(), flowCvk.confirm()]);

      return { vuid: this.vuid, cvk: cvk, auth: vuidAuth };
    } catch (err) {
      return Promise.reject(err);
    }
  }

  /** @param {string} password
   * @returns {Promise<{ vuid: Guid; cvk: ed25519Key; auth: AESKey; }>} */
  async logIn(password) {
    if (!this.vendorPub) throw new Error("vendorPub must not be empty");

    try {
      const pre_flowCmk = this._getCmkFlow();
      const venPnt = ed25519Point.fromString(this.vendorPub.y.toArray());
      const flowCmk = await pre_flowCmk;

      this.cvkAuth = await flowCmk.logIn(password, venPnt); 
      this._genVuid();

      const flowCvk = await this._getCvkFlow();
      const cvk = await flowCvk.getKey(this.cvkAuth);

      const keyId = Guid.seed(this.vendorPub.toArray());
      const vuidAuth = AESKey.seed(cvk.toArray()).derive(keyId.buffer);

      return { vuid: this.vuid, cvk: cvk, auth: vuidAuth };
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
  async _getCmkFlow(memory = false) {
    if (this._cmkFlow) return this._cmkFlow;

    if (this.homeUrl && (!this.cmkUrls || !this.cmkUrls.length)) {
      const dnsCln = new DnsClient(this.homeUrl, this.userid);
      const [cmkUrls] = await dnsCln.getInfoOrks();
      this.cmkUrls = cmkUrls;
    }

    if (this.cmkUrls && this.cmkUrls.length > 0) return (this._cmkFlow = new DAuthFlow(this.cmkUrls, this.userid, memory));

    throw new Error("cmkUrls or homeUrl must be provided");
  }

  /** @private */
  async _getCvkFlow(memory = false) {
    if (!this.vuid) throw new Error("vuid must not be empty");

    if (this._cvkFlow) return this._cvkFlow;

    if ((!this.cvkUrls || !this.cvkUrls.length) && (this.homeUrl || (this.cmkUrls && this.cmkUrls.length))) {
      const dnsCln = new DnsClient(this.homeUrl || this.cmkUrls[0], this.vuid);
      const [cvkUrls] = await dnsCln.getInfoOrks();
      this.cvkUrls = cvkUrls;
    }

    if (this.cvkUrls && this.cvkUrls.length > 0) return (this._cvkFlow = new DCryptFlow(this.cvkUrls, this.vuid, memory));

    throw new Error("cvkUrls must not be empty");
  }
}
