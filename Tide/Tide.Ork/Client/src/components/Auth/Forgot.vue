<template>
  <div>
    <h1>Forgot Password</h1>
    <div v-if="step == 0">
      <h2>Send emails</h2>
      <form @submit.prevent="sendEmails">
        <div class="form-group">
          <label for="email">Recovery Email</label>
          <input type="text" id="email" required v-model="recoveryEmail" />
        </div>
        <div class="form-group">
          <button type="submit">SEND EMAILS</button>
        </div>
        <p>OR</p>
        <p class="link" @click="$parent.changeMode('Login')">Login</p>
      </form>
    </div>
    <div v-if="step == 1">
      <form @submit.prevent="reconstruct">
        <div class="form-group" v-for="i in 3" :key="i">
          <label :for="`email${i}`">Frag 1</label>
          <input type="text" :id="`email${i}`" required v-model="frags[`frag${i}`]" />
        </div>
        <hr />
        <div class="form-group">
          <label for="new-password">New Password</label>
          <input type="text" id="new-password" required v-model="newPassword" />
        </div>
        <div class="form-group">
          <button type="submit">CONSTRUCT</button>
        </div>
        <p>OR</p>
        <p class="link" @click="$parent.changeMode('Login')">Login</p>
      </form>
    </div>
  </div>
</template>

<script>
export default {
  props: ["user"],
  data() {
    return {
      step: 0,
      recoveryEmail: "matt@tide.org",
      frags: {
        // frag1: "",
        // frag2: "",
        // frag3: "",
        frag1: "Rz2GgigbgKuXUrkwSYOF5whIsbXQPAbQdMih3g02cqEiODHK5fMb9wpQYfJd/s0F",
        frag2: "PTupV0ZOEyw8p5rMK+PX0Q424bnJ7nYh5HsfyJ8BbKf0pWmPb8f404XiW2DYHc+y",
        frag3: "9UMaFR74kxhyxmRKjkC3Yg66xE+XKmspDFCTBVDO7xHJ6hCvteZp4vxV8vYEpva9",
        frag4: "",
        frag5: "",
        frag6: "",
        frag7: "",
        frag8: "",
        frag9: "",
        frag10: "",
      },
      newPassword: "password",
    };
  },
  methods: {
    async sendEmails() {
      this.$loading(true, "Sending emails...");
      this.$nextTick(async () => {
        try {
          if (this.recoveryEmail == "") return (this.error = "Please input your email address.");
          await this.$store.dispatch("sendRecoverEmails", this.user);
          this.step = 1;
        } catch (error) {
          this.$bus.$emit("show-error", error);
        } finally {
          this.$loading(false, "");
        }
      });
    },
    async reconstruct() {
      this.$loading(true, "Changing your password...");
      this.$nextTick(async () => {
        try {
          var shares = `${this.frags.frag1}\n${this.frags.frag2}\n${this.frags.frag3}`;

          var user = await this.$store.dispatch("reconstructAccount", { username: this.user.username, shares, newPass: this.newPassword });
          this.$bus.$emit("show-status", "Your password has been changed");

          this.$parent.changeMode("Login");
        } catch (error) {
          this.$bus.$emit("show-error", error);
        } finally {
          this.$loading(false, "");
        }
      });
    },
  },
};
</script>

<style lang="scss" scoped></style>
