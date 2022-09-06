import { C25519Key, AESKey } from "cryptide";
import DCryptFlow from "../src/dauth/DCryptFlow";
import KeyClientSet from "../src/dauth/keyClientSet";
import RuleClientSet from "../src/dauth/RuleClientSet";
import KeyStore from "../src/keyStore";
import Rule from "../src/rule";
import Num64 from "../src/Num64";
import Cipher from "../src/Cipher";
import IdGenerator from "../src/IdGenerator";
import EnvTest from "./EnvTest";
import assert from 'assert';

const threshold = 3;
const cmkAuth = AESKey.from("AhATyXYow4qdCw7nFGVFu87JICzd7w9PbzAyp7M4r6PiHS7h0RTUNSP5XmcOVUmsvPKe");
const urls = EnvTest.orkUrls;

describe('DCryptFlow', function () {
    it('should decrypt a cipher', async function () {
        const user = Math.random().toString();
        const userId = IdGenerator.seed(user, cmkAuth).guid;
        const flow = new DCryptFlow(urls, userId);
        const ruleCln = new RuleClientSet(urls, userId);
        const keyCln = new KeyClientSet(urls);
        const vendorKey = C25519Key.generate();
        
        const msgs = ["😃The magical realist style and thematic substance of One Hundred Years of Solitude😄",
            "established it as"];
        
        const secrets = [ Buffer.from(msgs[0]), Buffer.from(msgs[1]) ];
        const keyStore = new KeyStore(vendorKey.public());
        const tag = Num64.seed("default");
        const rule = Rule.allow(userId, tag, keyStore.keyId);

        await Promise.all([keyCln.setOrUpdate(keyStore), ruleCln.setOrUpdate(rule)]);

        const ids = await Promise.all(flow.clients.map(cln => cln.getClientBuffer()));
        const signatures = ids.map((id) => vendorKey.sign(Buffer.concat([id, userId.toArray()]), 'ecDSA'));

        const cvk = await flow.signUp(cmkAuth, threshold, keyStore.keyId, signatures);
        const cipher1 = Cipher.encrypt(secrets[0], tag, cvk);
        const cipher2 = Cipher.encrypt(secrets[1], tag, cvk);

        let plains = await Promise.all([flow.decrypt(cipher1, vendorKey), flow.decrypt(cipher2, vendorKey)]);
        
        assert.equal(msgs.map(text => text.toString()).join('\n'), plains.map(text => text.toString()).join('\n'));
    });

    it('should decrypt the ciphers at once', async function () {
        const user = Math.random().toString();
        const userId = IdGenerator.seed(user, cmkAuth).guid;
        const flow = new DCryptFlow(urls, userId);
        const ruleCln = new RuleClientSet(urls, userId);
        const keyCln = new KeyClientSet(urls);
        const vendorKey = C25519Key.generate();
        
        const msgs = [ "😃The magical realist style and thematic substance of One Hundred Years of Solitude😄",
            "established it as",
            "😁an important representative novel of the literary Latin American Boom of the 1960s and 1970s😆" ];
            
        const secrets = [ Buffer.from(msgs[0]), Buffer.from(msgs[1]), Buffer.from(msgs[2]) ];
        const keyStore = new KeyStore(vendorKey.public());
        const tag1 = Num64.seed("large");
        const tag2 = Num64.seed("short");
        const rule1 = Rule.allow(userId, tag1, keyStore.keyId);
        const rule2 = Rule.allow(userId, tag2, keyStore.keyId);
        
        await Promise.all([keyCln.setOrUpdate(keyStore),
            ruleCln.setOrUpdate(rule1), ruleCln.setOrUpdate(rule2)
        ]);
        
        const ids = await Promise.all(flow.clients.map((cln) => cln.getClientBuffer()));
        const signatures = ids.map((id) => vendorKey.sign(Buffer.concat([id, userId.toArray()]), 'ecDSA'));
        
        const cvk = await flow.signUp(cmkAuth, threshold, keyStore.keyId, signatures);
        const cipher1 = Cipher.encrypt(secrets[0], tag1, cvk);
        const cipher2 = Cipher.encrypt(secrets[1], tag2, cvk);
        const cipher3 = Cipher.encrypt(secrets[2], tag1, cvk);
        
        const plains = await flow.decryptBulk([cipher1, cipher2, cipher3], vendorKey);
            
        assert.equal(msgs.map(text => text.toString()).join('\n'), plains.map(text => text.toString()).join('\n'));
    });
});
