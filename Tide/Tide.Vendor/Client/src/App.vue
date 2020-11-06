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
            config: {
                homeUrl: process.env.VUE_APP_RETURN_URL,
                hashedReturnUrl: process.env.VUE_APP_RETURN_URL_HASH,
                serverUrl: process.env.VUE_APP_SERVER_URL,
                chosenOrk: process.env.VUE_APP_CHOSEN_ORK,
                vendorPublic: process.env.VUE_APP_VENDOR_PUBLIC,
            },
        };
    },
    created() {
        window.addEventListener("tide-auth", async (e) => {
            var data = { vuid: e.detail.data.vuid, tideToken: e.detail.data.tideToken, publicKey: e.detail.data.cvkPublic };
            console.log(data);
            //this.jwt = (await request.post(`${this.config.serverUrl}/Authentication/${e.detail.data.action}`).send(data)).text;
            const resp = await request.post(`${this.config.serverUrl}/tide/register`).send(data);
            this.jwt = resp.headers["authorization"];
        });
        Tide.init(this.config);
    },
    methods: {
        async getProtected() {
            try {
                this.error = false;
                this.protectedData = "";

                var data = await request.get(`${this.config.serverUrl}/vendor`).set("Authorization", this.jwt);

                this.protectedData = `Data successfully fetched for user: <strong> ${data.text}</strong>`;
            } catch (error) {
                this.error = true;
                this.protectedData = `Failed gathering data`;
            }
        },
    },
};
</script>

<style lang="scss">
@import url("https://fonts.googleapis.com/css2?family=Montserrat:wght@300&display=swap");
body,
html {
    font-family: "Montserrat", sans-serif;
    width: 100%;
    padding: 0;
    margin: 0;
}
#home {
    width: 100%;
    height: 100vh;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;
}

#protected-data-btn {
    width: 200px;
    margin: 20px auto;
}

.loading {
    text-align: center;
    z-index: 1000000000000;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;
    width: 100%;
    height: 100%;
    position: fixed;
    top: 0;
    right: 0;
    bottom: 0;
    left: 0;
    background-color: rgba(0, 0, 0, 0.8);

    @keyframes spin {
        0% {
            transform: rotate(0);
        }
        100% {
            transform: rotate(-360deg);
        }
    }
}
</style>
