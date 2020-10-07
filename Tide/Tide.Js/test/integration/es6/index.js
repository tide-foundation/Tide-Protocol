import Tide from "./tide";

var vendorUrl = "https://tidevendor.azurewebsites.net";
// vendorUrl = "  http://127.0.0.1:6001";

var orkUrls = [...Array(3)].map((_, i) => `https://ork-${i}.azurewebsites.net`);
// orkUrls = [...Array(3)].map((_, i) => "http://localhost:500" + i);

const tide = new Tide("VendorId", vendorUrl, orkUrls, "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAANvjzMxmyjGxse3fwkqajZxhf088eQRgS4l9wKsnm+A2+HRLt/4n6lA0cO6pmBqB9Le72HFSQ1s9cjv6HF3O2m", [{ name: "mandatory", condition: "true" }]);

const user = "test@test.com";
const pass = "admin12345";

async function register() {
  try {
    var res = tide.registerV2(user, pass, user, orkUrls);
    console.log(res);
  } catch (error) {
    console.log(error);
  }
}

register();
