<template>
  <div id="auth">
    <div class="content">
      <Login v-if="mode == 'Login'"></Login>
      <Register v-else-if="mode == 'Register'"></Register>
      <Forgot v-else-if="mode == 'Forgot'"></Forgot>
      <p>{{status}}</p>
    </div>
  </div>
</template>

<script>
import Forgot from "./Forgot.vue";
import Register from "./Register.vue";
import Login from "./Login.vue";
export default {
    components: {
        Login,
        Register,
        Forgot,
    },
    created() {
        this.$store.dispatch("getDeals");
    },
    data() {
        return {
            status: "",
            mode: "Login",
        };
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
#auth {
}
</style>
