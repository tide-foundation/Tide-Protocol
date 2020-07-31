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
import Num64 from "../Num64";

export default class DAuthClient extends ClientBase {
  /**
   * @param {string} url
   * @param {string} user
   */
  constructor(url, user) {
    super(url, user);
  }

  /** @param {C25519Point} pass */
  async GetShare(pass) {
    var res = await this._get(`/dauth/${this.userGuid}/convert/${urlEncode(pass.toArray())}`);
    return C25519Point.from(fromBase64(res.text));
  }

  /**
   * @param {Num64} ticks
   * @param {Uint8Array} sign
   */
  async signIn(ticks, sign) {
    var sgn = urlEncode(sign);

    var res = await this._get(`/dauth/${this.userGuid}/authenticate/${ticks}/${sgn}`);
    return fromBase64(res.text);
  }

  /**
   * @param {bigInt.BigInteger} authShare
   * @param {bigInt.BigInteger} keyShare
   * @param {AESKey} secret
   * @param {AESKey} cmkAuth
   * @param {string} email
   */
  async signUp(authShare, keyShare, secret, cmkAuth, email) {
    var user = this.userGuid;
    var auth = urlEncode(authShare);
    var key = urlEncode(keyShare);
    var sec = urlEncode(secret.toString());
    var cmk = urlEncode(cmkAuth.toString());
    var mail = encodeURIComponent(email);

    return (await this._post(`/dauth/${user}/signup/${auth}/${key}/${sec}/${cmk}/${mail}`)).body;
  }

  /**
   * @param {bigInt.BigInteger} authShare
   * @param {AESKey} secret
   * @param {Num64} ticks
   * @param {Uint8Array} sign
   */
  async changePass(authShare, secret, ticks, sign, withCmk = false) {
    var auth = urlEncode(authShare);
    var sec = urlEncode(secret.toString());
    var sgn = urlEncode(sign);

    await this._post(`/dauth/${this.userGuid}/pass/${auth}/${sec}/${ticks}/${sgn}?withCmk=${withCmk}`);
  }

  async Recover() {
    await this._get(`/dauth/${this.userGuid}/cmk/`);
  }
}
