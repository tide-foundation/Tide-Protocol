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
// @ts-check
import { C25519Point, BnInput } from "cryptide";
import Guid from "../guid";

export default class RandomResponse {
    /**
    * @param {C25519Point} password
    * @param {C25519Point} cmkPub
    * @param { RandomShareResponse[] } shares
    */
    constructor(password, cmkPub, shares) {
        this.password = password;
        this.cmkPub = cmkPub;
        this.shares = shares;
    }

    toString() { return JSON.stringify(this); }
 
    inspect() { return JSON.stringify(this); }
    
    toJSON() { return {
        cmkPub: Buffer.from(this.cmkPub.toArray()).toString('base64'),
        password: Buffer.from(this.password.toArray()).toString('base64'),
        shares: this.shares
    }}

    /** @param {string|object} data */
    static from(data) {
        if (!data) return null;

        const obj = typeof data === 'string' ? JSON.parse(data) : data;
        if (!obj.password || !obj.cmkPub || !obj.shares)
            throw Error(`The JSON is not in the correct format: ${data}`);

        const password = C25519Point.from(Buffer.from(obj.password, 'base64'));
        const cmkPub = C25519Point.from(Buffer.from(obj.cmkPub, 'base64'));
        const shares = obj.shares.map(RandomShareResponse.from);

        return new RandomResponse(password, cmkPub, shares);
    }
}

export class RandomShareResponse {
    /**
    * @param {Guid} id 
    * @param { import("big-integer").BigInteger} cmk 
    * @param { import("big-integer").BigInteger} prism 
    */
    constructor(id, cmk, prism) {
        this.id = id
        this.cmk = cmk;
        this.prism = prism;
    }

    toString() { return JSON.stringify(this); }
 
    inspect() { return JSON.stringify(this); }
    
    toJSON() { 
        return {
            id: this.id.toString(),
            cmk: BnInput.getArray(this.cmk).toString('base64') ,
            prism: BnInput.getArray(this.prism).toString('base64')
        };
    }

    /** @param {string|object} data */
    static from(data) {
        if (!data) return null;

        const obj = typeof data === 'string' ? JSON.parse(data) : data;
        if (!obj.id || !obj.cmk || !obj.prism)
            throw Error(`The JSON is not in the correct format: ${data}`);

        const id = Guid.from(obj.id)
        const cmk = BnInput.getBig(Buffer.from(obj.cmk, 'base64'));
        const prism = BnInput.getBig(Buffer.from(obj.prism, 'base64'));
        
        return new RandomShareResponse(id, cmk, prism);
    }
}