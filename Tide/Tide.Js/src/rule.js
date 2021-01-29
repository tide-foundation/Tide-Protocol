/**
 * Babel Starter Kit (https:
 *
 * Copyright © 2015-2016 Kriasoft, LLC. All rights reserved.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE.txt file in the root directory of this source tree.
 */

import Guid from "./guid";
import Num64 from "./Num64";

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
    
  /**@type {'allow' | 'deny'}*/
    this.action = "deny";
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
   * @param {Guid} keyId
   */
  static allow(ownerId, tag, keyId, condition = "true") {
    return defaultRule(ownerId, tag, keyId, "allow", condition);
  }

  /**
   * @param {Guid} ownerId
   * @param {Num64} tag
   * @param {Guid} keyId
   */
  static deny(ownerId, tag, keyId, condition = "true") {
    return defaultRule(ownerId, tag, keyId, "deny", condition);
  }

  /** @param {{ ruleId: string; ownerId: string; tag: number; keyId: string; condition: string; action: 'allow'|'deny'; }} data */
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
 * @param {Guid} keyId
 * @param {'allow' | 'deny'} action
 * @param {string} condition
 */
function defaultRule(ownerId, tag, keyId, action, condition) {
  const rl = new Rule();

  rl.ruleId = new Guid();
  rl.ownerId = ownerId;
  rl.tag = tag;
  rl.keyId = keyId;
  rl.condition = condition;
  rl.action = action;

  return rl;
}
