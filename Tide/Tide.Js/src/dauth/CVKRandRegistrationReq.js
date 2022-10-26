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
import { AESKey,ed25519Point } from "cryptide";
import { CVKRandomShareResponse } from "./CVKRandomResponse";
import  DnsEntry  from "../DnsEnrty";

export default class CVKRandRegistrationReq {
    /**
    * @param { AESKey } cvkiAuth
    * @param { CVKRandomShareResponse[] } shares
    * @param {DnsEntry} entry
    * @param { BigInt } cvki // used only for dns entry signing
    * @param { BigInt } cvk2i // used only for dns entry signing
    */
    constructor(cvkiAuth, shares,entry,cvki,cvk2i) {
        this.cvkiAuth = cvkiAuth;
        this.shares = shares;
        this.entry = entry;
        this.cvki =cvki;
        this.cvk2i=cvk2i;
    }

    toString() { return JSON.stringify(this); }
 
    inspect() { return JSON.stringify(this); }
    
    toJSON() { return {
        cvkiAuth: this.cvkiAuth.toString(),
        shares: this.shares,
        entry : this.entry.toString(),
        cvki : this.cvki.toString(),
        cvk2i :this.cvk2i.toString()
    }}

    /** @param {string|object} data */
    static from(data) {
        if (!data) return null;

        const obj = typeof data === 'string' ? JSON.parse(data) : data;
        if (!obj.cvkiAuth || !obj.shares || !obj.dnsEntry  )
            throw Error(`The JSON is not in the correct format: ${data}`);

        const cvkiAuth = AESKey.from(obj.cvkiAuth);
        const shares = obj.shares.map(CVKRandomShareResponse.from);
        const entry = DnsEntry.from(obj.entry)
        const cvki = BigInt(obj.cvki);
        const cvk2i = BigInt(obj.cvk2i);
        
        return new CVKRandRegistrationReq(cvkiAuth,  shares,entry,cvki,cvk2i);
    }
}
