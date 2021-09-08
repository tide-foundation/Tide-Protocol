<template>
  <div id="login" class="full-height f-c auth-page">
    <div class="f-r page-title">
      <div class="spacer line"></div>
      <h1>Sign in to RMIT</h1>
      <div class="spacer "></div>
    </div>
    <form @submit.prevent="login">
      <tide-input id="username" v-model="user.username">Username</tide-input>
      <tide-input id="password" v-model="user.password" type="password">Password</tide-input>
      <div class="actions">
        <router-link class="font-small " to="/register">Create an account</router-link>
        <button>Sign In</button>
      </div>
    </form>

    <div class="loading-graphic full-width full-height f-c"></div>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import TideInput from "@/components/Tide-Input.vue";
import mainStore from "@/store/mainStore";

var user = ref<UserPass>({ username: "445", password: "445" });

const login = async () => {
  await mainStore.login(user.value);
  mainStore.authenticationComplete();
};
</script>

<style lang="scss" scoped>
#login {
}
</style>
