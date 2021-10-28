<template>
  <div id="options" class="full-height f-c auth-page">
    <router-link class="back-link font-small" to="/login">Back</router-link>
    <div class="f-r page-title">
      <div class="spacer line"></div>
      <h1>Tide Configuration</h1>
      <div class="spacer "></div>
    </div>
    <p class="font-small mb-30">
      For assurance you're connecting with <span class="primary">{{ descUrl }}</span> <br />using tamper free code, you can change the connection
      <br />
      source to anyone in the Tide network
    </p>
    <form @submit.prevent="changeOrk" class="mb-30">
      <tide-input v-model="newOrk" id="change" type="text">New server address</tide-input>
      <button class="full-width">Change Server</button>
    </form>
  </div>
</template>

<script lang="ts">
import Base from "@/assets/ts/Base";

export default class Login extends Base {
  newOrk: string = "";
  descUrl: string = "";

  created() {
    this.descUrl = window.location.protocol + "//" + window.location.host + "/";
  }
  async changeOrk() {
    window.location.href = `${this.newOrk.replace(/\/$/, "")}/${sessionStorage.getItem("true-url")}`;
  }
}
</script>

<style lang="scss" scoped>
#options {
}
</style>
