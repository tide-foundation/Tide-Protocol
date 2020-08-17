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
import IdGenerator from "../IdGenerator";
import Guid from "../Guid";

export default class ClientBase {
  /**
   * @param {string|URL} url
   * @param {string|Guid} user
   */
  constructor(url, user) {
    const baseUrl = typeof url === "string" ? new URL(url) : url;

    this.url = baseUrl.origin + "/api";
    this._clientId = IdGenerator.seed(baseUrl);
    this._userId = typeof user === "string" ? IdGenerator.seed(user) : new IdGenerator(user);
  }

  get clientId() {
    return this._clientId.id;
  }

  get clientGuid() {
    return this._clientId.guid;
  }

  get clientBuffer() {
    return this._clientId.buffer;
  }

  get userId() {
    return this._userId.id;
  }

  get userGuid() {
    return this._userId.guid;
  }

  get userBuffer() {
    return this._userId.buffer;
  }

  get userUrl() {
    return urlEncode(this.userBuffer);
  }

  /** @param {string} path
   *  @protected */
  _get(path) {
    return superagent.get(this.url + path);
  }

  /** @param {string} path
   *  @protected */
  _post(path) {
    return superagent.post(this.url + path);
  }

  /** @param {string} path
   *  @protected */
  _put(path) {
    return superagent.put(this.url + path);
  }
}

/** @param {string} text */
export function fromBase64(text) {
  return Buffer.from(text, "base64");
}

/** @param {string|Uint8Array|bigInt.BigInteger} data */
export function urlEncode(data) {
  return typeof data === "string" || data instanceof Uint8Array ? encodeBase64Url(data) : encodeBase64Url(Buffer.from(data.toArray(256).value));
}

/** @param {string|Uint8Array|Buffer} data */
function encodeBase64Url(data) {
  const text = typeof data === "string" ? data : data instanceof Buffer ? data.toString("base64") : Buffer.from(data).toString("base64");

  return text.replace(/\=/g, "").replace(/\+/g, "-").replace(/\//g, "_");
}
