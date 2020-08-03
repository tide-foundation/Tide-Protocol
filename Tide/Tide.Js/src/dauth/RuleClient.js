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
import Guid from "../Guid";
import Rule from "../rule";

export default class RuleClient {
  /**
   * @param {string} url
   * @param {Guid} user
   */
  constructor(url, user) {
    this.url = url + "/api/rule";
    this.guid = user;
  }

  /** @param {Guid} ruleId */
  async getById(ruleId) {
    const res = await superagent.get(this.url + '/' + ruleId.toString());
    return Rule.from(res.body);
  }

  /** @returns { Promise<Rule[]> } */
  async getSet() {
    const res = await superagent.get(this.url + '/user/' + this.guid.toString());
    return res.body.map(r => Rule.from(r));
  }

  /** @param {Rule} rule */
  async setOrUpdate(rule) {
    await superagent.post(this.url).set('Content-Type', 'application/json').send(rule.stringify());
  }
}

