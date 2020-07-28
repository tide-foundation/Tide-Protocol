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
import KeyClientSet from "../src/dauth/keyClientSet";
import RuleClientSet from "../src/dauth/RuleClientSet";
import Guid from "../src/guid";
import KeyStore from "../src/keyStore";
import Rule from "../src/rule";
import Num64 from "../src/Num64";
import Cipher from "../src/Cipher";

var threshold = 3;
var user = "admin";
var cvkAuth = new AESKey();
var urls = [...Array(threshold)].map((_, i) => "http://localhost:500" + (i + 1));
//var urls = [...Array(threshold)].map((_, i) => `https://raziel-ork-${i + 1}.azurewebsites.net`);

const flow = new DCryptFlow(urls, user, cvkAuth);
const userId = new Guid(flow.clients[0].userBuffer);

const keyCln = new KeyClientSet(urls);
const ruleCln = new RuleClientSet(urls, userId);

(async () => {
  await main();
})();

async function main() {
  try {
    var vendorKey = C25519Key.generate();
    var secret = new AesSherableKey();

    const tag = Num64.from("key");
    const keyStore = new KeyStore(vendorKey.public());
    const rule = Rule.allow(userId, tag, keyStore);

    var cvkPromise = flow.signUp(vendorKey.public(), threshold);
    await Promise.all([cvkPromise, 
      keyCln.setOrUpdate(keyStore),
      ruleCln.setOrUpdate(rule)]);

    var cvk = await cvkPromise;
    var cipher = Cipher.encrypt(secret.toArray(), tag, cvk);

    const asymmetric = Cipher.asymmetric(cipher);
    var plain = await flow.decrypt(asymmetric, vendorKey);
    var secretTag = AesSherableKey.from(plain);

    console.log(secret.toString());
    console.log(secretTag.toString());
  } catch (error) {
    console.log(error);
  }
}
