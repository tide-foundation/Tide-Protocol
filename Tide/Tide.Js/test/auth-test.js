const chai = window.chai;
const mocha = window.mocha;
mocha.setup("bdd");
console.log(Tide);
describe("Tide Actions", () => {
  var orkUrls = [...Array(3)].map((_, i) => `https://ork-${i}.azurewebsites.net`);
  var vendorUrl = "https://tidevendor.azurewebsites.net/";

  // var orkUrls = [...Array(3)].map((_, i) => "http://localhost:500" + (i + 1));
  // var vendorUrl = "http://127.0.0.1:6001";

  var user = "admin";
  var pass = "123456";
  var email = "tmp@tide.org";

  var tide = new Tide("VendorId", vendorUrl, orkUrls, "publickey");

  describe("Decentralized Authentication", () => {
    it("Registers an account", (done) => {
      tide
        .registerV2(user, pass, email, orkUrls)
        .then((r) => {
          done();
        })
        .catch((e) => done(e));
    });

    it("Logs into an account", (done) => {
      tide
        .loginV2(user, pass, orkUrls)
        .then((r) => {
          done();
        })
        .catch((e) => done(e));
    });
  });
});
