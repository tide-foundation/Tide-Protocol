<template>
    <span>
        <h2>Sign in</h2>
        <form @submit.prevent="$parent.changeMode('LoginPassword')">
            <input type="text" required class="mt-50" v-model="user.username" placeholder="Username" />

            <div class="action-row mt-50">
                <p @click="$parent.changeMode('Register')">Create Account</p>
                <button>Next</button>
            </div>

            <div class="advanced-options" @click="$parent.changeMode('ChangeOrk')"><p>Advanced Options</p></div>
        </form>
        <div id="qr">
            <canvas id="canvas"></canvas>
            <span> Tide Shield</span>
        </div>
    </span>
</template>

<script>
var QRCode = require("qrcode");
export default {
    props: ["user"],
    mounted() {
        var canvas = document.getElementById("canvas");
        console.log(canvas);
        QRCode.toCanvas(canvas, "From Tide, with love", { color: { dark: "#1e7ec2" }, margin: 0, errorCorrectionLevel: "L" });
    },
    methods: {
        async login() {
            try {
                this.$loading(true, "Logging in...");
                var loginResult = await this.$store.dispatch("loginAccount", this.user);

                await this.$store.dispatch("finalizeAuthentication", loginResult);
            } catch (error) {
                this.$bus.$emit("show-error", error);
            } finally {
                this.$loading(false, "");
            }
        },
    },
};
</script>

<style lang="scss" scoped>
#qr {
    position: absolute;
    right: 10px;
    top: 10px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    text-align: center;
    #canvas {
        background: red;
        margin-bottom: 0px;
    }
    span {
        font-size: 10px;
        margin-top: 5px;
        color: #000000;
        z-index: 20;
    }
}
#filler {
    height: 6px;
}
#go-to-account-box {
    display: flex;
    flex-direction: column;

    input {
        width: 20px;
        padding-left: 0px;
        margin-left: 0px;
    }
}
#go-to-account {
    text-align: left;
}
</style>
