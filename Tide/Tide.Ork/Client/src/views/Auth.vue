<template>
  <div id="auth">
    <div class="content">
      <span v-if="!$store.getters.loggedIn">
        <transition name="fade" mode="out-in">
          <LoginUsername :user="user" v-if="mode == 'LoginUsername'"></LoginUsername>
          <LoginPassword :user="user" v-if="mode == 'LoginPassword'"></LoginPassword>
          <ChangeOrk :user="user" v-if="mode == 'ChangeOrk'"></ChangeOrk>
          <SelectOrks :user="user" v-if="mode == 'SelectOrks'"></SelectOrks>
          <Register :user="user" v-else-if="mode == 'Register'"></Register>
          <ForgotSend :user="user" v-else-if="mode == 'ForgotSend'"></ForgotSend>
          <ForgotReconstruct :user="user" v-else-if="mode == 'ForgotReconstruct'"></ForgotReconstruct>
          <ForgotReset :user="user" v-else-if="mode == 'ForgotReset'"></ForgotReset>
        </transition>

        <p>{{ status }}</p>
      </span>
      <Logout v-else></Logout>
    </div>
  </div>
</template>

<script>
import ChangeOrk from "../components/Auth/ChangeOrk.vue";
import SelectOrks from "../components/Auth/SelectOrks.vue";
import Register from "../components/Auth/Register.vue";
import LoginUsername from "../components/Auth/LoginUsername.vue";
import LoginPassword from "../components/Auth/LoginPassword.vue";
import Logout from "../components/Auth/Logout.vue";
import ForgotSend from "../components/Auth/ForgotSend.vue";
import ForgotReconstruct from "../components/Auth/ForgotReconstruct.vue";
import ForgotReset from "../components/Auth/ForgotReset.vue";
export default {
  components: {
    ChangeOrk,
    SelectOrks,
    LoginUsername,
    LoginPassword,
    Register,
    Logout,
    ForgotSend,
    ForgotReconstruct,
    ForgotReset,
  },

  data() {
    return {
      status: "",
      mode: "Register",
      user: {
        username: this.$store.getters.debug ? `matt${Math.round(Math.random() * (90000 - 1) + 1)}@tide.org` : "",
        password: this.$store.getters.debug ? "password" : "",
        confirm: this.$store.getters.debug ? "password" : "",
        goToDashboard: false,
        homeOrk: "http://172.26.17.60:8081/",
        recoveryEmails: [this.$store.getters.debug ? "matt@tide.org" : ""],
        selectedOrks: [],
        frags: [],
      },
    };
  },
  created() {
    this.user.selectedOrks = this.$store.getters.orks;
  },
  methods: {
    setStatus(msg) {
      this.status = msg;
    },
    changeMode(mode) {
      this.mode = mode;
    },
    setUser(user) {
      this.$store.commit("SET_USER", user);
    },
  },
};
</script>

<style lang="scss" scoped></style>
