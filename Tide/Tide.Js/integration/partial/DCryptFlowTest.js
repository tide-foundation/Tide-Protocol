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
import DCryptFlow from "../../src/dauth/DCryptFlow";
import KeyClientSet from "../../src/dauth/keyClientSet";
import RuleClientSet from "../../src/dauth/RuleClientSet";
import KeyStore from "../../src/keyStore";
import Rule from "../../src/rule";
import Num64 from "../../src/Num64";
import Cipher from "../../src/Cipher";
import IdGenerator from "../../src/IdGenerator";

var threshold = 3;
var user = Math.random().toString();
var cmkAuth = AESKey.from("AhATyXYow4qdCw7nFGVFu87JICzd7w9PbzAyp7M4r6PiHS7h0RTUNSP5XmcOVUmsvPKe");
var urls = [...Array(threshold)].map((_, i) => "http://localhost:500" + (i + 1));
//var urls = [...Array(10)].map((_, i) => `https://ork-${i}.azurewebsites.net/`);

const userId = IdGenerator.seed(user, cmkAuth).guid;
const flow = new DCryptFlow(urls, userId);
const ruleCln = new RuleClientSet(urls, userId);
const keyCln = new KeyClientSet(urls);

(async () => {
  await bulkDecryption();
  //await singleDecryption();
})();

async function singleDecryption() {
  try {
    const vendorKey = C25519Key.fromString("DeXSP3DBdA2mlgkxGEWxq7lIJO6gyd0pUcqM3c71TLAAQbUNuNbGAR7dM9Pc2083PQ8JxydPhGNM8M37eVnOZUI9eL2HtqSbhEo3wYVnflW0xNvlUs8YMaBuK0yydCHK");

    const secret1 = Buffer.from("😃The magical realist style and thematic substance of One Hundred Years of Solitude😄");
    const secret2 = Buffer.from("established it as");

    const keyStore = new KeyStore(vendorKey.public());
    const tag = Num64.seed("default");
    const rule = Rule.allow(userId, tag, keyStore.keyId);

    await Promise.all([keyCln.setOrUpdate(keyStore), ruleCln.setOrUpdate(rule) ]);

    const ids = await Promise.all(flow.clients.map(cln => cln.getClientBuffer()));
    const signatures = ids.map((id) => vendorKey.sign(Buffer.concat([id, userId.toArray()]), 'edDSA'));

    const cvk = await flow.signUp(cmkAuth, threshold, keyStore.keyId, signatures);

    const cipher1 = Cipher.encrypt(secret1, tag, cvk);
    const cipher2 = Cipher.encrypt(secret2, tag, cvk);

    let plains = await Promise.all([flow.decrypt(cipher1, vendorKey), flow.decrypt(cipher2, vendorKey)]);
    console.log(plains.map(itm => itm.toString()).join('\n'));
  } catch (error) {
    console.log(error);
  }
}

async function bulkDecryption() {
  try {
    const vendorKey = C25519Key.fromString("DeXSP3DBdA2mlgkxGEWxq7lIJO6gyd0pUcqM3c71TLAAQbUNuNbGAR7dM9Pc2083PQ8JxydPhGNM8M37eVnOZUI9eL2HtqSbhEo3wYVnflW0xNvlUs8YMaBuK0yydCHK");

    const secret1 = Buffer.from("😃The magical realist style and thematic substance of One Hundred Years of Solitude😄");
    const secret2 = Buffer.from("established it as");
    const secret3 = Buffer.from("😁an important representative novel of the literary Latin American Boom of the 1960s and 1970s😆");

    const keyStore = new KeyStore(vendorKey.public());
    const tag1 = Num64.seed("large");
    const tag2 = Num64.seed("short");
    const rule1 = Rule.allow(userId, tag1, keyStore.keyId);
    const rule2 = Rule.allow(userId, tag2, keyStore.keyId);

    await Promise.all([keyCln.setOrUpdate(keyStore), 
      ruleCln.setOrUpdate(rule1), ruleCln.setOrUpdate(rule2)
    ]);

    const ids = await Promise.all(flow.clients.map((cln) => cln.getClientBuffer()));
    const signatures = ids.map((id) => vendorKey.sign(Buffer.concat([id, userId.toArray()]), 'edDSA'));

    const cvk = await flow.signUp(cmkAuth, threshold, keyStore.keyId, signatures);

    const cipher1 = Cipher.encrypt(secret1, tag1, cvk);
    const cipher2 = Cipher.encrypt(secret2, tag2, cvk);
    const cipher3 = Cipher.encrypt(secret3, tag1, cvk);

    const plain = await flow.decryptBulk([cipher1, cipher2, cipher3], vendorKey);

    console.log(plain.map(itm => itm.toString()).join(' \n'));
  } catch (error) {
    console.log(error);
  }
}
