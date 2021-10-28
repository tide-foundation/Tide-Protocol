<template>
  <div id="actions" class="full-height f-c auth-page mw-400">
    <div class="back-link font-small" @click="mainStore.authenticationComplete">Back to vendor</div>
    <div class="f-r page-title">
      <div class="spacer line"></div>
      <h1>
        Welcome back, <strong class="primary"> {{ mainStore.getState.account?.username }}</strong>
      </h1>
      <div class="spacer "></div>
    </div>
    <p class="font-small">
      How would you like to proceed?
    </p>
    <div class="button-row">
      <button @click="$router.push('/account')">Your Account <img src="../assets/img/icons/tide.svg" /></button>
      <button @click="logout" class="secondary">Logout <img src="../assets/img/icons/logout.svg" /></button>
    </div>
  </div>
</template>

<script lang="ts">
import Base from "@/assets/ts/Base";

export default class Actions extends Base {
  logout() {
    this.mainStore.logout();
    this.router.push("/");
  }
}
</script>
