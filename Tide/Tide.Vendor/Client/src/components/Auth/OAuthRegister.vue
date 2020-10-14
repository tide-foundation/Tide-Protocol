<template>
  <div>
    <h1>Register</h1>
    <form @submit.prevent="register">
      <div class="form-group">
        <button type="submit">REGISTER WITH TIDE</button>
      </div>
      <p>OR</p>
      <p class="link" @click="$parent.changeMode('Login')">Login</p>
    </form>
  </div>
</template>

<script>
import request from "superagent";
export default {
  props: ["user"],
  methods: {
    async register() {
      try {
        //  const url = `https://www.facebook.com/v8.0/dialog/oauth?client_id=367270867419899&redirect_uri=http://localhost:8080/validate&state=test`;
        const url = `http://172.26.17.60:8081/`;

        var win = window.open(url, "windowname1", "width=500, height=501");

        var pollTimer = window.setInterval(function() {
          try {
            console.log(win.document.URL);
            if (win.document.URL.indexOf(REDIRECT) != -1) {
              window.clearInterval(pollTimer);
              var url = win.document.URL;
              acToken = gup(url, "access_token");
              tokenType = gup(url, "token_type");
              expiresIn = gup(url, "expires_in");
              win.close();

              validateToken(acToken);
            }
          } catch (e) {}
        }, 100);
      } catch (error) {
        this.$bus.$emit("show-status", error);
      } finally {
        this.$loading(false, "");
      }
    },
  },
};
</script>

<style lang="scss" scoped></style>
