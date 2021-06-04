import DAuthV2Flow from "../src/dauth/DAuthV2Flow";
import EnvTest from "./EnvTest";
import assert from 'assert';

const threshold = 3;
const user = Math.random().toString();
const pass = "123456";
const newPass = "1234567";
const email = "tmp@tide.org";

const orkUrls = EnvTest.orkUrls;
const vendorUrl = EnvTest.vendorUrl;

describe('DAuthV2Flow', function () {
    it('should decrypt a cipher', async function () {
        var flow = new DAuthV2Flow(user);
        flow.cmkUrls = orkUrls;
        flow.cvkUrls = orkUrls;
        flow.vendorUrl = vendorUrl;

        var { auth: auth0 } = await flow.signUp(pass, email, threshold);

        flow = new DAuthV2Flow(user);
        flow.homeUrl = orkUrls[0];
        flow.vendorUrl = vendorUrl;

        var { auth: auth1 } = await flow.logIn(pass);
        assert.equal(auth0.toString(), auth1.toString());

        await flow.changePass(pass, newPass, threshold);
        var { auth: auth2 } = await flow.logIn(newPass);
        assert.equal(auth0.toString(), auth2.toString());
    });
});
