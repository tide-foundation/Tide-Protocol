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

<script setup lang="ts">
import { ref, inject } from "vue";
import TideInput from "@/components/Tide-Input.vue";
import mainStore from "@/store/mainStore";
import { BUS_KEY, SET_LOADING_KEY, SHOW_ERROR_KEY } from "@/assets/ts/Constants";

var user = ref<UserPass>({ username: ``, password: "" });

const bus = inject(BUS_KEY) as IBus;

const register = async () => {
  try {
    bus.trigger(SET_LOADING_KEY, true);
    await mainStore.registerAccount(user.value);
    mainStore.authenticationComplete();
  } catch (error) {
    bus.trigger(SHOW_ERROR_KEY, { type: "error", msg: `Failed to register: ${error}` } as Alert);
    bus.trigger(SET_LOADING_KEY, false);
  }
};
</script>

<style lang="scss" scoped>
#register {
  #registering {
  }
}
</style>
