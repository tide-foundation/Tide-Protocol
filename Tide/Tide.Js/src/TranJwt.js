/**
 * Babel Starter Kit (https://www.kriasoft.com/babel-starter-kit)
 *
 * Copyright © 2015-2016 Kriasoft, LLC. All rights reserved.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE.txt file in the root directory of this source tree.
 */
import { randomBytes } from 'crypto';
import Num64 from './Num64';
import Guid from './guid';
import { encodeBase64Url, decodeBase64Url } from "./Helpers";

const _window = 1000 * 60 * 60;

export default class TranJwt {
    /** @param {import("./guid").default } subject */
    constructor(subject) {
        this.payload = {
            jti: Num64.from(randomBytes(8)),
            sub: subject,
            iat: toUnixEpoch(new Date()),
            nbf: toUnixEpoch(new Date()),
            exp: toUnixEpoch(new Date(Date.now() + _window))
        };

        this.head = {
            alg: "TS256",
            typ: "JWT"
        }
        
        this.signature = "";
    }

    get id() { return this.payload.jti; }

    get validTo() { fromUnixEpoch(this.payload.exp); }

    get ValidFrom() { fromUnixEpoch(this.payload.nbf); }

    get IssuedAt() { fromUnixEpoch(this.payload.iat); }

    set validTo(value) { this.payload["exp"] = toUnixEpoch(value); }

    set ValidFrom(value) { this.payload["nbf"] = toUnixEpoch(value); }

    set IssuedAt(value) { this.payload["iat"] = toUnixEpoch(value); }

    encodeHead() { return encodeBase64Url(JSON.stringify(this.head)); }

    encodePayload() { return encodeBase64Url(JSON.stringify(this.payload)); }

    encodeMessage() { return `${this.encodeHead()}.${this.encodePayload()}` }

    /** @param {import("cryptide").C25519Key } key */
    sign(key) { this.signature = encodeBase64Url(key.sign(this.encodeMessage(), 'edDSA')); }

    /** @param {import("cryptide").C25519Key } key */
    verify(key) { return key.verify(this.encodeMessage(), decodeBase64Url(this.signature)); }

    toString() { return `${this.encodeHead()}.${this.encodePayload()}.${this.signature}` }

    inspect() { return this; }

    /** @param {string} jwt */
    static from(jwt) {
        const sections = (jwt ? jwt : "").split(".");
        if (sections.length != 3) throw new Error("Invalid JWT");

        try {
            const jwtTag = new TranJwt(Guid.empty);
            jwtTag.head = JSON.parse(decodeBase64Url(sections[0]).toString());
            jwtTag.payload = JSON.parse(decodeBase64Url(sections[1]).toString());
            jwtTag.signature = sections[2];

            return jwtTag;
        } catch (error) { throw (error instanceof SyntaxError ? new Error("Invalid JWT") : error); }
    }
}

/** @param {Date} date */
function toUnixEpoch(date) { return Math.floor(date.getTime() / 1000); }

/** @param {number} time */
function fromUnixEpoch(time) { return time ? new Date(time * 1000) : null; }