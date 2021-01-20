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

import { AESKey } from "cryptide";
import ClientBase, { urlEncode, fromBase64 } from "./ClientBase";
import Guid from "../guid";
import TranToken from "../TranToken";

export default class DCryptClient extends ClientBase {
  /**
   * @param {string|URL} url
   * @param {string|Guid} user
   */
  constructor(url, user, memory = false) {
    super(url, user, memory);
  }

  /**
   * @param {import("cryptide").C25519Key} cvkPub
   * @param {import("big-integer").BigInteger} cvki
   * @param {AESKey} cvkAuthi
   * @param {Guid} signedKeyId
   * @param {Uint8Array} signature
   * @returns {Promise<{orkid: string, sign: string}>}
   */
  async register(cvkPub, cvki, cvkAuthi, signedKeyId, signature) {
    var body = [ urlEncode(cvkPub.toArray()),
      urlEncode(cvki),
      urlEncode(cvkAuthi.toArray()),
      urlEncode(signature) ];
    
    var resp = await this._put(`/cvk/${this.userGuid}/${signedKeyId}`).send(body);
    if (!resp.ok || !resp.body && !resp.body.success) {
      const error = !resp.ok || !resp.body ? resp.text : resp.body.error;
      return  Promise.reject(new Error(error));
    }
    
    return resp.body.content;
  }

  /** @param { Guid } tranid
   * @param {AESKey} key */
  async getCvk(tranid, key) {
    let token = new TranToken().sign(key, this.userBuffer);
    
    for (let i = 0; i < 2; i++) {
      try {
        const tkn = urlEncode(token.toArray());
        const res = await this._get(`/cvk/${this.userGuid}/${tkn}`).set('tranid', tranid.toString());

        return fromBase64(res.text);
      } catch (error) {
        if (error.status !== 408 || i > 0)
          throw error;

        token = TranToken.from(error.response.text).sign(key, this.userBuffer)
      }
    }
  }

  /** @param {Guid} keyId
   *  @return {Promise<{ token: string; challenge: string}>} */
  async challenge(keyId = null) {
    const pathId = keyId ? "/" + keyId.toString() : "";
    return (await this._get(`/cvk/challenge/${this.userGuid}${pathId}`)).body;
  }

  /**
   * @param {Uint8Array} data
   * @param {Guid} keyId
   * @param {string} token
   * @param {Uint8Array} sign
   */
  async decrypt(data, keyId, token, sign) {
    var cipher = urlEncode(data);
    var tkn = urlEncode(token);
    var sgn = urlEncode(sign);

    var res = await this._get(`/cvk/plaintext/${this.userGuid}/${keyId}/${cipher}/${tkn}/${sgn}`);
    return fromBase64(res.text);
  }

  async confirm() {
    await this._post(`/cvk/${this.userGuid}`);
  }
}
