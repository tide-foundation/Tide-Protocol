import Tide from "../../src/Tide";
import request from "superagent";
import Cipher from "../../src/Cipher";
//import "../../dist/tide";

var orkUrls = [...Array(3)].map((_, i) => `https://ork-${i}.azurewebsites.net/`);
var vendorUrl = "https://tidevendor.azurewebsites.net/";

// var orkUrls = [...Array(3)].map((_, i) => "http://localhost:500" + (i + 1));
// var vendorUrl = "http://127.0.0.1:6001";

var tide = new Tide("VendorId", vendorUrl, orkUrls, "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAANvjzMxmyjGxse3fwkqajZxhf088eQRgS4l9wKsnm+A2+HRLt/4n6lA0cO6pmBqB9Le72HFSQ1s9cjv6HF3O2m", [
  { name: "field1", condition: "true" },
  { name: "field2", condition: "true" },
]);

var user = "admin";
var pass = "123456";
var email = "tmp@tide.org";

(async () => {
  await main();
})();

async function main() {
  try {
    await tide.initialize();

    var signUpResult = await tide.registerV2(user, pass, email, orkUrls);

    var userData = {
      id: signUpResult.vuid.toString(),
      cvkPub: signUpResult.publicKey,
      field1: tide.encrypt("My Field 1", "field1"),
      field2: tide.encrypt("My Field 2", "field2"),
    };

    await request.post(`${vendorUrl}/account`).send(userData);

    // var rules = await tide.getRules();
    // console.log(rules);
    // var ruleToEdit = rules[0];
    // ruleToEdit.condition = `[{"field":"DateInfo.Day","operator":"==","value":1,"level":0},{"union":"&&","field":"DateInfo.Today","operator":">","value":"21/01/2036","level":0}]`;
    // await tide.updateRule(ruleToEdit);

    var login = await tide.loginV2(user, pass, orkUrls);

    console.log("Done login");
  } catch (error) {
    console.log(error);
  }
}
