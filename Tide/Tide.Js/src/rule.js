/**
 * Babel Starter Kit (https:
 *
 * Copyright © 2015-2016 Kriasoft, LLC. All rights reserved.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE.txt file in the root directory of this source tree.
 */

import Guid from "./Guid";
import Num64 from "./Num64";
import KeyStore from "./keyStore";

export default class Rule {
  constructor() {
    /**@type {Guid}*/
    this.ruleId = null;
    /**@type {Guid}*/
    this.ownerId = null;

    this.tag = new Num64();
    /**@type {Guid}*/
    this.keyId = null;
    this.condition = "";
    this.action = "Deny";
  }

  stringify() {
    return JSON.stringify({
      ruleId: this.ruleId.toString(),
      ownerId: this.ownerId.toString(),
      tag: this.tag.toString(),
      keyId: this.keyId.toString(),
      condition: this.condition,
      action: this.action,
    });
  }

  /**
   * @param {Guid} ownerId
   * @param {Num64} tag
   * @param {KeyStore} key
   */
  static allow(ownerId, tag, key) {
    return defaultRule(ownerId, tag, key, "allow");
  }

  /**
   * @param {Guid} ownerId
   * @param {Num64} tag
   * @param {KeyStore} key
   */
  static deny(ownerId, tag, key) {
    return defaultRule(ownerId, tag, key, "deny");
  }

  /** @param {{ ruleId: string; ownerId: string; tag: number; keyId: string; condition: string; action: string; }} data */
  static from(data) {
    const rule = new Rule();
    rule.ruleId = Guid.from(data.ruleId);
    rule.ownerId = Guid.from(data.ownerId);
    rule.tag = new Num64(data.tag);
    rule.keyId = Guid.from(data.keyId);
    rule.condition = data.condition;
    rule.action = data.action;

    return rule;
  }
}

/**
 * @param {Guid} ownerId
 * @param {Num64} tag
 * @param {KeyStore} key
 * @param {'allow' | 'deny'} action
 */
function defaultRule(ownerId, tag, key, action) {
  const rl = new Rule();

  rl.ruleId = new Guid();
  rl.ownerId = ownerId;
  rl.tag = tag;
  rl.keyId = key.keyId;
  rl.condition = "true";
  rl.action = action;

  return rl;
}
