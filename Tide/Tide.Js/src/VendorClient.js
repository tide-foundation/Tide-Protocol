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
import { C25519Key } from "cryptide";
import Guid from "./Guid";
import IdGenerator from "./IdGenerator";
import { urlEncode } from "./dauth/ClientBase";
import TranToken from "./TranToken";

export default class VendorClient {

  get id() { return this._idGen.id; }

  get guid() { return this._idGen.guid; }

  /** @param {string|URL} url */
  constructor(url) {
    const baseUrl = typeof url === 'string' ? new URL(url) : url;
    this.url = baseUrl.origin + "/tide/vendor";
    this._idGen = IdGenerator.seed(baseUrl);

  }

  /** @returns {Promise<{ orkUrls: string[]; pubKey: C25519Key; }>}   */
  async configuration() {
    const res = await superagent.get(`${this.url}/configuration`);
    return {
      orkUrls: res.body.orkUrls,
      pubKey: C25519Key.from(res.body.pubKey)
    };
  }

  /** @param {Guid} vuid
   *  @param {import("cryptide").AESKey} auth */
  async signup(vuid, auth) {
    const res = await superagent.put(`${this.url}/account/${vuid}`)
      .set('Content-Type', 'application/json')
      .send(`"${Buffer.from(auth.toArray()).toString('base64')}"`);
    
    return TranToken.from(res.text) 
  }

  /** @param {Guid} vuid
   * @param {TranToken} token */
  async signin(vuid, token) {
    var tkn = urlEncode(token.toArray());

    await superagent.get(`${this.url}/auth/${vuid}/${tkn}`);
  }

  /** @param {Guid} vuid
   * @param {TranToken} token
   * @param {Uint8Array} cipher */
  async testCipher(vuid, token, cipher) {
    var tkn = urlEncode(token.toArray());
    var ciphertext = urlEncode(cipher);

    try {
      await superagent.get(`${this.url}/testcipher/${vuid}/${tkn}/${ciphertext}`)
        .set('Content-Type', 'application/json')
        .send([token.toString(), Buffer.from(cipher).toString('base64')]);

      return true;
    } catch {
      return false;
    }
  }
}
