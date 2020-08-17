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

import { AESKey, Hash, Utils, C25519Point, C25519Key } from "cryptide";
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
import Guid from "../guid";

export default class DAuthV2Flow {
  /** @param {string} user */
  constructor(user, newCvk = false) {
    this.user = user;

    /** @type {string[]} */
    this.cmkUrls = null;

    /** @type {string[]} */
    this.cvkUrls = null;

    /** @type {string} */
    this.vendorUrl = null;

    /** @type {bigInt.BigInteger} */
    this.cmk = null;

    /** @type {AESKey} */
    this.cmkAuth = null;

    /** @type {Guid} */
    this.vuid = null;
    
    this.userid = IdGenerator.seed(this.user).guid;

    if (newCvk)
      this.generateCvk();    
  }

  generateCvk() {
    this.cmk = Utils.random(1, C25519Point.n.subtract(1));
    this.cmkAuth = AESKey.seed(Buffer.from(this.cmk.toArray(256).value));
    this.vuid = IdGenerator.seed(this.user, this.cmkAuth).guid;
  }

  /**
   * @param {string} password
   * @param {string} email
   * @param {number} threshold
   */
  async signUp(password, email, threshold) {
    try {
      if (this.cmk === null)
        this.generateCvk();

      const cvk = C25519Key.generate();

      const flowCmk = this._getCmkFlow();
      const flowCvk = this._getCvkFlow();
      const vendorCln = this._getVendorClient();
      const ruleCln = new RuleClientSet(this.cmkUrls, this.vuid);
      const keyCln = new KeyClientSet(this.cmkUrls);

      // add vendor pub key
      const { pubKey } = await vendorCln.configuration();

      const vendorPubStore = new KeyStore(pubKey);
      await keyCln.setOrUpdate(vendorPubStore);

      //register vendor account
      const orkIds = flowCvk.clients.map(itm => itm.clientGuid);
      const vuidAuth = AESKey.seed(cvk.toArray()).derive(vendorCln.guid.toArray());
      const [vendorToken, signatures] = await vendorCln.signup(this.vuid, vuidAuth, orkIds);

      // register cmk
      await flowCmk.signUp(password, email, threshold, this.cmk);

      // register cvk
      await flowCvk.signUp(this.cmkAuth, threshold, vendorPubStore.keyId, signatures, cvk);
      
      // allow vendor to partial decrypt
      const tokenTag = Num64.seed("token");
      const allowTokenToVendor = Rule.allow(this.vuid, tokenTag, vendorPubStore);

      await ruleCln.setOrUpdate(allowTokenToVendor);

      //user encrypt vendor token
      const hashToken = Hash.shaBuffer(vendorToken.toArray());
      const cipher = Cipher.encrypt(hashToken, tokenTag, cvk);

      //test dauth and dcrypt
      const vuidAuthTag = await this.logIn(password);
      await vendorCln.signin(this.vuid, vuidAuthTag);
      
      const dcryptOk = await vendorCln.testCipher(this.vuid, vendorToken, cipher);
      if (!dcryptOk || vuidAuth.toString() !== vuidAuthTag.toString())
        return Promise.reject(new Error("Error in the verification workflow"));

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

    if (this.vuid === null)
      throw new Error("vuid must not be empty");

    if (this._cvkFlow === undefined)
      this._cvkFlow = new DCryptFlow(this.cvkUrls, this.vuid);

    return this._cvkFlow;
  }
}
