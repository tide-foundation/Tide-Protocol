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

import { C25519Point, AESKey } from "cryptide";
import ClientBase, { urlEncode, fromBase64 } from "./ClientBase";
import IdGenerator from "../IdGenerator";
import Guid from "../Guid";

export default class DCryptClient extends ClientBase {
  /**
   * @param {string} url
   * @param {string} user
   * @param {AESKey} key
   */
  constructor(url, user, key) {
    super(url, user);
    this._userId = IdGenerator.seed(user, key);
  }

  /**
   * @param {import("cryptide").C25519Key} vendor
   * @param {import("big-integer").BigInteger} cvki
   * @param {AESKey} auth
   */
  async register(vendor, cvki, auth) {
    var body = [ urlEncode(vendor.toArray()),
      urlEncode(cvki),
      urlEncode(auth.toArray()) ];
    
    await this._post(`/dauth/${this.userGuid}/cvk`).send(body);
  }

  /** @param {Guid} keyId
   *  @return {Promise<{ token: string; challenge: string}>} */
  async challenge(keyId = null) {
    const pathId = keyId ? '/' + keyId.toString() : '';
    return (await this._get(`/dauth/${this.userGuid}/challenge${pathId}`)).body;
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
    
    var res = await this._get(`/dauth/${this.userGuid}/decrypt/${keyId}/${cipher}/${tkn}/${sgn}`);
    return fromBase64(res.text);
  }
}
