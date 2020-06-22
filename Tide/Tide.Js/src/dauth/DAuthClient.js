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
import { C25519Point, AESKey } from "cryptide";
import IdGenerator from "../IdGenerator";

export default class DAuthClient {
  /**
   * @param {string} url
   * @param {string} user
   */
  constructor(url, user) {
    this.url = url + "/api";
    this.user = user;
    this._clientId = new IdGenerator(new URL(url).host);
    this._userId = new IdGenerator(user);
  }

  get clientId() {
    return this._clientId.id;
  }

  get clientBuffer() {
    return this._clientId.buffer;
  }

  get userId() {
    return this._userId.id;
  }

  get userBuffer() {
    return this._userId.buffer;
  }

  /** @param {C25519Point} pass */
  async GetShare(pass) {
    var res = await superagent.get(
      `${this.url}/dauth/${encodeBase64Url(
        this.userBuffer
      )}/share/${encodeBase64Url(pass.toArray())}`
    );
    return C25519Point.from(Buffer.from(res.text, "base64"));
  }

  /**
   * @param {Uint8Array} ticks
   * @param {Uint8Array} sign
   */
  async signIn(ticks, sign) {
    var usr = encodeBase64Url(this.userBuffer);
    var tck = encodeBase64Url(ticks);
    var sgn = encodeBase64Url(sign);

    var res = await superagent.get(
      `${this.url}/dauth/${usr}/signin/${tck}/${sgn}`
    );
    return Buffer.from(res.text, "base64");
  }

  /**
   * @param {bigInt.BigInteger} authShare
   * @param {bigInt.BigInteger} keyShare
   * @param {AESKey} secret
   * @param {AESKey} cmkAuth
   * @param {string} email
   */
  async signUp(authShare, keyShare, secret, cmkAuth, email) {
    var user = encodeBase64Url(this.userBuffer);
    var auth = encodeFromBig(authShare);
    var key = encodeFromBig(keyShare);
    var sec = encodeBase64Url(secret.toString());
    var cmk = encodeBase64Url(cmkAuth.toString());
    var mail = encodeURIComponent(email);

    return (
      await superagent.post(
        `${this.url}/dauth/${user}/signup/${auth}/${key}/${sec}/${cmk}/${mail}`
      )
    ).body;
  }

  /**
   * @param {bigInt.BigInteger} authShare
   * @param {AESKey} secret
   * @param {Uint8Array} ticks
   * @param {Uint8Array} sign
   */
  async changePass(authShare, secret, ticks, sign, withCmk = false) {
    var user = encodeBase64Url(this.userBuffer);
    var auth = encodeFromBig(authShare);
    var sec = encodeBase64Url(secret.toString());
    var tck = encodeBase64Url(ticks);
    var sgn = encodeBase64Url(sign);

    await superagent.post(`${this.url}/dauth/${user}/pass/${auth}/${sec}/${tck}/${sgn}?withCmk=${withCmk}`);
  }
}

/** @param {string|Uint8Array} input */
function encodeBase64Url(input) {
  const text =
    typeof input === "string" ? input : Buffer.from(input).toString("base64");
  return text.replace(/\=/g, "").replace(/\+/g, "-").replace(/\//g, "_");
}

/** @param {bigInt.BigInteger} number */
function encodeFromBig(number) {
  return encodeBase64Url(Buffer.from(number.toArray(256).value));
}
