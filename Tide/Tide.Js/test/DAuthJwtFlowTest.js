import assert from "assert";
import DAuthCmkJwtFlow from "../src/dauth/DAuthCmkJwtFlow";
import DAuthJwtFlow from "../src/dauth/DAuthJwtFlow";
import { CP256Key } from "cryptide";
import Guid from "../src/guid";
import EnvTest from "./EnvTest";

var threshold = 3;
var user = Guid.seed(Math.random().toString());
var pass = "123456";
var newPass = "1234567";
var email = "tmp@tide.org";

var orkUrls = EnvTest.orkUrls;
var vendorPub = CP256Key.generate();

describe('DAuthJwtFlow', function () {
    it('should register and change password', async function () {
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
});
