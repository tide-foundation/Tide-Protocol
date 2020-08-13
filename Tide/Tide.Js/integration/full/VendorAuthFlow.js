import Tide from "../../../src/Tide";

(async () => {
  await main();
})();

async function main() {
  try {
    var dnsOrks = [...Array(3)].map((_, i) => `https://ork-${i}.azurewebsites.net`);

    var tide = new Tide("VendorId", "https://tidevendor.azurewebsites.net/tide/v1", dnsOrks);

    await tide.initialize();

    const shuffledOrks = tide.orks.sort(() => 0.5 - Math.random());
    var orkIds = shuffledOrks.slice(0, 4).map((o) => o.id);

    var user = `User${Math.floor(Math.random() * 1000000)}`;
    var pass = "123456";

    var signUp = await tide.register(user, pass, user, orkIds);

    var msg = "Test Cipher";
    var cipher = tide.encrypt(msg);
    console.log(`cipher: ${msg}. decrypted: ${tide.decrypt(cipher)}`);

    var loginResult = await tide.login(user, pass);
    console.log("Completed register & login, key: ", loginResult);
  } catch (error) {
    console.log(error);
  }
}
