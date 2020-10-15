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

import { AESKey, Utils, C25519Point, C25519Key, CP256Key } from "cryptide";
import DAuthFlow from "./DAuthFlow";
import IdGenerator from "../IdGenerator";
import DCryptFlow from "./DCryptFlow";
import Guid from "../guid";

export default class DAuthJwtFlow {
  /** @param {string} user */
  constructor(user, newCvk = false) {
    this.user = user;

    /** @type {string[]} */
    this.cmkUrls = null;

    /** @type {string[]} */
    this.cvkUrls = null;

    /** @type {bigInt.BigInteger} */
    this.cmk = null;

    /** @type {AESKey} */
    this.cmkAuth = null;

    /** @type {Guid} */
    this.vuid = null;

    /** @type {CP256Key} */
    this.vendoPub = null;

    this.userid = IdGenerator.seed(this.user).guid;

    if (newCvk) this.generateCvk();
  }

  generateCvk() {
    this.cmk = Utils.random(1, C25519Point.n.subtract(1));
    this._setCmk(AESKey.seed(Buffer.from(this.cmk.toArray(256).value)));
  }

  /** @param {AESKey} key
   * @private */
  _setCmk(key) {
    this.cmkAuth = key;
    this.vuid = IdGenerator.seed(this.user, key).guid;
  }

  /**
   * @param {string} password
   * @param {string} email
   * @param {number} threshold
   * @returns {Promise<{ vuid: Guid; cvk: CP256Key; auth: AESKey; }>}
   */
  async signUp(password, email, threshold) {
    if (!this.vendoPub)
      throw new Error("vendoPub must not be empty");
    
    try {
      if (this.cmk === null) this.generateCvk();

      const cvk = C25519Key.generate();
      const cvkJwt = CP256Key.private(cvk.x);
      const flowCmk = this._getCmkFlow();
      const flowCvk = this._getCvkFlow();

      //vendor
      const keyId = Guid.seed(this.vendoPub.toArray());
      const vuidAuth = AESKey.seed(cvkJwt.toArray()).derive(keyId.buffer);
      const signatures = flowCvk.clients.map(_ => new Uint8Array());

      // register cmk
      await flowCmk.signUp(password, email, threshold, this.cmk);

      // register cvk
      await flowCvk.signUp(this.cmkAuth, threshold, keyId, signatures, cvk);

      //test dauth and dcrypt
      const { cvk: cvkTag } = await this.logIn(password);

      if (cvkJwt.toString() !== cvkTag.toString())
        return Promise.reject(new Error("Error in the verification workflow"));

      await Promise.all([flowCmk.confirm(), flowCvk.confirm()]);

      return { vuid: this.vuid, cvk: cvkJwt, auth: vuidAuth };
    } catch (err) {
      return Promise.reject(err);
    }
  }

  /** @param {string} password
  * @returns {Promise<{ vuid: Guid; cvk: CP256Key; auth: AESKey; }>} */
  async logIn(password) {
    if (!this.vendoPub)
      throw new Error("vendoPub must not be empty");

    try {
      const flowCmk = this._getCmkFlow();
      this._setCmk(await flowCmk.logIn(password));

      const flowCvk = this._getCvkFlow();
      const cvk = await flowCvk.getKey(this.cmkAuth, true);
      const cvkJwt = CP256Key.private(cvk.x);

      const keyId = Guid.seed(this.vendoPub.toArray());
      const vuidAuth = AESKey.seed(cvkJwt.toArray()).derive(keyId.buffer);

      return { vuid: this.vuid, cvk: cvkJwt, auth: vuidAuth };
    } catch (err) {
      return Promise.reject(err);
    }
  }

  async Recover() {
    await this._getCmkFlow().Recover();
  }

  /**
   * @param {string} textShares
   * @param {string} newPass
   * @param {number} threshold
   */
  async Reconstruct(textShares, newPass = null, threshold = null) {
    await this._getCmkFlow().Reconstruct(textShares, newPass, threshold);
  }

  /**
   * @param {string} pass
   * @param {string} newPass
   * @param {number} threshold
   */
  async changePass(pass, newPass, threshold) {
    await this._getCmkFlow().changePass(pass, newPass, threshold);
  }

  /** @private */
  _getCmkFlow() {
    if (this.cmkUrls === null || this.cmkUrls.length === 0) throw new Error("cmkUrls must not be empty");

    if (this._cmkFlow === undefined) this._cmkFlow = new DAuthFlow(this.cmkUrls, this.user);

    return this._cmkFlow;
  }

  /** @private */
  _getCvkFlow() {
    if (this.cvkUrls === null || this.cvkUrls.length === 0) throw new Error("cvkUrls must not be empty");

    if (this.vuid === null) throw new Error("vuid must not be empty");

    if (this._cvkFlow === undefined) this._cvkFlow = new DCryptFlow(this.cvkUrls, this.vuid);

    return this._cvkFlow;
  }
}
