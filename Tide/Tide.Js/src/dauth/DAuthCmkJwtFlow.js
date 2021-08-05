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

import { AESKey, CP256Key, C25519Point } from "cryptide";
import IdGenerator from "../IdGenerator";
import DCryptFlow from "./DCryptFlow";
import Guid from "../guid";
import DnsClient from "./DnsClient";

export default class DAuthCmkJwtFlow {
  /** @param {string|Guid} user */
  constructor(user) {
    /** @type {string} */
    this.homeUrl = null;

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

  /** @returns {Promise<{ vuid: Guid; cvk: CP256Key; auth: AESKey; }>} */
  async logIn() {
    if (!this.vendorPub) throw new Error("vendorPub must not be empty");
    if (!this.cmk) throw new Error("cmk must not be empty");

    try {
      const venPnt = C25519Point.fromString(this.vendorPub.y.toArray());
      this.cvkAuth = AESKey.seed(venPnt.times(this.cmk).toArray());
      this.vuid = IdGenerator.seed(this.userid.buffer, this.cvkAuth).guid;

      const flowCvk = await this._getCvkFlow();
      const cvk = await flowCvk.getKey(this.cvkAuth);

      const keyId = Guid.seed(this.vendorPub.toArray());
      const vuidAuth = AESKey.seed(cvk.toArray()).derive(keyId.buffer);

      return { vuid: this.vuid, cvk: cvk, auth: vuidAuth };
    } catch (err) {
      return Promise.reject(err);
    }
  }

  /** @private */
  async _getCvkFlow(memory = false) {
    if (!this.vuid) throw new Error("vuid must not be empty");

    if (this._cvkFlow) return this._cvkFlow;

    if ((!this.cvkUrls || !this.cvkUrls.length) && this.homeUrl) {
      const dnsCln = new DnsClient(this.homeUrl, this.vuid);
      const [cvkUrls] = await dnsCln.getInfoOrks();
      this.cvkUrls = cvkUrls;
    }

    if (this.cvkUrls && this.cvkUrls.length > 0)
      return this._cvkFlow = new DCryptFlow(this.cvkUrls, this.vuid, memory);

    throw new Error("homeUrl must be provided");
  }
}
