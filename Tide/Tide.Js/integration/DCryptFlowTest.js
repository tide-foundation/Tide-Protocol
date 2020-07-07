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

import { C25519Key, AesSherableKey, AESKey } from "cryptide";
import DCryptFlow from "../src/dauth/DCryptFlow";

var threshold = 3;
var user = "admin";
var cvkAuth = new AESKey();
var urls = [...Array(threshold)].map((_, i) => "http://localhost:500" + (i + 1));
//var urls = [...Array(threshold)].map((_, i) => `https://raziel-ork-${i + 1}.azurewebsites.net`);

var flow = new DCryptFlow(urls, user, cvkAuth);
(async () => {
  await main();
})();

async function main() {
  try {
    var vendorKey = C25519Key.generate();
    var secret = new AesSherableKey();

    var cvk = await flow.signUp(vendorKey.public(), threshold);
    var cipher = cvk.encryptKey(secret);

    var plain = await flow.decrypt(cipher, vendorKey);
    var secretTag = AesSherableKey.from(plain);

    console.log(secret.toString());
    console.log(secretTag.toString());
  } catch (error) {
    console.log(error);
  }
}
