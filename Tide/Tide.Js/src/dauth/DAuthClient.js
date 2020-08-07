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
   * @param {string|URL} url
   * @param {string} user
   */
  constructor(url, user) {
    super(url, user);
  }

  /** @param {C25519Point} pass */
  async GetShare(pass) {
    var res = await this._get(`/cmk/prism/${this.userGuid}/${urlEncode(pass.toArray())}`);
    return C25519Point.from(fromBase64(res.text));
  }

  /**
   * @param {Num64} ticks
   * @param {Uint8Array} sign
   */
  async signIn(ticks, sign) {
    var sgn = urlEncode(sign);

    var res = await this._get(`/cmk/auth/${this.userGuid}/${ticks}/${sgn}`);
    return fromBase64(res.text);
  }

  /**
   * @param {bigInt.BigInteger} prismi
   * @param {bigInt.BigInteger} cmki
   * @param {AESKey} prismAuthi
   * @param {AESKey} cmkAuthi
   * @param {string} email
   */
  async signUp(prismi, cmki, prismAuthi, cmkAuthi, email) {
    var user = this.userGuid;
    var prism = urlEncode(prismi);
    var cmk = urlEncode(cmki);
    var prismAuth = urlEncode(prismAuthi.toString());
    var cmkAuth = urlEncode(cmkAuthi.toString());
    var mail = encodeURIComponent(email);

    return (await this._put(`/cmk/${user}/${prism}/${cmk}/${prismAuth}/${cmkAuth}/${mail}`)).body;
  }

  /**
   * @param {bigInt.BigInteger} prismi
   * @param {AESKey} prismAuthi
   * @param {Num64} ticks
   * @param {Uint8Array} sign
   */
  async changePass(prismi, prismAuthi, ticks, sign, withCmk = false) {
    var prism = urlEncode(prismi);
    var prismAuth = urlEncode(prismAuthi.toString());
    var sgn = urlEncode(sign);

    await this._post(`/cmk/prism/${this.userGuid}/${prism}/${prismAuth}/${ticks}/${sgn}?withCmk=${withCmk}`);
  }
  
  async Recover() {
    await this._get(`/cmk/mail/${this.userGuid}`);
  }
}
