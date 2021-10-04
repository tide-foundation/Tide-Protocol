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

<script lang="ts">
import Base from "@/assets/ts/Base";
import { FORGOT_PASSWORD_USERNAME } from "@/assets/ts/Constants";

interface Fragment {
  val: string;
}

type ForgotMode = "Reconstruct" | "Reset";

export default class Reconstruct extends Base {
  mode: ForgotMode = "Reconstruct";
  newPassword: NewPassword = { password: ``, confirm: "" };
  frags: Fragment[] = [];
  FRAG_COUNT = Math.min(this.mainStore.getState.config.orks.length, 16);

  mounted() {
    for (let i = 0; i < this.FRAG_COUNT; i++) {
      this.frags.push({ val: "" } as Fragment);
    }
  }

  toNextStep = () => (this.mode = "Reset");

  async reset() {
    try {
      this.setLoading(true);
      const username = sessionStorage.getItem(FORGOT_PASSWORD_USERNAME);
      if (username == null) return;

      let assembled = "";
      for (let i = 0; i < this.FRAG_COUNT; i++) {
        assembled += `${this.frags[i].val}${i != this.FRAG_COUNT - 1 ? "\n" : ""}`;
      }

      await this.mainStore.reconstructAccount(username, assembled, this.newPassword.password);
      this.showAlert("success", "Your password has been changed");

      this.router.push("/login");
    } catch (error) {
      this.showAlert("error", `Failed resetting your password. Please check your fragments are correct and try again. ${error}`);
    } finally {
      this.setLoading(false);
    }
  }
}
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
