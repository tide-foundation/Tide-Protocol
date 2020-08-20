/**
 * Babel Starter Kit (https://www.kriasoft.com/babel-starter-kit)
 *
 * Copyright © 2015-2016 Kriasoft, LLC. All rights reserved.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE.txt file in the root directory of this source tree.
 */
import { randomBytes } from 'crypto';
import { AESKey } from 'cryptide';
import Num64 from './Num64';

export default class TranToken {
    /** @param {AESKey} key */
    constructor(key = null) {
        this.id = Num64.from(randomBytes(8));
        this.ticks = getTicks();

        this.signature = key == null ? new Uint8Array() 
            : getSignature(key, this.id, this.ticks) ;
    }

    /** @param {AESKey} key
     *  @param {Uint8Array} data */
    sign(key, data = null) {
        this.signature = getSignature(key, this.id, this.ticks, data);
        return this;
    }

    /** @param {AESKey} key
    *  @param {Uint8Array} data */
    check(key, data = null) {
        const signature = getSignature(key, this.id, this.ticks, data);
        if (signature.length != this.signature.length)
            return false;
        
        let isValid = true;
        for (let i = 0; i < signature.length; i++) {
            if (signature[0] !== this.signature[0])
                isValid = false;
        }

        return isValid;
    }
    copy() {
        return TranToken.from(this.toArray())
    }

    toString() {
        return Buffer.from(this.toArray()).toString('base64');
    }

    /** @returns {Uint8Array} */
    toArray() {
        const buffer = Buffer.alloc(32);
        buffer.set(this.id.toArray());
        buffer.set(this.ticks.toArray(), 8);
        buffer.set(this.signature, 16);

        return buffer;
    }

    inspect() {
        return this.toString();
    }

    /** @param {Uint8Array|string} data */
    static from(data) {
        const buffer = typeof data === 'string' ? Buffer.from(data, 'base64') : data
        const token = new TranToken();

        token.id = Num64.from(buffer.slice(0, 8));
        token.ticks = Num64.from(buffer.slice(8, 16));
        token.signature = buffer.slice(16);

        return token;
    }
}

function getTicks() {
    return new Num64(new Date().getTime()).mul(10000)
        .add(Num64.from("621355968000000000"));
}

/**
 * @param {AESKey} key
 * @param {Num64} id
 * @param {Num64} ticks
 * @param {Uint8Array} data
 */
function getSignature(key, id, ticks, data = null) {
    const length = 16 + (data !== null ? data.length : 0)
    const buffer = Buffer.alloc(length);

    buffer.set(id.toArray());
    buffer.set(ticks.toArray(), 8);
    if (data !== null)
        buffer.set(data, 16);

    return key.hash(buffer).slice(0, 16);
}
