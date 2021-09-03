<template>
  <div id="login" class="full-height f-c">
    <h1>Sign in to RMIT</h1>
    <tide-input id="username" v-model="user.username">Username</tide-input>
    <tide-input id="password" v-model="user.password" type="password">Password</tide-input>
    <div class="actions">
      <router-link class="font-small " to="/register">Create an account</router-link>
      <button>Sign In</button>
    </div>

    <div id="logging-in" class="full-width full-height f-c"></div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from "vue";
import TideInput from "@/components/Tide-Input.vue";

var user = ref<UserPass>({ username: "AwesomeSurfer69", password: "some password" });
</script>

<style lang="scss" scoped>
#login {
  text-align: center;
  align-items: stretch;

  h1 {
    font-size: 2rem;
    margin-bottom: 40px;
  }

  .actions {
    a {
      //  margin-left: 10px;
    }

    button {
      width: 100%;
    }
  }

  #logging-in {
    pointer-events: none;
    position: absolute;
    //background-color: $background;
    left: 0;
    top: 0;
  }
}
</style>
