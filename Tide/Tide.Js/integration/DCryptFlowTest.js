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
import KeyStore from "../src/keyStore";
import Rule from "../src/rule";
import Num64 from "../src/Num64";
import Cipher from "../src/Cipher";
import IdGenerator from "../src/IdGenerator";

var threshold = 3;
var user = "admin";
var cmkAuth = AESKey.from(
  "AhATyXYow4qdCw7nFGVFu87JICzd7w9PbzAyp7M4r6PiHS7h0RTUNSP5XmcOVUmsvPKe"
);
var urls = [...Array(threshold)].map(
  (_, i) => "http://localhost:500" + (i + 1)
);
//var urls = [...Array(threshold)].map((_, i) => `https://raziel-ork-${i + 1}.azurewebsites.net`);

const userId = IdGenerator.seed(user, cmkAuth).guid;
const flow = new DCryptFlow(urls, userId);
const ruleCln = new RuleClientSet(urls, userId);
const keyCln = new KeyClientSet(urls);

(async () => {
  await main();
})();

async function main() {
  try {
    var vendorKey = C25519Key.fromString(
      "DeXSP3DBdA2mlgkxGEWxq7lIJO6gyd0pUcqM3c71TLAAQbUNuNbGAR7dM9Pc2083PQ8JxydPhGNM8M37eVnOZUI9eL2HtqSbhEo3wYVnflW0xNvlUs8YMaBuK0yydCHK"
    );
    var secret = new AesSherableKey();

    const tag = Num64.seed("key");
    const keyStore = new KeyStore(vendorKey.public());
    const rule = Rule.allow(userId, tag, keyStore);

    var cvkPromise = flow.signUp(cmkAuth, threshold);
    await Promise.all([
      cvkPromise,
      keyCln.setOrUpdate(keyStore),
      ruleCln.setOrUpdate(rule),
    ]);

    var cvk = await cvkPromise;
    var cipher = Cipher.encrypt(secret.toArray(), tag, cvk);

    var plain = await flow.decrypt(cipher, vendorKey);
    var secretTag = AesSherableKey.from(plain);

    console.log(secret.toString());
    console.log(secretTag.toString());
  } catch (error) {
    console.log(error);
  }
}
