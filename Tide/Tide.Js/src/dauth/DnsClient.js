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
import DnsEntry from "../DnsEnrty";

export default class DnsClient {
  /**
   * @param {string} url
   * @param {import("../guid").default} user
   */
  constructor(url, user) {
    this.url = url + "/dns/";
    this.guid = user;
  }

  /** @returns {Promise<DnsEntry|null>} */
  async getDns() {
    var resp = await superagent.get(this.url + this.guid);
    if (resp.status === 404)
      return null;
    
    if (!resp.ok || !resp.body && !resp.body.success) {
      const error = !resp.ok || !resp.body ? resp.text : resp.body.error;
      return Promise.reject(new Error(error));
    }

    return DnsEntry.from(resp.body)
  }

  /** @param {import("../DnsEnrty").default} entry */
  async addDns(entry) {
    var resp = await superagent.post(this.url).set('Content-Type', 'application/json').send(entry.toString());

    if (!resp.ok || !resp.body && !resp.body.success) {
      const error = !resp.ok || !resp.body ? resp.text : resp.body.error;
      return  Promise.reject(new Error(error));
    }
    
    return Promise.resolve();
  }
}
