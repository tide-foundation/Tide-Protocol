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

import superagent from "superagent";
import { C25519Key, AESKey } from "cryptide";
import Guid from "./guid";
import IdGenerator from "./IdGenerator";
import { urlEncode } from "./dauth/ClientBase";
import TranToken from "./TranToken";

export default class VendorClient {
  async getId() {
    if (!this._idGen)
      await this._setIdGen();

    return this._idGen.id;
  }

  async getGuid() {
    if (!this._idGen)
      await this._setIdGen();

    return this._idGen.guid;
  }


  /** @param {string|URL} url */
  constructor(url) {
    const baseUrl = typeof url === "string" ? new URL(url) : url;
    this.url = baseUrl.origin + "/tide/vendor";
    this.bearer = "";
    /** @type {IdGenerator} */
    this._idGen = null;
  }

  /** @returns {Promise<{ orkUrls: string[]; pubKey: C25519Key; }>}   */
  async configuration() {
    const res = await superagent.get(`${this.url}/configuration`);
    return {
      orkUrls: res.body.orkUrls,
      pubKey: C25519Key.from(res.body.pubKey),
    };
  }

  /** @param {Guid} vuid
   *  @param {import("cryptide").AESKey} auth
   *  @param {string[]} orks
   *  @returns {Promise<[TranToken, Buffer[]]>}
   * */
  async signup(vuid, auth, orks) {
    const res = await superagent.put(`${this.url}/account/${vuid}`)
      .set('Content-Type', 'application/json')
      .send({ OrkUrls: orks,
        auth: Buffer.from(auth.toArray()).toString('base64') });
    
    return [TranToken.from(res.body.token), 
      res.body.signatures.map(sign => Buffer.from(sign, 'base64'))];
  }

  /**
   * @param {Guid} vuid
   * @param {AESKey} authKey
   */
  async signin(vuid, authKey) {
    const token = new TranToken();
    token.sign(authKey, vuid.toArray());

    var tkn = urlEncode(token.toArray());

    var res = await superagent.get(`${this.url}/auth/${vuid}/${tkn}`);

    this.bearer = res.text;
    return res.text;
  }

  /** @param {Guid} vuid
   * @param {TranToken} token
   * @param {Uint8Array} cipher */
  async testCipher(vuid, token, cipher) {
    var tkn = urlEncode(token.toArray());
    var ciphertext = urlEncode(cipher);

    try {
      await superagent.get(`${this.url}/testcipher/${vuid}/${tkn}/${ciphertext}`).set("Authorization", "Bearer " + this.bearer);

      return true;
    } catch {
      return false;
    }
  }

  async _setIdGen() {
    const { pubKey } = await this.configuration();
    this._idGen = IdGenerator.seed(pubKey.toArray());
  }
}
