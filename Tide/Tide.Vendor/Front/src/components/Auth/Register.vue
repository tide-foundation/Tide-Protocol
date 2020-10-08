<template>
    <div>
        <h1>Register</h1>
        <form @submit.prevent="register">
            <div class="form-group">
                <label for="email">Email</label>
                <input type="text" id="email" v-model="user.email" />
            </div>
            <div class="form-group">
                <label for="password">Password</label>
                <input type="password" id="password" v-model="user.password" />
            </div>
            <div class="form-group">
                <button type="submit">REGISTER</button>
            </div>
            <p>OR</p>
            <p class="link" @click="$parent.changeMode('Login')">Login</p>
        </form>
    </div>
</template>

<script>
import request from "superagent";
export default {
    data() {
        return {
            user: {
                email: "thrakmar@gmail.com",
                password: "password",
            },
        };
    },
    methods: {
        async register() {
            try {
                this.$loading(true, "Registering...");
                var signUpResult = await this.$tide.register(this.user.email, this.user.password, this.user.email, this.$store.getters.tempOrksToUse);

                var userData = {
                    id: signUpResult.vuid.toString(),
                    publicKey: signUpResult.publicKey,
                };

                await request.post(`${this.$store.getters.vendorUrl}/account`).send(userData);

                this.$parent.setUser(signUpResult);
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