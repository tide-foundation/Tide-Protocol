<template>
  <div id="account" class="full-height f-c auth-page mw-400">
    <div class="back-link font-small" @click="mainStore.authenticationComplete">Back to vendor</div>
    <div class="f-r page-title">
      <div class="spacer line"></div>
      <h1>Your Tide Account</h1>
      <div class="spacer "></div>
    </div>
    <p class="font-small">
      Alter the way you interact with the Tide ecosystem
    </p>
    <div class="button-row">
      <button @click="$router.push('/change-password')">Change password <img src="../assets/img/icons/key.svg" /></button>
      <button disabled>Change Account Servers <img src="../assets/img/icons/ork.svg" /></button>
      <button @click="logout" class="secondary">Logout <img src="../assets/img/icons/logout.svg" /></button>
    </div>
  </div>
</template>

<script lang="ts">
import Base from "@/assets/ts/Base";

export default class Account extends Base {
  logout() {
    this.mainStore.logout();
    this.router.push("/");
  }
}
</script>

<style lang="scss" scoped>
#account {
  h1 {
    margin-bottom: 0px;
  }

  p {
    margin-bottom: 50px;
  }

  #return-button {
    position: absolute;
    top: 5px;
    left: 5px;

    width: 40px !important;
    height: 40px !important;

    img {
      margin: 0px;
    }
  }
}
</style>
