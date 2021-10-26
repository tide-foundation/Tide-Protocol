/**
 * Babel Starter Kit (https:
 *
 * Copyright © 2015-2016 Kriasoft, LLC. All rights reserved.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE.txt file in the root directory of this source tree.
 */

import { Hash } from "cryptide";
import BN from 'bn.js';

export default class Num64 {
    get isZero() { return this.num.isZero(); }
    
    /** @param {number|BN} num */
    constructor(num=0) {
        this.num = getBN(num);
    }

    /** @returns {Uint8Array} */
    toArray() {
        const array = this.num.toArray('le');
        const buffer =  Buffer.alloc(8);
        buffer.set(array, 0);
        
        return buffer;
    }

    /** @param {number|Num64} number */
    add(number) { return new Num64(this.num.add(getBN(number))); }

    /** @param {number|Num64} number */
    mul(number) { return new Num64(this.num.mul(getBN(number))); }

    /** @param {number|Num64} number */
    sub(number) { return new Num64(this.num.sub(getBN(number))); }

    /** @param {number|Num64} number */
    div(number) { return new Num64(this.num.div(getBN(number))); }

    toString() { return this.num.toString(); }

    toJSON() { return this.toString(); }

    inspect() { return this.toString(); }

    /** @param {string|Uint8Array} data */
    static from(data) {
        return new Num64(typeof data === 'string'
            ? new BN(data) : new BN(data, 10, 'le'));
    }

    /** @param {string|Uint8Array} data */
    static seed(data) {
        return Num64.from(Hash.shaBuffer(data).slice(0, 8));
    }
}

/** @param {number|Num64|BN} num
 * @returns {BN} */
function getBN(num) {
    return typeof num === 'number' ? new BN(num)
        : num instanceof Num64 ? num.num
        : num;
}