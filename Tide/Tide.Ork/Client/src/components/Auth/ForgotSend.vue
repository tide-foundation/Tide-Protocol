<template>
  <div>
    <h2>Forgot Password</h2>
    <form @submit.prevent="sendEmails">
      <p class="disclaimer mt-50">
        This will send password fragments to your nominated email address(es)
      </p>

      <div class="action-row mt-50">
        <p @click="$parent.changeMode('LoginPassword')">Back to Sign-in</p>
        <button type="submit">Send Emails</button>
      </div>
    </form>
  </div>
</template>

<script>
export default {
  props: ["user"],
  methods: {
    async sendEmails() {
      this.$loading(true, "Sending emails...");
      this.$nextTick(async () => {
        try {
          console.log(this.user);
          await this.$store.dispatch("sendRecoverEmails", this.user);
          this.$parent.changeMode("ForgotReconstruct");
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

<style lang="scss" scoped>
.black {
  color: black;
}
</style>
