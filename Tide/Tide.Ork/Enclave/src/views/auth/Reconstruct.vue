<template>
  <div id="reconstruct" class="full-height f-c auth-page">
    <div class="f-r page-title">
      <div class="spacer line"></div>
      <h1>{{ mode == "Reconstruct" ? "Reconstruct" : "Choose a new password" }}</h1>
      <div class="spacer "></div>
    </div>
    <p class="font-small">
      {{
        mode == "Reconstruct" ? "Input the fragments found in your nominated email address(s) to reconsuct your Tide account" : "Make it a good one!"
      }}
    </p>
    <form @submit.prevent="toNextStep" v-if="mode == 'Reconstruct'">
      <div class="row">
        <div class="col-12 col-md-4" v-for="(frag, index) in frags">
          <tide-input :key="index" :id="`frag-${index}`" v-model="frag.val">Fragment {{ index + 1 }}</tide-input>
        </div>
      </div>

      <div class="actions">
        <router-link class="font-small " to="/login">Back to login</router-link>
        <button>Reconstruct</button>
      </div>
    </form>

    <form @submit.prevent="reset" v-else>
      <tide-input id="password" type="password" v-model="newPassword.password">New password</tide-input>
      <tide-input id="confirm" type="password" v-model="newPassword.confirm">Confirm new password</tide-input>
      <div class="actions">
        <a href="#" class="font-small" @click.native="() => (mode = 'Reconstruct')">Back to fragments</a>
        <button>Reset Password</button>
      </div>
    </form>

    <div class="loading-graphic full-width full-height f-c"></div>
  </div>
</template>

<script setup lang="ts">
interface Fragment {
  val: string;
}

interface NewPassword {
  password: string;
  confirm: string;
}

type ForgotMode = "Reconstruct" | "Reset";

import { ref, inject, onMounted } from "vue";
import TideInput from "@/components/Tide-Input.vue";
import { BUS_KEY, FORGOT_PASSWORD_USERNAME, SET_LOADING_KEY, SHOW_ERROR_KEY } from "@/assets/ts/Constants";
import mainStore from "@/store/mainStore";
import router from "@/router/router";

const bus = inject(BUS_KEY) as IBus;

var mode = ref<ForgotMode>("Reconstruct");

var newPassword = ref<NewPassword>({ password: ``, confirm: "" });

var frags = ref([] as Fragment[]);

const FRAG_COUNT = Math.min(mainStore.getState.config.orks.length, 16);

onMounted(() => {
  for (let i = 0; i < FRAG_COUNT; i++) {
    frags.value.push({ val: "" } as Fragment);
  }
});

const toNextStep = async () => {
  mode.value = "Reset";
};

const reset = async () => {
  try {
    bus.trigger(SET_LOADING_KEY, true);
    const username = sessionStorage.getItem(FORGOT_PASSWORD_USERNAME);
    if (username == null) return;

    let assembled = "";
    for (let i = 0; i < FRAG_COUNT; i++) {
      assembled += `${frags.value[i].val}${i != FRAG_COUNT - 1 ? "\n" : ""}`;
    }

    await mainStore.reconstructAccount(username, assembled, newPassword.value.password);
    bus.trigger(SHOW_ERROR_KEY, { type: "success", msg: "Your password has been changed" } as Alert);

    router.push("/login");
  } catch (error) {
    bus.trigger(SHOW_ERROR_KEY, {
      type: "error",
      msg: `Failed resetting your password. Please check your fragments are correct and try again. ${error}`,
    } as Alert);
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
