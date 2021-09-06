<template>
  <div id="login" class="full-height f-c">
    <div class="f-r page-title">
      <div class="spacer line"></div>
      <h1>Sign in to RMIT</h1>
      <div class="spacer "></div>
    </div>
    <form @submit.prevent="login">
      <tide-input id="username" v-model="user.username">Username</tide-input>
      <tide-input id="password" v-model="user.password" type="password">Password</tide-input>
      <div class="actions">
        <router-link class="font-small " to="/register">Create an account</router-link>
        <button>Sign In</button>
      </div>
    </form>

    <div id="logging-in" class="full-width full-height f-c"></div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from "vue";
import TideInput from "@/components/Tide-Input.vue";
import mainStore from "@/store/mainStore";

var user = ref<UserPass>({ username: "333", password: "333" });

const login = async () => {
  const account = await mainStore.login(user.value);
  mainStore.redirectToVendor();
};
</script>

<style lang="scss" scoped>
#login {
  text-align: center;
  align-items: stretch;

  .page-title {
    justify-content: space-between;

    .spacer {
      width: 4px;

      &.line {
        background-color: $primary;
        height: 50px;

        transform: translate(-20px, 17px);
      }
    }
  }

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
