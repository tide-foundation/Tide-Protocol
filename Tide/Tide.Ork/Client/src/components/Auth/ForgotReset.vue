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
        <p @click="$parent.changeMode('LoginPassword')">Back to Sign-in</p>
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
          for (let i = 0; i < this.user.frags.length; i++) assembled += `${this.user.frags[i]}${i != this.user.frags.length - 1 ? "\n" : ""}`;
          await this.$store.dispatch("reconstructAccount", { username: this.user.username, shares: assembled, newPass: this.user.password });

          this.$bus.$emit("show-status", "Your password has been changed");
          this.$parent.changeMode("LoginPassword");
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

<style></style>
