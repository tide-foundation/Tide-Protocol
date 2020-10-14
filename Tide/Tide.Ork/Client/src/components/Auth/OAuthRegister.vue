<template>
  <div>
    <h1>Register</h1>
    <form @submit.prevent="register">
      <div class="form-group">
        <button type="submit">REGISTER WITH TIDE</button>
      </div>
      <p>{{ message }}</p>
      <p>OR</p>
      <p class="link" @click="$parent.changeMode('Login')">Login</p>
    </form>
  </div>
</template>

<script>
import request from "superagent";
export default {
  props: ["user"],
  data() {
    return {
      message: "",
    };
  },
  methods: {
    async register() {
      let seconds = 3;
      this.updateMessage(seconds);
      const i = setInterval(() => {
        seconds--;
        this.updateMessage(seconds);
        if (seconds == 0) {
          const data = {
            type: "authenticated",
            cvk: "User cvk",
            scope: "something lol",
          };

          window.opener.postMessage(JSON.stringify(data), "http://127.0.0.1:5500");
          clearInterval(i);
        }
      }, 1000);
    },
    updateMessage(seconds) {
      this.message = `Closing in ${seconds} seconds`;
    },
  },
};
</script>

<style lang="scss" scoped></style>
