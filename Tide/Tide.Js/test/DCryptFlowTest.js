import { C25519Key, AESKey , ed25519Key, CP256Key} from "cryptide";
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
import DAuthJwtFlow from "../src/dauth/DAuthJwtFlow";

const threshold = 3;
const cmkAuth = AESKey.from("AhATyXYow4qdCw7nFGVFu87JICzd7w9PbzAyp7M4r6PiHS7h0RTUNSP5XmcOVUmsvPKe");
const urls = EnvTest.orkUrls;
var pass = "123456";
var newPass = "1234567";
var email = "tmp@tide.org";
const vendorPub = CP256Key.generate();

describe('DCryptFlow', function () {
    it('should decrypt a cipher', async function () {
        const user = Math.random().toString();
        const userId = IdGenerator.seed(user, cmkAuth).guid;
        var flowCreate = new DAuthJwtFlow(userId);
        flowCreate.cmkUrls = urls;
        flowCreate.cvkUrls = urls;
        flowCreate.vendorPub = vendorPub;

        const account = await flowCreate.signUp2(pass, email, threshold);
        console.log(`[signUp] vuid: ${account.vuid} `);

        const ruleCln = new RuleClientSet(urls, account.vuid);
        const keyCln = new KeyClientSet(urls);
        const vendorKey = ed25519Key.generate();
        const cvk = ed25519Key.generate();
       
        
        const msgs = ["😃The magical realist style and thematic substance of One Hundred Years of Solitude😄",
            "established it as"];
        
        const secrets = [ Buffer.from(msgs[0]), Buffer.from(msgs[1]) ];
        const keyStore = new KeyStore(vendorKey.public());
        const tag = Num64.seed("default");
        const rule = Rule.allow(account.vuid, tag, keyStore.keyId);

        await Promise.all([keyCln.setOrUpdate(keyStore), ruleCln.setOrUpdate(rule)]);
        console.log("1");
       // const ids = await Promise.all(flow.clients.map(cln => cln.getClientBuffer()));
        //const signatures = ids.map((id) => vendorKey.sign(Buffer.concat([id, userId.toArray()])));
        console.log("2");
        
        //const cvk = await flow.signUp(cmkAuth, threshold, keyStore.keyId, signatures);
        const flow = new DCryptFlow(urls, account.vuid);
        const cipher1 = Cipher.encrypt(secrets[0], tag, cvk);
        const cipher2 = Cipher.encrypt(secrets[1], tag, cvk);
        console.log("3");
        let plains = await Promise.all([flow.decrypt(cipher1, vendorKey), flow.decrypt(cipher2, vendorKey)]);
        console.log("4");
        assert.equal(msgs.map(text => text.toString()).join('\n'), plains.map(text => text.toString()).join('\n'));
    });

    it('should decrypt the ciphers at once', async function () {
        const user = Math.random().toString();
        const userId = IdGenerator.seed(user, cmkAuth).guid;
        const flow = new DCryptFlow(urls, userId);
        const ruleCln = new RuleClientSet(urls, userId);
        const keyCln = new KeyClientSet(urls);
        const vendorKey = ed25519Key.generate();
        
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
        const signatures = ids.map((id) => vendorKey.sign(Buffer.concat([id, userId.toArray()])));
        
        const cvk = await flow.signUp(cmkAuth, threshold, keyStore.keyId, signatures);
        const cipher1 = Cipher.encrypt(secrets[0], tag1, cvk);
        const cipher2 = Cipher.encrypt(secrets[1], tag2, cvk);
        const cipher3 = Cipher.encrypt(secrets[2], tag1, cvk);
        
        const plains = await flow.decryptBulk([cipher1, cipher2, cipher3], vendorKey);
            
        assert.equal(msgs.map(text => text.toString()).join('\n'), plains.map(text => text.toString()).join('\n'));
    });
});
