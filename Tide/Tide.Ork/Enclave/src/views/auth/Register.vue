<template>
  <div id="login" class="full-height f-c auth-page">
    <div class="f-r page-title">
      <div class="spacer line"></div>
      <h1>Register to RMIT</h1>
      <div class="spacer "></div>
    </div>
    <form @submit.prevent="register">
      <tide-input id="username" v-model="user.username">Username</tide-input>
      <tide-input id="password" v-model="user.password" type="password">Password</tide-input>
      <tide-input id="email" v-model="user.email" type="email">Email</tide-input>
      <div class="actions">
        <router-link class="font-small " to="/login">Have an account?</router-link>
        <button>Register</button>
      </div>
    </form>

    <div class="loading-graphic full-width full-height f-c"></div>
  </div>
</template>

<script lang="ts">
import Base from "@/assets/ts/Base";

export default class Register extends Base {
  user: UserPass = { username: ``, password: "" };
  async register() {
    try {
      this.setLoading(true);

      await this.mainStore.registerAccount(this.user);
      this.mainStore.authenticationComplete();
    } catch (error) {
      this.showAlert("error", `Failed to register: ${error}`);

      this.setLoading(false);
    }
  }
}
</script>

<style lang="scss" scoped>
#register {
  #registering {
  }
}
</style>
