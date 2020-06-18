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

import assert from 'assert';
import DAuthFlow from '../src/dauth/dauthFlow';

var threshold = 2;
var user = 'admin';
var pass = "123456";
var urls = [...Array(threshold)].map((_, i) => 'http://127.0.0.1:500' + (i + 1));
//var urls = [...Array(threshold)].map((_, i) => `https://raziel-ork-${i + 1}.azurewebsites.net`);
var flow = new DAuthFlow(urls, user);
(async () => {
    await main();
})();

async function main() {
    try {
        var key = await flow.signUp(pass, threshold);
        var keyTag = await flow.logIn(pass);

        assert.equal(keyTag.toString(), key.toString());
        console.log("wuju!!!!")
    } catch (error) {
        console.log(error);
    }
}
 