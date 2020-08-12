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

import { AESKey, C25519Cipher, Hash } from "cryptide";
import DCryptFlow from "../src/dauth/DCryptFlow";
import KeyClientSet from "../src/dauth/keyClientSet";
import RuleClientSet from "../src/dauth/RuleClientSet";
import KeyStore from "../src/keyStore";
import Rule from "../src/rule";
import Num64 from "../src/Num64";
import Cipher from "../src/Cipher";
import IdGenerator from "../src/IdGenerator";
import VendorClient from "../src/VendorClient";

var threshold = 3;
var user = "admin";
var cvkAuth = new AESKey();
var vendorClient = new VendorClient("http://127.0.0.1:6001");

(async () => {
  await main();
})();

async function main() {
  const { orkUrls, pubKey } = await vendorClient.configuration();

  const vuid = IdGenerator.seed(user, cvkAuth).guid;
  const flow = new DCryptFlow(orkUrls, vuid);
  const ruleCln = new RuleClientSet(orkUrls, vuid);
  const keyCln = new KeyClientSet(orkUrls);

  //user register cvk
  var cvk = await flow.signUp(cvkAuth, threshold);

  //register vendor account
  const vuidAuth = AESKey.seed(cvk.toArray()).derive(vendorClient.guid.toArray());
  const vendorToken = await vendorClient.signup(vuid, vuidAuth);

  //user register rule
  const tokenTag = Num64.seed("token");
  const vendorPubStore = new KeyStore(pubKey);
  const allowTokenToVendor = Rule.allow(vuid, tokenTag, vendorPubStore);

  await Promise.all([keyCln.setOrUpdate(vendorPubStore),
    ruleCln.setOrUpdate(allowTokenToVendor)]);

  //user encrypt token
  const hashToken = Hash.shaBuffer(vendorToken.toArray());
  const cipher = Cipher.encrypt(hashToken, tokenTag, cvk);

  //user send cipher to vendor
  const isOk = await vendorClient.testCipher(vuid, vendorToken, cipher);
  console.log(isOk);
}
