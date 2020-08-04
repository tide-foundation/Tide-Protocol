import Tide from "../src/Tide";

(async () => {
  await main();
})();

async function main() {
  try {
    var urls = [...Array(3)].map(
      (_, i) => `https://ork-${i}.azurewebsites.net`
    );
    var tide = new Tide(
      "VendorId",
      "https://tidevendor.azurewebsites.net/tide/v1",
      urls
    );

    await tide.initialize();

    const shuffledOrks = tide.orks.sort(() => 0.5 - Math.random());
    var orkIds = shuffledOrks.slice(0, 4).map((o) => o.id);

    var user = `User${Math.floor(Math.random() * 1000000)}`;
    // var user = "lol1232";
    var pass = "123456";
    var signUp = await tide.register(user, pass, "tmp@tide.org", orkIds);

    var cipher = tide.encrypt("heyyyo");
    console.log(tide.decrypt(cipher));

    var loginResult = await tide.login(user, pass);
    console.log(loginResult);
  } catch (error) {
    console.log(error);
  }
}
