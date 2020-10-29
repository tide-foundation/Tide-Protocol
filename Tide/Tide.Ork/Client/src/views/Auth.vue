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
          <Forgot :user="user" v-else-if="mode == 'Forgot'"></Forgot>
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
import Forgot from "../components/Auth/Forgot.vue";
export default {
  components: {
    ChangeOrk,
    SelectOrks,
    LoginUsername,
    LoginPassword,
    Register,
    Logout,
    Forgot,
  },

  data() {
    return {
      status: "",
      mode: "LoginUsername",
      user: {
        username: "matt@tide.org",
        password: "password",
        confirm: "password",
        goToDashboard: false,
        homeOrk: "http://172.26.17.60:8081/",
        recoveryEmails: ["matt@tide.org"],
        selectedOrks: [],
      },
    };
  },
  created() {
    this.user.username = this.$store.getters.username; // Populate from store with random name
    this.user.selectedOrks = this.$store.getters.tempOrksToUse;
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

<style lang="scss" scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.5s;
}
.fade-enter, .fade-leave-to /* .fade-leave-active below version 2.1.8 */ {
  opacity: 0;
}
</style>
