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

<script lang="ts">
import Base from "@/assets/ts/Base";

export default class ChangePassword extends Base {
  newPassword: NewPassword = { password: ``, confirm: "" };

  async change() {
    try {
      this.setLoading(true);
      await this.mainStore.changePassword(this.newPassword);

      this.showAlert("success", "Your password has been changed");

      this.router.push("/account");
    } catch (error) {
      this.showAlert("error", `Failed changing your password. ${error}`);
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
