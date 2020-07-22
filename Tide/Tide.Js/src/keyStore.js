/**
 * Babel Starter Kit (https:
 *
 * Copyright © 2015-2016 Kriasoft, LLC. All rights reserved.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE.txt file in the root directory of this source tree.
 */

import Guid from "./guid";
import { C25519Key, Hash } from "cryptide";

export default class KeyStore {
    /** @param {C25519Key} key */
    constructor(key = null) {
        /**@type {C25519Key}*/
        this.key = key;

        /**@type {Guid}*/
        this.keyId = null;
        if (key)
            this.keyId = Guid.from(Hash.shaBuffer(key.toArray()).slice(16));
    }

    stringify() {
        return JSON.stringify({
            keyId: this.keyId.toString(),
            key: this.key.toString()
        }); 
    }

    /** @param {{ keyId: string; key: string; }} data */
    static from(data) {
        const rule = new KeyStore();
        rule.keyId = Guid.from(data.keyId);
        rule.key = C25519Key.fromString(data.key);

        return rule;
    }
}