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

import BigInt from 'big-integer';
import { SecretShare, C25519Point, AESKey } from 'cryptide';

export default class DAuthShare {
    /**
     * @param {bigInt.BigInteger} id
     * @param {bigInt.BigInteger} share
     */
    constructor(id, share) {
        this.id = id;
        this.share = share;
    }

    /** @param {string|Uint8Array} data */
    static from(data) {
        var buffer = typeof data === "string" ? Buffer.from(data, "base64") : Buffer.from(data);
        var id = BigInt.fromArray(Array.from(buffer.slice(0, 16)), 256, false);
        var share = BigInt.fromArray(Array.from(buffer.slice(16)), 256, false);
        
        return new DAuthShare(id, share);
    }
}

