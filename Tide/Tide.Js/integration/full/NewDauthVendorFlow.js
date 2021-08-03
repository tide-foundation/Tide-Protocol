import TideAuthentication from "../../src/export/TideAuthentication";
import request from "superagent";

// var orkUrls = [...Array(3)].map((_, i) => `https://ork-${i}.azurewebsites.net/`);
// var vendorUrl = "https://tidevendor.azurewebsites.net/";

var orkUrls = [...Array(3)].map((_, i) => "http://localhost:500" + (i + 1));
var vendorUrl = "http://127.0.0.1:6001";

var auth = new TideAuthentication("VendorId", vendorUrl, orkUrls);

var user = "admin";
var pass = "123456";
var newPass = "987654321";
var email = "tmp@tide.org";

(async () => {
  await main();
})();

async function main() {
  try {
    var signUpResult = await auth.register(user, pass, email, orkUrls);

    var userData = {
      id: signUpResult.vendorKey.toString(),
      cvkPub: signUpResult.vendorKey.public().toString(),
    };

    await request.post(`${vendorUrl}/account`).send(userData);

    var login = await auth.login(user, pass);

    await tide.changePassword(pass, newPass);

    var newLogin = await auth.login(user, newPass);

    console.log("Done login");
  } catch (error) {
    console.log(error);
  }
}
