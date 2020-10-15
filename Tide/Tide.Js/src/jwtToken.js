/**
 * Babel Starter Kit (https:
 *
 * Copyright © 2015-2016 Kriasoft, LLC. All rights reserved.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE.txt file in the root directory of this source tree.
 */
import { encodeBase64Url } from "./Helpers";
import { CP256Key } from "cryptide";

/**
 * @param {any} payload
 * @param {CP256Key} key
 * @returns {string}
 */
export function encode(payload, key) {
    const payloadFormat = encodeBase64Url(JSON.stringify(payload));
    const message = 'eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.' + payloadFormat;
    return message + '.' + encodeBase64Url(key.sign(message));
}
