<template>
    <div>
        <h1>Login</h1>
        <form @submit.prevent="login">
            <div class="form-group">
                <label for="email">Email</label>
                <input type="text" id="email" v-model="user.email" />
            </div>
            <div class="form-group">
                <label for="password">Password</label>
                <input type="password" id="password" v-model="user.password" />
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
                var user = await this.$tide.login(this.user.email, this.user.password);
                this.$parent.setUser(user);
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
</style>