<template>
  <span>
    <h2>Sign in</h2>
    <!-- <div id="qr">
      <canvas id="canvas"></canvas>
      <p>Scan with <strong class="link bold">Tide Shield</strong> to instantly login</p>
    </div>
    <span class="center bold">OR</span> -->
    <form @submit.prevent="$parent.changeMode('LoginPassword')">
      <input type="text" required class="mt-30" v-model="user.username" placeholder="Username" ref="focus" />

      <div class="action-row mt-20">
        <p v-if="!$store.getters.demoMode" @click="$parent.changeMode('Register')">Create Account</p>
        <p v-else></p>
        <button>Next</button>
      </div>

      <!-- <div class="advanced-options" @click="$parent.changeMode('ChangeOrk')"><p>Advanced Options</p></div> -->
    </form>
  </span>
</template>

<script>
var QRCode = require("qrcode");

export default {
  props: ["user"],

  mounted() {
    var canvas = document.getElementById("canvas");
    QRCode.toCanvas(canvas, this.$store.getters.qrData, { color: {}, margin: 0, errorCorrectionLevel: "L" });

    this.$refs.focus.focus();
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
  display: flex;
  flex-direction: column;
  justify-content: center;
  text-align: center;
  #canvas {
    margin-bottom: 0px;
    margin-left: auto;
    margin-right: auto;
  }
  h3 {
    font-family: acumin-pro, sans-serif;

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
