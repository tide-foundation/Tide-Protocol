<template>
  <div id="reconstruct" class="full-height f-c auth-page">
    <div class="f-r page-title">
      <div class="spacer line"></div>
      <h1>Change your password</h1>
      <div class="spacer "></div>
    </div>
    <p class="font-small">
      Enter a new password. Make it a good one!
    </p>
    <form @submit.prevent="change">
      <tide-input id="password" type="password" v-model="newPassword.password">New password</tide-input>
      <tide-input id="confirm" type="password" v-model="newPassword.confirm">Confirm new password</tide-input>
      <div class="actions">
        <a href="#" class="font-small" @click.native="router.push('/account')">Back to account</a>
        <button>Change Password</button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, inject } from "vue";
import TideInput from "@/components/Tide-Input.vue";
import { BUS_KEY, SET_LOADING_KEY, SHOW_ERROR_KEY } from "@/assets/ts/Constants";
import mainStore from "@/store/mainStore";
import router from "@/router/router";

const bus = inject(BUS_KEY) as IBus;

var newPassword = ref<NewPassword>({ password: ``, confirm: "" });

const change = async () => {
  try {
    bus.trigger(SET_LOADING_KEY, true);

    await mainStore.changePassword(newPassword.value);

    bus.trigger(SHOW_ERROR_KEY, { type: "success", msg: "Your password has been changed" } as Alert);

    router.push("/account");
  } catch (error) {
    bus.trigger(SHOW_ERROR_KEY, { type: "error", msg: `Failed changing your password. ${error}` } as Alert);
  } finally {
    bus.trigger(SET_LOADING_KEY, false);
  }
};
</script>

<style lang="scss" scoped>
#reconstruct {
  width: 100%;
  max-width: 1150px;
  height: 100%;
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
    margin-top: 20px;
    margin-bottom: 50px;
  }
}
</style>
