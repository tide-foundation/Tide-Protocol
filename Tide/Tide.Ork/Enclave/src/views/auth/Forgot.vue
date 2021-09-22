<template>
  <div id="forgot" class="full-height f-c auth-page">
    <div class="f-r page-title">
      <div class="spacer line"></div>
      <h1>Recover your password</h1>
      <div class="spacer "></div>
    </div>
    <p class="font-small">
      This will send password fragments to your nominated email address(es)
    </p>
    <form @submit.prevent="sendEmails">
      <tide-input id="username" v-model="user.username">Username</tide-input>
      <div class="actions">
        <router-link class="font-small " to="/login">Back to login</router-link>
        <button>Recover</button>
      </div>
    </form>

    <div class="loading-graphic full-width full-height f-c"></div>
  </div>
</template>

<script setup lang="ts">
import { ref, inject } from "vue";
import TideInput from "@/components/Tide-Input.vue";
import { BUS_KEY, FORGOT_PASSWORD_USERNAME, SET_LOADING_KEY, SHOW_ERROR_KEY } from "@/assets/ts/Constants";
import mainStore from "@/store/mainStore";
import router from "@/router/router";

const bus = inject(BUS_KEY) as IBus;

var user = ref<UserPass>({ username: ``, password: "" });

const sendEmails = async () => {
  try {
    sessionStorage.setItem(FORGOT_PASSWORD_USERNAME, user.value.username);
    bus.trigger(SET_LOADING_KEY, true);
    await mainStore.sendRecoveryEmails(user.value.username);
    router.push("Reconstruct");
  } catch (error) {
    bus.trigger(SHOW_ERROR_KEY, { type: "error", msg: `Failed to send emils: ${error}` } as Alert);
  } finally {
    bus.trigger(SET_LOADING_KEY, false);
  }
};
</script>

<style lang="scss" scoped>
#forgot {
  h1 {
    margin-bottom: 0px;
  }

  p {
    margin-bottom: 50px;
  }
  .actions {
    a {
      max-width: 80px;
    }
  }
}
</style>
