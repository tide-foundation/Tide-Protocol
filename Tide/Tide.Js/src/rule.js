/**
 * Babel Starter Kit (https:
 *
 * Copyright © 2015-2016 Kriasoft, LLC. All rights reserved.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE.txt file in the root directory of this source tree.
 */

import Guid from "./guid";

export default class Rule {
    constructor() {
        /**@type {Guid}*/
        this.ruleId = null;
        /**@type {Guid}*/
        this.ownerId = null;
        this.tag = 0;
        /**@type {Guid}*/
        this.keyId = null;
        this.condition = "";
        this.action = "Deny";
    }

    stringify() {
        return JSON.stringify({
            ruleId: this.ruleId.toString(),
            ownerId: this.ownerId.toString(),
            tag: this.tag,
            keyId: this.keyId.toString(),
            condition: this.condition,
            action: this.action
        }); 
    }

    /** @param {{ ruleId: string; ownerId: string; tag: number; keyId: string; condition: string; action: string; }} data */
    static from(data) {
        const rule = new Rule();
        rule.ruleId = Guid.from(data.ruleId);
        rule.ownerId = Guid.from(data.ownerId);
        rule.tag = data.tag;
        rule.keyId = Guid.from(data.keyId);
        rule.condition = data.condition;
        rule.action = data.action;

        return rule;
    }
}