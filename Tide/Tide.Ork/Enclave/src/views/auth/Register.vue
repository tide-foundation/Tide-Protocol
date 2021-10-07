<template>
  <div id="register" class="full-height f-c auth-page">
    <div class="f-r page-title">
      <div class="spacer line"></div>
      <h1>Register to RMIT</h1>
      <div class="spacer "></div>
    </div>
    <form @submit.prevent="register">
      <tide-input id="username" v-model="user.username">Username</tide-input>
      <tide-input id="password" v-model="user.password" type="password">Password</tide-input>
      <div class="f-r full-width">
        <tide-input class="full-width mr-10" id="email" v-model="user.emails[0]" type="email">Email 1</tide-input>
        <button v-tippy="'Add additional email addresses'" @click="showEmails = true" type="button">+</button>
      </div>

      <div class="actions">
        <router-link class="font-small " to="/login">Have an account?</router-link>
        <button>Register</button>
      </div>
    </form>

    <div class="loading-graphic full-width full-height f-c"></div>

    <div id="modal" v-if="showEmails" class="f-c">
      <div id="emails-back" @click="showEmails = false"><img src="../../assets/img/icons/back-dark.svg" /></div>
      <h2>Email Addresses</h2>
      <p class="font-small">
        Attach additional email addresses to your <span class="primary">Tide</span> account. Emails are used when you forget your password. Each email
        address adds additional security.
      </p>
      <div id="emails" class="full-width">
        <tide-input
          v-for="(email, i) in user.emails"
          :key="i"
          :id="`email${i + 1}`"
          v-model="user.emails[i]"
          type="email"
          :style="{ opacity: i == user.emails.length - 1 && highlightlastEmail && user.emails.length > 1 ? 0.5 : 1 }"
          >Email {{ i + 1 }}</tide-input
        >
      </div>
      <div class="f-r full-width mt-10">
        <button class="full-width font-small" @click="user.emails.push('')">Add email</button>
        <div v-if="user.emails.length > 1" class="invisible">...</div>
        <button
          v-if="user.emails.length > 1"
          class="full-width delete font-small"
          @click="user.emails.pop()"
          @mouseover="highlightlastEmail = true"
          @mouseleave="highlightlastEmail = false"
        >
          Remove email {{ user.emails.length }}
        </button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import Base from "@/assets/ts/Base";

export default class Register extends Base {
  user: UserPass = { username: ``, password: "", emails: [] };
  showEmails = false;
  highlightlastEmail = false;
  async register() {
    try {
      this.setLoading(true);

      await this.mainStore.registerAccount(this.user);
      this.mainStore.authenticationComplete();
    } catch (error) {
      this.showAlert("error", `Failed to register: ${error}`);

      this.setLoading(false);
    }
  }
}
</script>

<style lang="scss" scoped>
#register {
  position: relative;
  #modal {
    position: absolute;
    width: 100%;
    height: 100%;
    background-color: $background;
    border: 1px solid #eaebf5;
    box-shadow: rgba(0, 0, 0, 0.16) 0px 10px 36px 0px, rgba(0, 0, 0, 0.06) 0px 0px 0px 1px;
    padding: 10px;

    #emails-back {
      position: absolute;
      left: 15px;
      top: 5px;
      width: 30px;
      height: 30px;
      cursor: pointer;
    }

    h2 {
      font-size: 1.3rem;
    }

    #emails {
      padding: 10px 0;
      flex-grow: 1;
      // height: 270px;
      overflow-y: auto;
      border-top: 1px solid #eaebf5;
      border-bottom: 1px solid #eaebf5;

      &::-webkit-scrollbar {
        width: 0.3em;
      }

      &::-webkit-scrollbar-track {
        background: rgb(231, 231, 231);
      }

      &::-webkit-scrollbar-thumb {
        background-color: darkgrey;
      }
    }
  }
}
</style>
