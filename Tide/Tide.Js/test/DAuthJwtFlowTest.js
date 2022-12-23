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
    it('should register and change password', async function () { //fix this after implementing change password new logic
        var user = new Guid();
        var vendorPub = CP256Key.generate();
        
        var flowCreate = new DAuthJwtFlow(user);
        flowCreate.cmkUrls = orkUrls;
        flowCreate.cvkUrls = orkUrls;
        flowCreate.vendorPub = vendorPub;
    
        var { cvkPub: auth0 } = await flowCreate.signUp2(pass, email, threshold);
    
        var flowLogin = new DAuthJwtFlow(user);
        flowLogin.homeUrl = orkUrls[0];
        flowLogin.cmkUrls = orkUrls;
        flowLogin.cvkUrls = orkUrls;
        flowLogin.vendorPub = vendorPub;
    
       // await flowCreate.changePass(pass, newPass, threshold);
        
        var { cvkPub: auth2 } = await flowLogin.logIn2(pass); //change to newwPass
        assert.equal(auth0.toString(), auth2.toString());
    });

    it('should register and lgin', async function () {
        var user = new Guid();
        var vendorPub = CP256Key.generate();
        
        var flowCreate = new DAuthJwtFlow(user);
        flowCreate.cmkUrls = orkUrls;
        flowCreate.cvkUrls = orkUrls;
        flowCreate.vendorPub = vendorPub;
    
        var { cvkPub: auth0 } = await flowCreate.signUp2(pass, email, threshold);
    
        var flowLogin = new DAuthJwtFlow(user);
        flowLogin.homeUrl = orkUrls[0];
        flowLogin.cmkUrls = orkUrls;
        flowLogin.cvkUrls = orkUrls;
        flowLogin.vendorPub = vendorPub;
    
        var { cvkPub: auth2 } = await flowLogin.logIn2(pass);
        assert.equal(auth0.toString(), auth2.toString());
    });

    it('should register a new CVK to an existing CMK', async function () {
        var user = new Guid();
        var vendorPub1 = CP256Key.generate();
        var vendorPub2 = CP256Key.generate();
        
        const flowCreate = new DAuthJwtFlow(user);
        flowCreate.cmkUrls = orkUrls;
        flowCreate.cvkUrls = orkUrls;
        flowCreate.vendorPub = vendorPub1;
        var { cvkPub: CVK1 } = await flowCreate.signUp2(pass, email, threshold);

        // const flowLink = new DAuthJwtFlow(user);
        // flowLink.cmkUrls = orkUrls;
        // flowLink.cvkUrls = orkUrls;
        // flowLink.vendorPub = vendorPub2;
        // var { cvkPub: CVK2 } = await flowLink.signUpCVK(pass, threshold);

        var flowLogin1 = new DAuthJwtFlow(user);
        flowLogin1.homeUrl = orkUrls[0];
        flowLogin1.cmkUrls = orkUrls;
        flowLogin1.cvkUrls = orkUrls;
        flowLogin1.vendorPub = vendorPub1;
        var { cvkPub: CVK1v2 } = await flowLogin1.logIn2(pass);
        
        // var flowLogin2 = new DAuthJwtFlow(user);
        // flowLogin2.homeUrl = orkUrls[0];
        // flowLogin2.vendorPub = vendorPub2;
        // var { cvkPub: CVK2v2 } = await flowLogin2.logIn2(pass);

        assert.equal(CVK1.toString(), CVK1v2.toString());
        //assert.equal(CVK2.toString(), CVK2v2.toString());
    });
});
