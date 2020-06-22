import Tide from "../src/Tide";

(async () => {
  await main();
})();

async function main() {
  try {
    var urls = [...Array(3)].map(
      (_, i) => `https://ork-${i}.azurewebsites.net`
    );
    var tide = new Tide("VendorId", "http://localhost:50598/vendor", urls);

    var user = `User${Math.floor(Math.random() * 1000000)}`;
    var pass = "123456";
    var signUp = await tide.register(user, pass, "tmp@tide.org");
    console.log(signUp);

    var loginResult = await tide.login(user, pass);
    console.log(loginResult);
  } catch (error) {
    console.log(error);
  }
}
