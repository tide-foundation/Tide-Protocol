<template>
  <div id="auth">
    <div class="content">
      <span v-if="!$store.getters.loggedIn">
        <Login :user="user" v-if="mode == 'Login'"></Login>
        <Register :user="user" v-else-if="mode == 'Register'"></Register>

        <p>{{ status }}</p>
      </span>
      <Logout v-else></Logout>
    </div>
  </div>
</template>

<script>
import Register from "../components/Auth/Register.vue";
import OAuthRegister from "../components/Auth/OAuthRegister.vue";
import Login from "../components/Auth/Login.vue";
import Logout from "../components/Auth/Logout.vue";
export default {
  components: {
    Login,
    Register,
    OAuthRegister,
    Logout,
  },

  data() {
    return {
      status: "",
      mode: "Register",
      user: {
        email: "",
        password: "password",
      },
    };
  },
  created() {
    this.user.email = this.$store.getters.email;
  },
  methods: {
    setStatus(msg) {
      this.status = msg;
    },
<<<<<<< Updated upstream:Tide/Tide.Vendor/Front/src/views/Auth.vue

    data() {
        return {
            status: "",
            mode: "Login",
            user: {
                username: "",
                password: "password",
            },
        };
    },
    created() {
        this.user.username = this.$store.getters.username;
=======
    changeMode(mode) {
      this.mode = mode;
>>>>>>> Stashed changes:Tide/Tide.Vendor/Client/src/views/Auth.vue
    },
    setUser(user) {
      this.$store.commit("SET_USER", user);
    },
  },
};
</script>

<style lang="scss" scoped>
#auth {
}
</style>
