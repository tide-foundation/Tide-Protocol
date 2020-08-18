import Tide from "../../src/Tide";
//import "../../dist/tide";

console.log(Tide);

var orkUrls = [...Array(3)].map((_, i) => `https://ork-${i}.azurewebsites.net`);
var vendorUrl = "https://tidevendor.azurewebsites.net/";

var orkUrls = [...Array(3)].map((_, i) => "http://localhost:500" + (i + 1));
var vendorUrl = "http://127.0.0.1:6001";

var tide = new Tide("VendorId", vendorUrl, orkUrls, "publickey");

var user = "admin";
var pass = "123456";
var email = "tmp@tide.org";

(async () => {
  await main();
})();

async function main() {
  try {
    var signUp = await tide.registerV2(user, pass, email, orkUrls);
    console.log("Done signup");
    //
    var login = await tide.loginV2(user, pass, orkUrls);
    console.log("Done login");
  } catch (error) {
    console.log(error);
  }
}
