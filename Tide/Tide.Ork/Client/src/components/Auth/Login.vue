<template>
    <div>
        <h1>Login</h1>
        <form @submit.prevent="login">
            <div class="form-group">
                <label for="email">Username</label>
                <input type="text" id="email" v-model="user.username" />
            </div>
            <div class="form-group">
                <label for="password">Password</label>
                <input type="password" id="password" v-model="user.password" />
                <div id="filler"></div>
            </div>
            <div class="form-group">
                <button type="submit">LOGIN</button>
            </div>
            <p>OR</p>
            <p class="link" @click="$parent.changeMode('Register')">Register</p>
        </form>
    </div>
</template>

<script>
export default {
    props: ["user"],
    methods: {
        async login() {
            try {
                this.$loading(true, "Logging in...");
                var loginResult = await this.$store.dispatch("loginAccount", this.user);

                await this.$store.dispatch("finalizeAuthentication", loginResult);
            } catch (error) {
                this.$bus.$emit("show-status", error);
            } finally {
                this.$loading(false, "");
            }
        },
    },
};
</script>

<style lang="scss" scoped>
h1 {
    text-align: center;
}
#filler {
    height: 6px;
}
</style>
