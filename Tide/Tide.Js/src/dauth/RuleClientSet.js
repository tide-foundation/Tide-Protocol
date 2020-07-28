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

import Guid from "../guid";
import Rule from "../rule";
import RuleClient from "./RuleClient";

export default class RuleClientSet {
  /**
   * @param {string[]} urls
   * @param {Guid} user
   */
  constructor(urls, user) {
    this.clients = urls.map(url => new RuleClient(url, user));
    this.guid = user;
  }

  /** @param {Guid} ruleId */
  async getById(ruleId) {
    return await Promise.all(this.clients.map(cln => cln.getById(ruleId)));
  }

  async getSet() {
    return await Promise.all(this.clients.map(cln => cln.getSet()));
  }

  /** @param {Rule} rule */
  async setOrUpdate(rule) {
    return await Promise.all(this.clients.map(cln => cln.setOrUpdate(rule)));
  }
}

