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

import { AESKey, Hash } from "cryptide";
import Num64 from "../Num64";
import DAuthFlow from "./DAuthFlow";
import IdGenerator from "../IdGenerator";
import DCryptFlow from "./DCryptFlow";
import VendorClient from "../VendorClient";
import RuleClientSet from "./RuleClientSet";
import KeyClientSet from "./keyClientSet";
import KeyStore from "../keyStore";
import Rule from "../rule";
import Cipher from "../Cipher";

export default class DAuthV2Flow {
  get vuid() {
    return this._getCvkFlow().user;
  }

  /** @param {string} user */
  constructor(user) {
    this.user = user;

    /** @type {string[]} */
    this.cmkUrls = null;

    /** @type {string[]} */
    this.cvkUrls = null;

    /** @type {string} */
    this.vendorUrl = null;

    /** @type {AESKey} */
    this.cmkAuth = null;
  }

  /**
   * @param {string} password
   * @param {string} email
   * @param {number} threshold
   */
  async signUp(password, email, threshold) {
    try {
      const vendorCln = this._getVendorClient();
      const { pubKey } = await vendorCln.configuration();

      // register cmk
      const flowCmk = this._getCmkFlow();
      this.cmkAuth = await flowCmk.signUp(password, email, threshold);

      // register cvk
      const flowCvk = this._getCvkFlow();
      const vuid = flowCvk.user;
      const cvk = await flowCvk.signUp(this.cmkAuth, threshold);

      //register vendor account
      const vuidAuth = AESKey.seed(cvk.toArray()).derive(vendorCln.guid.toArray());
      const vendorToken = await vendorCln.signup(vuid, vuidAuth);

      //user register rule
      const ruleCln = new RuleClientSet(this.cmkUrls, vuid);
      const keyCln = new KeyClientSet(this.cmkUrls);

      const tokenTag = Num64.seed("token");
      const vendorPubStore = new KeyStore(pubKey);
      const allowTokenToVendor = Rule.allow(vuid, tokenTag, vendorPubStore);

      await Promise.all([keyCln.setOrUpdate(vendorPubStore), ruleCln.setOrUpdate(allowTokenToVendor)]);

      //user encrypt vendor token
      const hashToken = Hash.shaBuffer(vendorToken.toArray());
      const cipher = Cipher.encrypt(hashToken, tokenTag, cvk);

      //test dauth and dcrypt
      const vuidAuthTag = await this.logIn(password);
      await vendorCln.signin(vuid, vuidAuthTag);

      const dcryptOk = await vendorCln.testCipher(vuid, vendorToken, cipher);
      if (!dcryptOk || vuidAuth.toString() !== vuidAuthTag.toString()) return Promise.reject(new Error("Error in the verification workflow"));

      return vuidAuth;
    } catch (err) {
      return Promise.reject(err);
    }
  }

  /** @param {string} password */
  async logIn(password) {
    try {
      const flowCmk = this._getCmkFlow();
      this.cmkAuth = await flowCmk.logIn(password);

      const flowCvk = this._getCvkFlow();
      const cvkTag = await flowCvk.getKey(this.cmkAuth);

      const vendorCln = this._getVendorClient();
      const vuidAuth = AESKey.seed(cvkTag.toArray()).derive(vendorCln.guid.toArray());

      return vuidAuth;
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

  _getVendorClient() {
    if (this.vendorUrl === null || this.vendorUrl.length === 0) throw new Error("vendorUrl must not be empty");

    if (this._vendorClient === undefined) this._vendorClient = new VendorClient(this.vendorUrl);

    return this._vendorClient;
  }

  _getCmkFlow() {
    if (this.cmkUrls === null || this.cmkUrls.length === 0) throw new Error("cmkUrls must not be empty");

    if (this._cmkFlow === undefined) this._cmkFlow = new DAuthFlow(this.cmkUrls, this.user);

    return this._cmkFlow;
  }

  _getCvkFlow() {
    if (this.cvkUrls === null || this.cvkUrls.length === 0) throw new Error("cvkUrls must not be empty");

    if (this.cmkAuth == null) throw new Error("cmkAuth must not be empty");

    if (this._cvkFlow === undefined) {
      const vuid = IdGenerator.seed(this.user, this.cmkAuth).guid;
      this._cvkFlow = new DCryptFlow(this.cvkUrls, vuid);
    }

    return this._cvkFlow;
  }
}
