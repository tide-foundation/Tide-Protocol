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

import { AESKey, CP256Key } from "cryptide";
import IdGenerator from "../IdGenerator";
import DCryptFlow from "./DCryptFlow";
import Guid from "../guid";
import DnsClient from "./DnsClient";

export default class DAuthCmkJwtFlow {
  /** @param {string} user */
  constructor(user) {
    this.user = user;

    /** @type {string} */
    this.homeUrl = null;

    /** @type {string[]} */
    this.cvkUrls = null;

    /** @type {bigInt.BigInteger}
     * @private */
    this._cmk = null;

    /** @type {AESKey} */
    this.cmkAuth = null;

    /** @type {Guid} */
    this.vuid = null;

    /** @type {CP256Key} */
    this.vendorPub = null;

    this.userid = IdGenerator.seed(this.user).guid;
  }

  /** @returns {bigInt.BigInteger} */
  get cmk() { return this._cmk; }

  /** @param {bigInt.BigInteger} key */
  set cmk(key) {
    this._cmk = key;
    this.cmkAuth = AESKey.seed(Buffer.from(key.toArray(256).value))
    this.vuid = IdGenerator.seed(this.user, this.cmkAuth).guid;
  }

  /** @returns {Promise<{ vuid: Guid; cvk: CP256Key; auth: AESKey; }>} */
  async logIn() {
    if (!this.vendorPub) throw new Error("vendorPub must not be empty");
    if (!this.cmkAuth) throw new Error("cmk must not be empty");

    try {
      await this._setCvkUrlFromDns();
      
      const flowCvk = this._getCvkFlow();
      const cvk = await flowCvk.getKey(this.cmkAuth, true);
      const cvkJwt = CP256Key.private(cvk.x);

      const keyId = Guid.seed(this.vendorPub.toArray());
      const vuidAuth = AESKey.seed(cvkJwt.toArray()).derive(keyId.buffer);

      return { vuid: this.vuid, cvk: cvkJwt, auth: vuidAuth };
    } catch (err) {
      return Promise.reject(err);
    }
  }

  /** @private */
  async _setCvkUrlFromDns() {
    if (this.cvkUrls && this.cvkUrls.length > 0)
      return;
    
    if (!this.homeUrl)
      throw new Error('homeUrl must be provided');

    const dnsCln = new DnsClient(this.homeUrl, this.vuid);
    const [cvkUrls] = await dnsCln.getInfoOrks();
    this.cvkUrls = cvkUrls;
  }

  /** @private */
  _getCvkFlow(memory = false) {
    if (this.cvkUrls === null || this.cvkUrls.length === 0) throw new Error("cvkUrls must not be empty");

    if (this.vuid === null) throw new Error("vuid must not be empty");

    if (this._cvkFlow === undefined) this._cvkFlow = new DCryptFlow(this.cvkUrls, this.vuid, memory);

    return this._cvkFlow;
  }
}
