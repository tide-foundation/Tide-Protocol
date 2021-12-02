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
// @ts-check
import * as superagent from "superagent";
import IdGenerator from "../IdGenerator";
import Guid from "../guid";
import { C25519Key } from "cryptide";
import { ClientError } from "./Errors";

export default class ClientBase {
  /**
   * @param {string|URL} url
   * @param {string|Guid} user
   */
  constructor(url, user, memory = false) {
    const baseUrl = typeof url === "string" ? new URL(url) : url;

    this.memory = memory;
    this.baseUrl = baseUrl.origin;
    this.url = baseUrl.origin + "/api";
    /** @type {IdGenerator} */
    this._clientId = null;
    this._userId = typeof user === "string" ? IdGenerator.seed(user) : new IdGenerator(user);
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

  async getClientId() {
    if (!this._clientId)
      await this._setClientId();

    return this._clientId.id;
  }

  async getClientBuffer() {
    if (!this._clientId)
      await this._setClientId();

    return this._clientId.buffer;
  }

  async getClientGenerator() {
    if (!this._clientId)
      await this._setClientId();

    return this._clientId;
  }

  /** @returns {Promise<C25519Key>} */
  async getPublic() {
    var res = await this._get("/public");
    return C25519Key.from(res.text);
  }

  async _setClientId() {
    const key = await this.getPublic();
    this._clientId = IdGenerator.seed(key.toArray());
  }

  /** @param {string} path
   *  @protected */
  _get(path) {
    return request('get', path, this);
  }

  /** @param {string} path
   *  @protected */
  _post(path) {
    return request('post', path, this);
  }

  /** @param {string} path
   *  @protected */
  _put(path) {
    return request('put', path, this);
  }
}

/**
 * @returns {superagent.SuperAgentRequest}
 * @param { 'get'|'post'|'put' } type
 * @param {string} path
 * @param {ClientBase} cln
 */
function request(type, path, cln) {
  var req = superagent[type](cln.url + path);
  if (cln.memory)
    req.set('memory', 'true');

  return req;
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

/** 
* @template T
* @param {Promise<T>} req
* @returns {Promise<T|Error>}
*/
export async function processError (req) {
  try {
    return await req;
  }
  catch (err) {
    if (!(err instanceof Error)) {
      return Error(err.toString());
    }
    
    if (err['response'] && err['response']['error']) {
      /** @type {superagent.HTTPError} */
      const error = err['response']['error'];
      return new ClientError(error.text, error.method, error.path, String(error.status))
    }
    
    return err;
  }
}