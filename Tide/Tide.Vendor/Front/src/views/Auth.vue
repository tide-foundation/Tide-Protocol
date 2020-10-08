<template>
    <div id="auth">
        <div class="content">
            <span v-if="!$store.getters.loggedIn">
                <Login v-if="mode == 'Login'"></Login>
                <Register v-else-if="mode == 'Register'"></Register>
                <Forgot v-else-if="mode == 'Forgot'"></Forgot>

                <p>{{ status }}</p>
            </span>
            <Logout v-else></Logout>
        </div>
    </div>
</template>

<script>
import Forgot from "../components/Auth/Forgot.vue";
import Register from "../components/Auth/Register.vue";
import Login from "../components/Auth/Login.vue";
import Logout from "../components/Auth/Logout.vue";
export default {
    components: {
        Login,
        Register,
        Forgot,
        Logout,
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
