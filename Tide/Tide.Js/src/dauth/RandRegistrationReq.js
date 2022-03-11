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
import { AESKey } from "cryptide";
import RandomResponse from "./RandomResponse";

export default class RandRegistrationReq {
    /**
    * @param { AESKey } prismAuth
    * @param { string } email
    * @param { RandomResponse[] } shares
    */
    constructor(prismAuth, email, shares) {
        this.prismAuth = prismAuth;
        this.email = email;
        this.shares = shares;
    }

    toString() { return JSON.stringify(this); }
 
    inspect() { return JSON.stringify(this); }
    
    toJSON() { return {
        prismAuth: this.prismAuth.toString(),
        email: this.email,
        shares: this.shares
    }}

    /** @param {string|object} data */
    static from(data) {
        if (!data) return null;

        const obj = typeof data === 'string' ? JSON.parse(data) : data;
        if (!obj.prismAuth || !obj.email || !obj.shares)
            throw Error(`The JSON is not in the correct format: ${data}`);

        const prismAuth = AESKey.from(obj.prismAuth)
        const email = obj.email;
        const shares = obj.shares.map(shr => RandomResponse.from(shr));
        
        return new RandRegistrationReq(prismAuth, email, shares);
    }
}
