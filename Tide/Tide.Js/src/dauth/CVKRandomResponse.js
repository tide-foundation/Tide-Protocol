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
    * @param { CVKRandomShareResponse[] } shares
    */
    constructor(cvkPub,  shares) {
        this.cvkPub = cvkPub;
        this.shares = shares;
    }

    toString() { return JSON.stringify(this); }
 
    inspect() { return JSON.stringify(this); }
    
    toJSON() { return {
        cvkPub: Buffer.from(this.cvkPub.toArray()).toString('base64'),
        shares: this.shares
    }}

    /** @param {string|object} data */
    static from(data) {

        const obj = typeof data === 'string' ? JSON.parse(data) : data;
        if (!obj.cvkPub ||  !obj.shares)
            throw Error(`The JSON is not in the correct format: ${data}`);

        const cvkPub = ed25519Point.from(Buffer.from(obj.cvkPub, 'base64'));
        const shares = obj.shares.map(CVKRandomShareResponse.from);

        return new CVKRandomResponse( cvkPub, shares);
    }
}

export class CVKRandomShareResponse {
    /**
    * @param {Guid} id 
    * @param { import("big-integer").BigInteger} cvk 
    */
    constructor(id, cvk) {
        this.id = id
        this.cvk = cvk;
    }

    toString() { return JSON.stringify(this); }
 
    inspect() { return JSON.stringify(this); }
    
    toJSON() { 
        return {
            id: this.id.toString(),
            cvk: BnInput.getArray(this.cvk).toString('base64') ,
        };
    }

    /** @param {string|object} data */
    static from(data) {
        if (!data) return null;

        const obj = typeof data === 'string' ? JSON.parse(data) : data;
        if (!obj.id || !obj.cvk )
            throw Error(`The JSON is not in the correct format: ${data}`);

        const id = Guid.from(obj.id)
        const cvk = BnInput.getBig(Buffer.from(obj.cvk, 'base64'));
     
        
        return new CVKRandomShareResponse(id, cvk);
    }
}