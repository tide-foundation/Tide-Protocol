/**
 * Babel Starter Kit (https://www.kriasoft.com/babel-starter-kit)
 *
 * Copyright © 2015-2016 Kriasoft, LLC. All rights reserved.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE.txt file in the root directory of this source tree.
 */
import { randomBytes } from 'crypto';
import { Hash } from 'cryptide';

export default class Guid {
    /** @param {Uint8Array} data */
    constructor(data = null) {
        this.buffer = typeof data === 'undefined' || data === null ? randomBytes(16) : data;
    }

    toString() {
        const buff = Buffer.from(this.buffer);
        const segments = [buff.slice(0, 4), buff.slice(4, 6), buff.slice(6, 8), buff.slice(8, 10), buff.slice(10, 16)];

        segments[0].swap32();
        segments[1].swap16();
        segments[2].swap16();

        return segments.map(b => b.toString('hex')).join('-');
    }

    /** @returns {Uint8Array} */
    toArray() {
        return this.buffer;
    }

    inspect() {
        return this.toString();
    }

    /** @param {Uint8Array|string} data */
    static from(data) {
        if (!(typeof data === 'string'))
            return new Guid(data);
        
        const segments = data.split('-').map(slc => Buffer.from(slc, 'hex'));
        segments[0].swap32();
        segments[1].swap16();
        segments[2].swap16();

        return new Guid(Buffer.concat(segments));
    }

    /** @param {Uint8Array|string} data */
    static seed(data) {
        return new Guid(Hash.shaBuffer(data).slice(0, 16));
    }
}
