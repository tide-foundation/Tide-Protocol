import TideAuthentication from "../../src/export/TideAuthentication";
import request from "superagent";
import { CP256Key, EcKeyFormat, C25519Key, Hash, Utils } from "cryptide";

var orkUrls = [...Array(3)].map((_, i) => `https://dork${i + 1}.azurewebsites.net/`);
var vendorUrl = "https://futureplaces.azurewebsites.net/";

// var orkUrls = [...Array(3)].map((_, i) => "http://localhost:500" + (i + 1));
// var vendorUrl = "http://127.0.0.1:6001";

var auth = new TideAuthentication("VendorId", vendorUrl, orkUrls, "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAANvjzMxmyjGxse3fwkqajZxhf088eQRgS4l9wKsnm+A2+HRLt/4n6lA0cO6pmBqB9Le72HFSQ1s9cjv6HF3O2m");

var user = "admin";
var pass = "123456";
var newPass = "987654321";
var email = "tmp@tide.org";

(async () => {
  await main();
})();

async function main() {
  try {
    test();
    // var signUpResult = await auth.registerJwt(user, pass, email, orkUrls);
    // console.log("Register Success");

    // var loginResult = await auth.loginJwt(user, pass, orkUrls);
    // console.log("Login Success");

    // const serverUrl = "http://172.26.17.60:8080";
    // const hashedReturnUrl = "D7RPSr7foQxZELrOT/a2CutCLer6uipjUBhNvYEPD5cCVokvAeFxLTGZkQbVsvIgZM125t6KJEThyoAPC/0KlA==";

    // console.log(auth.validateReturnUrl(serverUrl, hashedReturnUrl));
    // var userData = {
    //   id: signUpResult.vendorKey.toString(),
    //   cvkPub: signUpResult.vendorKey.public().toString(),
    // };

    // await request.post(`${vendorUrl}/account`).send(userData);

    // var login = await auth.login(user, pass);

    // await tide.changePassword(pass, newPass);

    // var newLogin = await auth.login(user, newPass);

    // console.log("Done login");
  } catch (error) {
    console.log(error);
  }
}

function test() {
  var prvKey = C25519Key.from("AC4wlcDNzlLGRPgne2Lr+3z0yLSWZwfxSmMrRLzAiRQNvjzMxmyjGxse3fwkqajZxhf088eQRgS4l9wKsnm+A2+HRLt/4n6lA0cO6pmBqB9Le72HFSQ1s9cjv6HF3O2m");
  var pubkey = prvKey.public();
  var urls = ["dauth.me"];
  for (let i = 0; i < urls.length; i++) {
    var urlSigned = prvKey.sign(urls[i]);
    if (pubkey.verify(urls[i], urlSigned)) console.log(`${urls[i]} ${Buffer.from(urlSigned).toString("base64")}`);
  }
}
