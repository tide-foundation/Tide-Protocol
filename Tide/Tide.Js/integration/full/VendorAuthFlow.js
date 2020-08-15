import Tide from "../../src/Tide";

(async () => {
  await main();
})();

async function main() {
  try {
    var orkUrls = [...Array(3)].map((_, i) => `https://ork-${i}.azurewebsites.net`);
    var vendorUrl = "https://tidevendor.azurewebsites.net/";

    // var orkUrls = [...Array(3)].map((_, i) => "http://localhost:500" + (i + 1));
    // var vendorUrl = "http://127.0.0.1:6001";

    var user = "admin";
    var pass = "123456";
    var email = "tmp@tide.org";

    var tide = new Tide("VendorId", vendorUrl, orkUrls, "publickey");

    var signUp = await tide.registerV2(user, pass, email, orkUrls);
    console.log(signUp);
    //
    var login = await tide.loginV2(user, pass, orkUrls);
    console.log(login);
  } catch (error) {
    console.log(error);
  }
}
