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
import { ed25519Point, BnInput } from "cryptide";
import Guid from "../guid";

export default class CVKRandomResponse {
    /**
    * @param {ed25519Point} cvkPub
    * @param {ed25519Point} cvk2Pub
    * @param { CVKRandomShareResponse[] } shares
    */
    constructor(cvkPub, cvk2Pub, shares) {
        this.cvkPub = cvkPub;
        this.shares = shares;
        this.cvk2Pub = cvk2Pub;
    }

    toString() { return JSON.stringify(this); }
 
    inspect() { return JSON.stringify(this); }
    
    toJSON() { return {
        cvkPub: Buffer.from(this.cvkPub.toArray()).toString('base64'),
        cvk2Pub: Buffer.from(this.cvk2Pub.toArray()).toString('base64'),
        shares: this.shares
    }}

    /** @param {string|object} data */
    static from(data) {

        const obj = typeof data === 'string' ? JSON.parse(data) : data;
        if (!obj.cvkPub || !obj.cvk2Pub || !obj.shares)
            throw Error(`The JSON is not in the correct format: ${data}`);

        const cvkPub = ed25519Point.from(Buffer.from(obj.cvkPub, 'base64'));
        const cvk2Pub = ed25519Point.from(Buffer.from(obj.cvk2Pub, 'base64'));
        const shares = obj.shares.map(CVKRandomShareResponse.from);

        return new CVKRandomResponse( cvkPub, cvk2Pub, shares);
    }
}

export class CVKRandomShareResponse {
    /**
    * @param {Guid} id 
    * @param { import("big-integer").BigInteger} cvk 
    *  @param { import("big-integer").BigInteger} cvk2 
    */
    constructor(id, cvk, cvk2) {
        this.id = id
        this.cvk = cvk;
        this.cvk2 =cvk2;
    }

    toString() { return JSON.stringify(this); }
 
    inspect() { return JSON.stringify(this); }
    
    toJSON() { 
        return {
            id: this.id.toString(),
            cvk: BnInput.getArray(this.cvk).toString('base64') ,
            cvk2: BnInput.getArray(this.cvk2).toString('base64') ,
        };
    }

    /** @param {string|object} data */
    static from(data) {
        if (!data) return null;

        const obj = typeof data === 'string' ? JSON.parse(data) : data;
        if (!obj.id || !obj.cvk || !obj.cvk2)
            throw Error(`The JSON is not in the correct format: ${data}`);

        const id = Guid.from(obj.id)
        const cvk = BnInput.getBig(Buffer.from(obj.cvk, 'base64'));
        const cvk2 = BnInput.getBig(Buffer.from(obj.cvk2, 'base64'));
     
        
        return new CVKRandomShareResponse(id, cvk,cvk2);
    }
}