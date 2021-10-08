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
        <button id="plus-button" v-tippy="'Add additional email addresses'" @click="showEmails = true" type="button">
          +
          <div v-if="user.emails.length > 1" id="plus-button-number" class="font-small">{{ user.emails.length }}</div>
        </button>
      </div>

      <div class="actions">
        <router-link class="font-small " to="/login">Have an account?</router-link>
        <button>Register</button>
      </div>
    </form>

    <div class="loading-graphic full-width full-height f-c"></div>
    <form @submit.prevent="showEmails = false" id="email-form" v-if="showEmails">
      <div id="modal" class="f-c">
        <button id="emails-back" type="submit" class="f-c"><img src="../../assets/img/icons/back-dark.svg" /></button>
        <h2>Email Addresses</h2>
        <p class="font-small">
          Attach additional email addresses to your <span class="primary">Tide</span> account. Each email address adds additional security.
        </p>

        <div id="emails" class="full-width">
          <div v-for="(email, i) in user.emails" :key="i" class="email-container  f-r">
            <tide-input
              class="email-input"
              :id="`email${i + 1}`"
              v-model="user.emails[i]"
              type="email"
              :required="true"
              :style="{ opacity: i == user.emails.length - 1 && highlightlastEmail && user.emails.length > 1 ? 0.5 : 1 }"
              >Email {{ i + 1 }}</tide-input
            >
            <div v-if="i == user.emails.length - 1 && user.emails.length > 1" class="invisible">...</div>
            <button v-if="i == user.emails.length - 1 && user.emails.length > 1" @click="user.emails.pop()" class="del-email-icon f-c delete">
              <img src="../../assets/img/icons/times.svg" />
            </button>
          </div>

          <button class="full-width font-small" @click="user.emails.push('')" type="button">Add email</button>
        </div>
        <!-- <div class="f-r full-width mt-10">
          <button class="full-width font-small" @click="user.emails.push('')" type="button">Add email</button>
        </div> -->
      </div>
    </form>
  </div>
</template>

<script lang="ts">
import Base from "@/assets/ts/Base";

export default class Register extends Base {
  user: UserPass = { username: ``, password: "", emails: [""] };
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
  #email-form {
    position: absolute;
    width: 100%;
    height: 100%;

    #modal {
      position: absolute;
      width: 100%;
      // margin-top: 60px;
      height: calc(100% + 58px);
      background-color: $background;
      border: 1px solid #eaebf5;
      box-shadow: rgba(0, 0, 0, 0.16) 0px 10px 36px 0px, rgba(0, 0, 0, 0.06) 0px 0px 0px 1px;
      padding: 10px;

      #emails-back {
        background-color: transparent;
        border: 1px solid transparent;
        position: absolute;
        left: 8px;
        top: 7px;
        width: 40px;
        height: 40px;
        cursor: pointer;
        padding: 8px 0px;
        img {
          margin: 0px;

          height: 30px;
        }

        &:hover {
          background-color: #d8ced3;
          border: 1px solid #c0b8bc;
        }
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

        .email-input {
          width: 100%;
        }

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
}

#plus-button {
  position: relative;

  #plus-button-number {
    position: absolute;
    top: 3px;
    left: 4px;
    color: white;
  }
}
.email-container {
  .del-email-icon {
    width: 70px;

    padding: 0px;
    img {
      margin: 0px;
    }
  }
}
</style>
