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

<script lang="ts">
import Base from "@/assets/ts/Base";
import { FORGOT_PASSWORD_USERNAME } from "@/assets/ts/Constants";

export default class Forgot extends Base {
  user: UserPass = { username: "", password: "", emails: [] };
  async sendEmails() {
    try {
      sessionStorage.setItem(FORGOT_PASSWORD_USERNAME, this.user.username);
      this.setLoading(true);

      await this.mainStore.sendRecoveryEmails(this.user.username);
      this.router.push("Reconstruct");
    } catch (error) {
      this.showAlert("error", `Failed to send emils: ${error}`);
    } finally {
      this.setLoading(false);
    }
  }
}
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
