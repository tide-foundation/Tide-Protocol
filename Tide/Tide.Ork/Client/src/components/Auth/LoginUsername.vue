<template>
  <span>
    <h2>Sign in</h2>
    <form @submit.prevent="$parent.changeMode('LoginPassword')">
      <input type="text" required class="mt-50" v-model="user.username" placeholder="Username" />

      <div class="action-row mt-50">
        <p @click="$parent.changeMode('Register')">Create Account</p>
        <button>Next</button>
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
      <div class="advanced-options" @click="$parent.changeMode('ChangeOrk')"><p>Advanced Options</p></div>
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
