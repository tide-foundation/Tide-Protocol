const chai = window.chai;
const mocha = window.mocha;
mocha.setup("bdd");

describe("Tide Actions", () => {
  var dnsOrks = [...Array(3)].map((_, i) => `https://ork-${i}.azurewebsites.net`);
  var tide = new Tide("VendorId", "https://tidevendor.azurewebsites.net/tide/v1", dnsOrks);
  var user = {
    username: `User${Math.floor(Math.random() * 1000000)}`,
    password: "654321",
  };

  describe("Initialization", () => {
    it("Creates ork array", (done) => {
      tide
        .initialize()
        .then((r) => {
          if (tide.orks != null && tide.orks.length > 0) done();
          else done("Ork list was not established");
        })
        .catch((e) => done(e));
    });
  });

  describe("Decentralized Authentication", () => {
    it("Registers an account", (done) => {
      const shuffledOrks = tide.orks.sort(() => 0.5 - Math.random());
      user.selectedOrks = shuffledOrks.slice(0, 4).map((o) => o.id);

      tide
        .register(user.username, user.password, user.username, user.selectedOrks)
        .then((r) => {
          done();
        })
        .catch((e) => done(e));
    });

    it("Logs into an account", (done) => {
      tide
        .login(user.username, user.password)
        .then((r) => {
          done();
        })
        .catch((e) => done(e));
    });
  });

  describe("Encryption Flow", () => {
    var toEncrypt = "👋🏿 this is you!!!  😰🥰";
    var encrypted;
    it("Encrypts a string", () => {
      // TODO: Ask jose how to test for an encrypted value properly
      encrypted = tide.encrypt(toEncrypt);
      chai.expect(encrypted).to.have.length.above(50);
    });

    it("Decrypts the string", () => {
      // TODO: Ask jose how to test for an encrypted value properly
      var decrypted = tide.decrypt(encrypted);
      chai.expect(toEncrypt).to.eql(decrypted);
    });
  });
});
