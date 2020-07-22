/**
 * Babel Starter Kit (https://www.kriasoft.com/babel-starter-kit)
 *
 * Copyright © 2015-2016 Kriasoft, LLC. All rights reserved.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE.txt file in the root directory of this source tree.
 */
import { randomBytes } from 'crypto';

export default class Guid {
    /** @param {Buffer} data */
    constructor(data = null) {
        this.buffer = typeof data === 'undefined' || data === null ? randomBytes(16) : data;
    }

    /** @param {Uint8Array|string} data */
    static from(data) {
        return new Guid(typeof data === 'string' ? Buffer.from(data, 'hex') : Buffer.from(data)) ;
    }

    toString() {
        const sec1 = this.buffer.slice(0, 4).toString('hex');
        const sec2 = this.buffer.slice(4, 6).toString('hex');
        const sec3 = this.buffer.slice(6, 8).toString('hex');
        const sec4 = this.buffer.slice(8, 10).toString('hex');
        const sec5 = this.buffer.slice(10, 16).toString('hex');
        return `${sec1}-${sec2}-${sec3}-${sec4}-${sec5}`;
    }

    inspect() {
        return this.toString();
    }
}