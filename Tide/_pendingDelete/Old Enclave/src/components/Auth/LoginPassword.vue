<template>
  <span>
    <div id="not-you" @click="$parent.changeMode('LoginUsername')">Not you?</div>
    <h2>Hi {{ user.username }}</h2>

    <form @submit.prevent="login">
      <input type="password" class="mt-50" v-model="user.password" placeholder="Enter Password" ref="focus" />

      <div class="action-row mt-50">
        <p v-if="!$store.getters.demoMode" @click="$parent.changeMode('ForgotSend')">Forgot Password?</p>
        <p v-else></p>
        <button>LOGIN</button>
      </div>

      <!-- <div class="advanced-options">
        <label class="checkbox-container">
          Enter account settings after sign in
          <input type="checkbox" v-model="user.goToDashboard" />
          <span class="checkmark"></span>
        </label>
      </div> -->
    </form>
  </span>
</template>

<script>
export default {
  props: ["user"],
  mounted() {
    this.$refs.focus.focus();
  },
  methods: {
    async login() {
      this.$loading(true, "Logging in...");
      this.$nextTick(async () => {
        try {
          var loginResult = await this.$store.dispatch("loginAccount", this.user);

          await this.$store.dispatch("finalizeAuthentication", loginResult);
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
#not-you {
  position: absolute;
  top: 10px;
  left: 10px;
  cursor: pointer;
  color: #555555;
  font-size: 0.9rem;
  &:hover {
    color: orange;
  }
}
</style>
