<template>
  <span>
    <div id="not-you" @click="$parent.changeMode('LoginUsername')">Not you?</div>
    <h2>Hi {{ user.username }}</h2>

    <form @submit.prevent="login">
      <input type="password" class="mt-50" v-model="user.password" placeholder="Enter Password" />

      <div class="action-row mt-50">
        <p>Forgot Password?</p>
        <button>LOGIN</button>
      </div>
      <!-- <div class="form-group">
        <label for="password">Password</label>
        <input type="password" id="password" v-model="user.password" />
      </div>
      <div class="form-group" id="go-to-account-box">
        <label for="go-to-account">Go to account</label>
        <input type="checkbox" id="go-to-account" v-model="user.goToDashboard" />
      </div> -->
      <!-- <div class="form-group">
        <button type="submit">LOGIN</button>
      </div>
      <p>OR</p>
      <p class="link" @click="$parent.changeMode('Register')">Register</p> -->
      <div class="advanced-options">
        <label class="checkbox-container">
          Enter account settings after sign in
          <input type="checkbox" checked="checked" />
          <span class="checkmark"></span>
        </label>
        <!-- <p>Enter account settings after sign in</p> -->
      </div>
    </form>
  </span>
</template>

<script>
export default {
  props: ["user"],
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
