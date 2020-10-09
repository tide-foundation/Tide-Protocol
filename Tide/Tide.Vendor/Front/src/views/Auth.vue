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
import Login from "../components/Auth/Login.vue";
import Logout from "../components/Auth/Logout.vue";
export default {
    components: {
        Login,
        Register,

        Logout,
    },

    data() {
        return {
            status: "",
            mode: "Login",
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
