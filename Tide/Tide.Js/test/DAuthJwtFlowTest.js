import assert from "assert";
import DAuthCmkJwtFlow from "../src/dauth/DAuthCmkJwtFlow";
import DAuthJwtFlow from "../src/dauth/DAuthJwtFlow";
import { CP256Key } from "cryptide";
import Guid from "../src/guid";
import EnvTest from "./EnvTest";

var threshold = 3;
var pass = "123456";
var newPass = "1234567";
var email = "tmp@tide.org";
var orkUrls = EnvTest.orkUrls;

describe('DAuthJwtFlow', function () {
    it('should register and change password', async function () {
        var user = new Guid();
        var vendorPub = CP256Key.generate();
        
        var flowCreate = new DAuthJwtFlow(user);
        flowCreate.cmkUrls = orkUrls;
        flowCreate.cvkUrls = orkUrls;
        flowCreate.vendorPub = vendorPub;
    
        var { auth: auth0 } = await flowCreate.signUp(pass, email, threshold);
    
        var flowLogin = new DAuthCmkJwtFlow(user);
        flowLogin.cmkUrls = orkUrls;
        flowLogin.cvkUrls = orkUrls;
        flowLogin.vendorPub = vendorPub;
        flowLogin.cmk = flowCreate.cmk;
    
        var { auth: auth1 } = await flowLogin.logIn();
        assert.equal(auth0.toString(), auth1.toString());
    
        await flowCreate.changePass(pass, newPass, threshold);
        
        var { auth: auth2 } = await flowCreate.logIn(newPass);
        assert.equal(auth0.toString(), auth2.toString());
    });

    it('should register a new CVK to an existing CMK', async function () {
        var user = new Guid();
        var vendorPub1 = CP256Key.generate()
        var vendorPub2 = CP256Key.generate();
        
        const flowCreate = new DAuthJwtFlow(user);
        flowCreate.cmkUrls = orkUrls;
        flowCreate.cvkUrls = orkUrls;
        flowCreate.vendorPub = vendorPub1;
        var { auth: CVK1 } = await flowCreate.signUp(pass, email, threshold);

        const flowLink = new DAuthJwtFlow(user);
        flowLink.cmkUrls = orkUrls;
        flowLink.cvkUrls = orkUrls;
        flowLink.vendorPub = vendorPub2;
        var { auth: CVK2 } = await flowLink.signUpCVK(pass, threshold);

        var flowLogin1 = new DAuthJwtFlow(user);
        flowLogin1.homeUrl = orkUrls[0];
        flowLogin1.vendorPub = vendorPub1;
        var { auth: CVK1v2 } = await flowLogin1.logIn(pass);
        
        var flowLogin2 = new DAuthJwtFlow(user);
        flowLogin2.homeUrl = orkUrls[0];
        flowLogin2.vendorPub = vendorPub2;
        var { auth: CVK2v2 } = await flowLogin2.logIn(pass);

        assert.equal(CVK1.toString(), CVK1v2.toString());
        assert.equal(CVK2.toString(), CVK2v2.toString());
    });
});
