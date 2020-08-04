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

export default class VendorClient {
  /** @param {string|URL} url */
  constructor(url) {
    const baseUrl = typeof url === 'string' ? new URL(url) : url;
    this.url = baseUrl.origin + "/vendor";
  }

  /** @returns {Promise<{ orkUrls: string[]; pubKey: C25519Key; }>}   */
  async register() {
    const res = await superagent.get(`${this.url}/register`);
    return {
      orkUrls: res.body.orkUrls,
      pubKey: C25519Key.from(res.body.pubKey)
    };
  }

  /** @param {Guid} vuid
   * @param {Uint8Array} cipher
   * @returns {Promise<Uint8Array>} */
  async testCipher(vuid, cipher) {
    const res = await superagent.post(`${this.url}/cipher/vuid/${vuid}`)
      .set('Content-Type', 'application/json')
      .send(`"${Buffer.from(cipher).toString('base64')}"`);

    return Buffer.from(res.text, 'base64');
  }
}
