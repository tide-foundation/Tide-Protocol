<template>
  <div id="auth">
    <div id="content">
      <Login v-if="mode == 'Login'"></Login>
      <Register v-else-if="mode == 'Register'"></Register>
      <Forgot v-else-if="mode == 'Forgot'"></Forgot>
      <p>{{status}}</p>
    </div>
  </div>
</template>

<script>
import Forgot from "@/components/Forgot.vue";
import Register from "@/components/Register.vue";
import Login from "@/components/Login.vue";
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
    width: 100%;
    height: 100vh;
    display: flex;
    justify-content: center;
    align-items: center;

    #content {
        display: flex;
        justify-content: center;
        align-items: center;
        flex-direction: column;
        width: 98%;
        max-width: 400px;
    }
}
</style>
