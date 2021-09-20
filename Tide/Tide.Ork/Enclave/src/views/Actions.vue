<template>
  <div id="actions" class="full-height f-col">
    <div class="f-r page-title">
      <div class="spacer line"></div>
      <h1>
        You are logged in as <strong class="primary"> {{ mainStore.getState.account?.username }}</strong
        >. Is this you?
      </h1>
      <div class="spacer "></div>
    </div>

    <button @click="logout">Logout</button>

    <button @click="backToVendor">Back to {{ mainStore.getState.config.vendorName }}</button>
  </div>
</template>

<script setup lang="ts">
import mainStore from "@/store/mainStore";
import router from "@/router/router";
const logout = () => {
  mainStore.logout();
  router.push("/");
};

const backToVendor = () => {
  mainStore.authenticationComplete();
};
</script>

<style lang="scss" scoped>
#actions {
  min-width: 300px;
  max-width: 1150px;
  min-height: 400px;
}
</style>
