import TideAuthentication from "../../src/export/TideAuthentication";

import { C25519Key } from "cryptide";
import BigInt from "big-integer";

var orkUrls = [...Array(20)].map((_, i) => `https://pdork${i + 1}.azurewebsites.net/`);
//var orkUrls = [...Array(5)].map((_, i) => `https://dork${i + 1}.azurewebsites.net/`);
var vendorUrl = "https://dauth.me/";

// var orkUrls = [...Array(3)].map((_, i) => "http://localhost:500" + (i + 1));
// var vendorUrl = "http://127.0.0.1:6001";

var auth = new TideAuthentication("VendorId", vendorUrl, orkUrls, "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAANvjzMxmyjGxse3fwkqajZxhf088eQRgS4l9wKsnm+A2+HRLt/4n6lA0cO6pmBqB9Le72HFSQ1s9cjv6HF3O2m");

var user = "admin";
user = `User${Math.floor(Math.random() * 10000000 + 1)}`;
var pass = "123456";
var newPass = "987654321";
var email = `${user}@tide.org`;

user = "User8055063";
var cmk = "1142788662753092032256759778964003122853138519425469063003113810094517041476";
(async () => {
  await main();
})();

async function main() {
  try {
    // test();

    // const shuffled = orkUrls.sort(() => 0.5 - Math.random());
    // let selectedOrks = shuffled.slice(0, 7);

    // var signUpResult = await auth.registerJwt(user, pass, email, orkUrls);
    // console.log("Register Success");
    // var loginResult = await auth.loginJwt(user, pass, 123);
    // console.log("Login Success");

    var signInResult = await auth.loginUsingCmkJwt(user, orkUrls, cmk, 123);

    console.log(user);
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
