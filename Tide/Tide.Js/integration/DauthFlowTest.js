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

import assert from "assert";
import DAuthFlow from "../src/dauth/dauthFlow";

var threshold = 3;
var user = "admin";
var pass = "123456";
var newPass = "1234567";
var urls = [...Array(threshold)].map(
  (_, i) => "http://localhost:500" + (i + 1)
);
var mail = "tmp@tide.org";

//var urls = [...Array(threshold)].map((_, i) => `https://raziel-ork-${i + 1}.azurewebsites.net`);
var flow = new DAuthFlow(urls, user);
(async () => {
  await main();
})();

async function main() {
  try {
    var key = await flow.signUp(pass, mail, threshold);
    var keyTag1 = await flow.logIn(pass);
    assert.equal(key.toString(), keyTag1.toString());

    await flow.changePass(pass, newPass, threshold);
    var keyTag2 = await flow.logIn(newPass);
    assert.equal(key.toString(), keyTag2.toString());
  } catch (error) {
    console.log(error);
  }
}


