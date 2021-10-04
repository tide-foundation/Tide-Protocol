<template>
  <div id="actions" class="full-height f-c auth-page">
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
      <div class="row">
        <div class="col-12 col-md-4">
          <button class="secondary" @click="backToVendor">
            Back to {{ mainStore.getState.config.vendorName }} <img src="../assets/img/icons/return.svg" />
          </button>
        </div>
        <div class="col-12 col-md-4">
          <button @click="$router.push('/account')">Your Account <img src="../assets/img/icons/tide.svg" /></button>
        </div>
        <div class="col-12 col-md-4">
          <button @click="logout">Logout <img src="../assets/img/icons/logout.svg" /></button>
        </div>
      </div>
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

  backToVendor() {
    this.mainStore.authenticationComplete();
  }
}
</script>
