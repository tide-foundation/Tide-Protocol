<template>
  <div>
    <h2>Reset Password</h2>
    <form @submit.prevent="resetPassword">
      <div class="mt-50 password-row">
        <input type="password" v-model="user.password" placeholder="Password" />
        <input type="password" v-model="user.confirm" placeholder="Confirm" />
      </div>

      <password-meter :password="user.password" class="mt-10" />

      <div class="action-row mt-50">
        <p @click="$parent.changeMode('ForgotReconstruct')">Back to fragments</p>
        <button type="submit">Done</button>
      </div>
    </form>
  </div>
</template>

<script>
import passwordMeter from "vue-simple-password-meter";
export default {
  props: ["user"],
  components: { passwordMeter },
  methods: {
    async resetPassword() {
      this.$loading(true, "Resetting password...");
      this.$nextTick(async () => {
        try {
          let assembled = "";
          for (let i = 0; i < this.user.frags.length; i++) if (this.user.frags[i] != "" && this.user.frags[i] != null) assembled += `${this.user.frags[i]}${i != this.user.frags.length - 1 ? "\n" : ""}`;

          await this.$store.dispatch("reconstructAccount", { username: this.user.username, shares: assembled, newPass: this.user.password });

          this.$bus.$emit("show-status", "Your password has been changed");
          this.$parent.changeMode("LoginPassword");
        } catch (error) {
          console.log(error);
          this.$bus.$emit("show-error", "Failed resetting your password. Please check your fragments are correct and try again.");
        } finally {
          this.$loading(false, "");
        }
      });
    },
  },
};
</script>

<style></style>
