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
import { CVKRandomShareResponse } from "./CVKRandomResponse";

export default class CVKRandRegistrationReq {
    /**
    * @param { AESKey } cvkiAuth
    * @param { CVKRandomShareResponse[] } shares
    */
    constructor(cvkiAuth, shares) {
        this.cvkiAuth = cvkiAuth;
        this.shares = shares;
    }

    toString() { return JSON.stringify(this); }
 
    inspect() { return JSON.stringify(this); }
    
    toJSON() { return {
        cvkiAuth: this.cvkiAuth.toString(),
        shares: this.shares
    }}

    /** @param {string|object} data */
    static from(data) {
        if (!data) return null;

        const obj = typeof data === 'string' ? JSON.parse(data) : data;
        if (!obj.cvkiAuth || !obj.shares)
            throw Error(`The JSON is not in the correct format: ${data}`);

        const cvkiAuth = AESKey.from(obj.cvkiAuth)
        const shares = obj.shares.map(CVKRandomShareResponse.from);
        
        return new CVKRandRegistrationReq(cvkiAuth,  shares);
    }
}
