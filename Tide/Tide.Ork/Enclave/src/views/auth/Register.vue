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
      <div class="actions">
        <router-link class="font-small " to="/login">Have an account?</router-link>
        <button>Register</button>
      </div>
    </form>

    <div class="loading-graphic full-width full-height f-c"></div>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import TideInput from "@/components/Tide-Input.vue";
import mainStore from "@/store/mainStore";

var user = ref<UserPass>({ username: `tide_user_${(Math.random() + 1).toString(36).substring(7)}`, password: "333" });

const register = async () => {
  await mainStore.registerAccount(user.value);
  mainStore.authenticationComplete();
};
</script>

<style lang="scss" scoped>
#register {
  #registering {
  }
}
</style>
