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

import DAuthV2Flow from "../../src/dauth/DAuthV2Flow";
import assert from "assert";

var threshold = 3;
var user = "admin";
var pass = "123456";
var newPass = "1234567";
var email = "tmp@tide.org";

var orkUrls = [...Array(threshold)].map((_, i) => "http://localhost:500" + (i + 1));
//var vendorUrl = "http://127.0.0.1:6001";

// var orkUrls = [...Array(20)].map((_, i) => `https://pdork${i + 1}.azurewebsites.net/`);
var vendorUrl = "http://localhost:44384";

(async () => {
  await signUp();
})();

async function signUp() {
  try {
    var flow = new DAuthV2Flow(user);
    flow.cmkUrls = orkUrls;
    flow.cvkUrls = orkUrls;
    flow.vendorUrl = vendorUrl;

    var { auth: auth0 } = await flow.signUp(pass, email, threshold);

    /*
    flow = new DAuthV2Flow(user);
    flow.homeUrl = orkUrls[0];
    flow.vendorUrl = vendorUrl;

    var { auth: auth1 } = await flow.logIn(pass);
    assert.equal(auth0.toString(), auth1.toString());

    await flow.changePass(pass, newPass, threshold);
    var { auth: auth2 } = await flow.logIn(newPass);
    assert.equal(auth0.toString(), auth2.toString());

    console.log(`all good for vuid ${flow.vuid}`);*/
  } catch (error) {
    console.log(error);
  }
}
