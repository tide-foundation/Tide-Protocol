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
import { AESKey, ed25519Point } from "cryptide";
import DnsEntry from "../DnsEnrty";
import TranToken from "../TranToken";
import DnsClient from "./DnsClient";
import { RandomShareResponse } from "./RandomResponse";

export default class ApplyResponseDecrypted {
    /**
    * @param { ed25519Point } gBlurUserCMKi
    * @param { ed25519Point } gBlindR
    * @param { ed25519Point } gCMK
    * @param { TranToken } certTime
    */
    constructor(gBlurUserCMKi, gBlindR, gCMK, certTime) {
       this.gBlurUserCMKi = gBlurUserCMKi;
       this.gBlindR = gBlindR;
       this.gCMK = gCMK;
       this.certTime = certTime;
    }

    /** @param {Buffer} data */
    static from(data) {
        const obj = JSON.parse(data.toString());
        if (!obj.gBlurUserCMKi || !obj.gBlindR || !obj.gCMK || !obj.certTime)
            throw Error(`ApplyResponseDecrypted: The JSON is not in the correct format: ${data}`);

        const gBlurUserCMKi = ed25519Point.from(obj.gBlurUserCMKi);
        const gBlindR = ed25519Point.from(obj.gBlindR);
        const gCMK = ed25519Point.from(obj.gCMK);
        const certTime = TranToken.from(obj.certTime);
        
        return new ApplyResponseDecrypted(gBlurUserCMKi, gBlindR, gCMK, certTime);
    }
}
