<template>
  <div id="account" class="full-height f-c auth-page">
    <div class="f-r page-title">
      <div class="spacer line"></div>
      <h1>Your Tide Account</h1>
      <div class="spacer "></div>
    </div>
    <p class="font-small">
      Alter the way you interact with the Tide ecosystem
    </p>
    <div class="button-row">
      <div class="row">
        <div class="col-12 col-md-4">
          <button disabled @click="$router.push('/change-password')">Change password <img src="../assets/img/icons/key.svg" /></button>
        </div>
        <div class="col-12 col-md-4">
          <button disabled>Change ORKs <img src="../assets/img/icons/ork.svg" /></button>
        </div>
        <!-- <div class="col-12 col-md-4">
          <button disabled>Account History <img src="../assets/img/icons/history.svg" /></button>
        </div> -->
        <div class="col-12 col-md-4">
          <button @click="logout">Logout <img src="../assets/img/icons/logout.svg" /></button>
        </div>
      </div>
    </div>

    <button @click="mainStore.authenticationComplete" id="return-button" class="f-c secondary"><img src="../assets/img/icons/return.svg" /></button>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from "vue";
import mainStore from "@/store/mainStore";
import router from "@/router/router";
const logout = () => {
  mainStore.logout();
  router.push("/");
};
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
