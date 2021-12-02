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

import Guid from "../guid";
import Rule from "../rule";
import RuleClient from "./RuleClient";
import SetClient from "./SetClient";

export default class RuleClientSet {
  /**
   * @param {string[]} urls
   * @param {Guid} user
   */
  constructor(urls, user) {
    this.clients = new SetClient(urls.map((url) => new RuleClient(url, user)));
    this.guid = user;
  }

  /** 
   * @param {Guid} ruleId
   * @param {Array<string>} indices
   */
  async getById(ruleId, indices = null) {
    if (!indices) indices = this.clients.keys;
    const resp = await this.clients.filter(indices, cln => cln.getById(ruleId));

    return resp.values;
  }

  /** @param {Array<string>} indices */
   async getSet(indices = null) {
    if (!indices) indices = this.clients.keys;
    const resp = await this.clients.filter(indices, cln => cln.getSet());

    return resp.values;
  }

  /**
   * @param {Rule} rule
   * @param {Array<string>} indices
   */
  async setOrUpdate(rule, indices = null) {
    if (!indices) indices = this.clients.keys;
    await this.clients.filter(indices, cln => cln.setOrUpdate(rule));
  }
}
