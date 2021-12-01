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
import DAuthCmkJwtFlow from "../../src/dauth/DAuthCmkJwtFlow";
import DAuthJwtFlow from "../../src/dauth/DAuthJwtFlow";
import { CP256Key } from "cryptide";
import Guid from "../../src/guid";

var nodes = 3;
var threshold = 3;
var user = new Guid();
var pass = "123456";
var newPass = "1234567";
var email = "tmp@tide.org";

var orkUrls = [...Array(nodes)].map((_, i) => `http://ork${i+1}.local`);
var vendorPub = CP256Key.generate();

// var orkUrls = [...Array(3)].map((_, i) => `https://ork-${i}.azurewebsites.net`);
// var vendorUrl = "https://tidevendor.azurewebsites.net/";

(async () => {
  await signUp();
})();

async function signUp() {
  try {
    var flowCreate = new DAuthJwtFlow(user);
    flowCreate.cmkUrls = orkUrls;
    flowCreate.cvkUrls = orkUrls;
    flowCreate.vendorPub = vendorPub;

    var { auth: auth0 } = await flowCreate.signUp(pass, email, threshold);

    var flowLogin = new DAuthCmkJwtFlow(user);
    flowLogin.homeUrl = orkUrls[0];
    flowLogin.vendorPub = vendorPub;
    flowLogin.cmk = flowCreate.cmk;

    var { auth: auth1 } = await flowLogin.logIn();
    assert.equal(auth0.toString(), auth1.toString());

    await flowCreate.changePass(pass, newPass, threshold);
    var { auth: auth2 } = await flowCreate.logIn(newPass);
    assert.equal(auth0.toString(), auth2.toString());

    console.log(`all good for vuid ${flowCreate.vuid}`);
  } catch (error) {
    console.log(error);
  }
}
