<template>
  <div id="login" class="full-height f-c auth-page">
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
      <div class="full-width f-r-r mt-10 font-small">
        <router-link to="/forgot">Forgot password?</router-link>
      </div>
    </form>
    <div id="to-account-checkbox" class="f-r font-small" :class="{ checked: goToAccount }" @click="goToAccount = !goToAccount">
      <div>Continue to account</div>
      <div id="checkbox"></div>
    </div>
  </div>
</template>

<script lang="ts">
import Base from "@/assets/ts/Base";

export default class Login extends Base {
  goToAccount: boolean = false;
  user: UserPass = { username: "", password: "", emails: [] };

  async login() {
    try {
      this.setLoading(true);
      await this.mainStore.login(this.user);
      // Go to form if data is available
      if (this.mainStore.getState.config.formData != null) return this.router.push("/form");
      else if (this.goToAccount) return this.router.push("/account");
      else this.mainStore.authenticationComplete();
    } catch (error) {
      this.showAlert("error", `Failed to login: ${error}`);

      this.setLoading(false);
    }
  }
}
</script>

<style lang="scss" scoped>
#login {
  #to-account-checkbox {
    cursor: pointer;
    position: absolute;
    top: 5px;
    right: 5px;
    align-items: center;
    #checkbox {
      margin-left: 7px;
      width: 12px;
      height: 12px;
      border-radius: 3px;
      border: 1px solid #0072c6;
      background-color: transparent;
      transform: translate(0px, 1px);
    }
    &.checked {
      #checkbox {
        background-color: #0072c6;
      }
    }
  }
}
</style>
