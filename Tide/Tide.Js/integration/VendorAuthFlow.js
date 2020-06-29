import Tide from "../src/Tide";


(async () => {
  await main();
})();

async function main() {
  try {
    var urls = [...Array(3)].map(
      (_, i) => `https://ork-${i}.azurewebsites.net`
    );
    var tide = new Tide("VendorId", "https://tidevendor.azurewebsites.net/vendor", urls);

   var user = `User${Math.floor(Math.random() * 1000000)}`;
  // var user = "lol1232";
    var pass = "123456";
    var signUp = await tide.register(user, pass, "tmp@tide.org");
    console.log(signUp);

    var cipher = tide.encrypt('heyyyo')
    console.log(tide.decrypt(cipher))

    var loginResult = await tide.login(user, pass);
    console.log(loginResult);
  } catch (error) {
    console.log(error);
  }
}  
        