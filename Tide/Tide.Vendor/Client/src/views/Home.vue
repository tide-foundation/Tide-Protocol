<template>
  <div id="home">
    <h1>THIS IS AN EXAMPLE VENDOR WEBSITE</h1>
    <h2>PLEASE LOGIN OR REGISTER</h2>
    <span id="tide"></span>

    <button @click="getProtected" id="protected-data-btn">Request Protected Data</button>
    <p :style="{ color: error ? 'red' : 'black' }" v-html="protectedData"></p>
  </div>
</template>

<script src="tide-button.js"></script>
<script>
import request from "superagent";
export default {
  name: "Home",
  data() {
    return {
      jwt: "",
      protectedData: "",
      error: false,
    };
  },
  created() {
    window.addEventListener("tide-auth", async (e) => {
      var data = { vuid: e.detail.data.vuid, tideToken: e.detail.data.tideToken, publicKey: e.detail.data.cvkPublic };

      this.jwt = (await request.post(`https://futureplaces.azurewebsites.net/Authentication/register`).send(data)).text;
    });
    this.init();
  },
  methods: {
    init() {
      const config = {
        homeUrl: process.env.VUE_APP_RETURN_URL,
        hashedReturnUrl: process.env.VUE_APP_RETURN_URL_HASH,
        serverUrl: process.env.VUE_APP_SERVER_URL,
        chosenOrk: process.env.VUE_APP_CHOSEN_ORK,
        vendorPublic: process.env.VUE_APP_VENDOR_PUBLIC,
      };
      console.log(config);
      Tide.init(config);
    },
    async getProtected() {
      try {
        this.error = false;
        this.protectedData = "";
        var test = await request.get(`https://futureplaces.azurewebsites.net/Authentication`).set("Authorization", `Bearer ${this.jwt}`);

        this.protectedData = `Data successfully fetched for user: <strong> ${test.text}</strong>`;
      } catch (error) {
        this.error = true;
        this.protectedData = `Failed gathering data`;
      }
    },
  },
};
</script>

<style lang="scss">
#home {
  display: flex;
  justify-content: center;
  flex-direction: column;
  text-align: center;

  #protected-data-btn {
    width: 200px;
    margin: 20px auto;
  }
}
</style>
