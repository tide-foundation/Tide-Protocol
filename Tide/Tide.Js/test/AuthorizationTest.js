import { C25519Key, AESKey } from "cryptide";
import KeyClient from "../src/dauth/keyClient";
import DnsClient from "../src/dauth/DnsClient";
import RuleClient from "../src/dauth/RuleClient";
import DnsEntry from "../src/DnsEnrty";
import DCryptClient from "../src/dauth/DCryptClient";
import KeyStore from "../src/keyStore";
import EnvTest from "./EnvTest";
import Rule from "../src/rule";
import Tags from "../src/tags";
import Guid from "../src/guid";
import TranJwt from "../src/TranJwt"
import assert from "assert";

var orkUrls = EnvTest.orkUrls;

describe('KeyClient', function () {
    it('should authorize when a valid token is provided', async function () {
        const client = new KeyClient(orkUrls[0]);
        
        //setting up a new vendor key
        const prvKey = C25519Key.generate();
        const pubKey = prvKey.public(); 
        const storedKey = new KeyStore(pubKey);

        await client.setOrUpdate(storedKey); 
        
        //updating a vendor key with a token
        const prvKeyNew = C25519Key.generate();
        storedKey.key = prvKeyNew.public();

        const authToken = new TranJwt(storedKey.keyId);
        authToken.sign(prvKey)

        await client.setOrUpdate(storedKey, authToken); 

        //deleting a vendor key with a token
        authToken.sign(prvKeyNew)
        await client.delete(storedKey.keyId, authToken);    
    });

    it('should not authorize when an invalid token is provided', async function () {
        const client = new KeyClient(orkUrls[0]);
        
        //setting up a new vendor key
        const prvKey = C25519Key.generate();
        const pubKey = prvKey.public(); 
        const storedKey = new KeyStore(pubKey);

        await client.setOrUpdate(storedKey);
        
        //updating without a token throws an error
        storedKey.key = C25519Key.generate();
        await assert.rejects(async () => { await client.setOrUpdate(storedKey) }, Error);

        //updating with an expired token throws an error 
        const authToken = new TranJwt(storedKey.keyId);
        authToken.validTo = new Date();
        authToken.sign(prvKey);
        
        await assert.rejects(async () => { await client.delete(storedKey.keyId, authToken) }, Error);

        //updating with an invalid token throws an error 
        const prvKeyNew = C25519Key.generate();
        authToken.validTo = new Date(Date.now() + 1000);
        authToken.sign(prvKeyNew);

        await assert.rejects(async () => { await client.delete(storedKey.keyId, authToken) }, Error);
    });
});

describe('DnsClient', function () {
    it('should authorize when valid token is provided', async function () {
        const user = new Guid();
        const client = new DnsClient(orkUrls[0], user);
        
        //setting up a new dns entry
        const prvKey = C25519Key.generate();
        const pubKey = prvKey.public(); 

        const entry = new DnsEntry();
        entry.id = user;
        entry.public = pubKey
        entry.signatures = []
        entry.orks = [];
        entry.sign(prvKey, 'ecDSA');

        await client.addDns(entry);

        //updating a dns entry
        const prvKeyNew = C25519Key.generate();
        entry.public = prvKeyNew.public();
        entry.sign(prvKeyNew, 'ecDSA')

        const authToken = new TranJwt(user);
        authToken.sign(prvKey)

        await client.addDns(entry, authToken);
    });

    it('should not authorize when an invalid token is provided', async function () {
        const user = new Guid();
        const client = new DnsClient(orkUrls[0], user);
        
        //setting up a new dns entry
        const prvKey = C25519Key.generate();
        const pubKey = prvKey.public(); 

        const entry = new DnsEntry();
        entry.id = user;
        entry.public = pubKey
        entry.signatures = []
        entry.orks = [];
        entry.sign(prvKey, 'ecDSA');

        await client.addDns(entry);

        //
        entry.orks = ['http://ork1.local']

        await assert.rejects(async () => { await client.addDns(entry) }, Error);

        //updating with an expired token throws an error 
        const authToken = new TranJwt(user);
        authToken.validTo = new Date();
        authToken.sign(prvKey);
        
        await assert.rejects(async () => { await client.addDns(entry, authToken) }, Error);

        //updating with an invalid token throws an error 
        const prvKeyNew = C25519Key.generate();
        authToken.validTo = new Date(Date.now() + 1000);
        authToken.sign(prvKeyNew);

        await assert.rejects(async () => { await client.addDns(entry, authToken) }, Error);

    });
});

describe('RuleClient', function () {
    it('should authorize when valid token is provided', async function () {
        const user = new Guid();
        const ruleCln = new RuleClient(orkUrls[0], user);
        const cmkCln = new DCryptClient(orkUrls[0], user);

        //setting up a new CMK
        const cvkPrv = C25519Key.generate();
        const cvkPub = cvkPrv.public(); 
        const cvkStored = new KeyStore(cvkPub);
        const cvkAuth = new AESKey();
        const signature = new Uint8Array();

        await cmkCln.register(cvkPub, cvkPrv.x, cvkAuth, cvkStored.keyId, signature);

        //setting up a new rule
        const rule = Rule.allow(user, Tags.vendor, cvkStored.keyId);
        await ruleCln.setOrUpdate(rule);
        
        //updating a rule with a token
        rule.action = "deny"

        const authToken = new TranJwt(user);
        authToken.sign(cvkPrv)
        
        await ruleCln.setOrUpdate(rule, authToken);

        //deleting a rule with a token
        await ruleCln.delete(rule.ruleId, authToken);
    });

    it('should not authorize when an invalid token is provided', async function () {
        const user = new Guid();
        const ruleCln = new RuleClient(orkUrls[0], user);
        const cmkCln = new DCryptClient(orkUrls[0], user);

        //setting up a new CMK
        const cvkPrv = C25519Key.generate();
        const cvkPub = cvkPrv.public(); 
        const cvkStored = new KeyStore(cvkPub);
        const cvkAuth = new AESKey();
        const signature = new Uint8Array();

        await cmkCln.register(cvkPub, cvkPrv.x, cvkAuth, cvkStored.keyId, signature);

        //setting up a new rule
        const rule = Rule.allow(user, Tags.vendor, cvkStored.keyId);
        await ruleCln.setOrUpdate(rule);

        //updating without a token throws an error
        rule.action = "deny"
        await assert.rejects(async () => { await ruleCln.setOrUpdate(rule) }, Error);

        //updating with an expired token throws an error 
        const authToken = new TranJwt(user);
        authToken.validTo = new Date();
        authToken.sign(cvkPrv);
        
        await assert.rejects(async () => { await ruleCln.setOrUpdate(rule, authToken) }, Error);

        //updating with an invalid token throws an error 
        const cvkPrvNew = C25519Key.generate();
        authToken.validTo = new Date(Date.now() + 10000);
        authToken.sign(cvkPrvNew);

        await assert.rejects(async () => { await ruleCln.delete(rule.ruleId, authToken) }, Error);
    });
});
