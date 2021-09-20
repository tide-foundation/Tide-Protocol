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
  </div>
</template>

<script setup lang="ts">
import { ref, inject } from "vue";
import TideInput from "@/components/Tide-Input.vue";
import mainStore from "@/store/mainStore";
import { BUS_KEY, SET_LOADING_KEY } from "@/assets/ts/Constants";
import router from "@/router/router";

var user = ref<UserPass>({ username: "", password: "" });

const bus = inject(BUS_KEY) as IBus;

const login = async () => {
  try {
    bus.trigger(SET_LOADING_KEY, true);
    await mainStore.login(user.value);
    // Go to form if data is available
    if (mainStore.getState.config.formData != null) return router.push("/form");
    else mainStore.authenticationComplete();
  } catch (error) {
    bus.trigger(SET_LOADING_KEY, false);
  }
};
</script>

<style lang="scss" scoped>
#login {
}
</style>
