<template>
    <div>
        <h1>Register</h1>
        <form @submit.prevent="register">
            <div class="form-group">
                <label for="email">Username</label>
                <input type="text" id="email" v-model="user.username" />
            </div>
            <div class="form-group">
                <label for="password">Password</label>
                <input type="password" id="password" v-model="user.password" />
                <password-meter :password="user.password" />
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
import passwordMeter from "vue-simple-password-meter";
import request from "superagent";
export default {
    props: ["user"],
    components: { passwordMeter },
    methods: {
        async register() {
            try {
                this.$loading(true, "Registering...");
                var signUpResult = await this.$tide.register(this.user.username, this.user.password, "admin@admin.com", this.$store.getters.tempOrksToUse);

                var userData = {
                    id: signUpResult.vuid.toString(),
                    vendorKey: signUpResult.vendorKey.toString(),
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
.po-password-strength-bar {
    border-radius: 0px;
    transition: all 0.2s linear;
    margin-top: -9px;
    margin-bottom: 10px;
    width: calc(100% + 9px);
}
</style>