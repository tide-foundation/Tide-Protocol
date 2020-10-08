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
                <label>Ork Count</label>
                <select v-model="orkCount">
                    <option v-for="i in 20" :key="i" :value="i">{{ i }}</option>
                </select>
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
                email: "john@wick.com",
                password: "password",
            },
            orkCount: 10,
        };
    },
    created() {
        try {
            this.orkCount = localStorage.getItem("count");
        } catch (error) {}
    },
    watch: {
        orkCount: function (val) {
            localStorage.setItem("count", val);
        },
    },
    methods: {
        async register() {
            try {
                this.$loading(true, "Registering...");
                const orks = await this.$store.dispatch("getTempOrks", this.orkCount);

                var signUpResult = await this.$tide.register(this.user.email, this.user.password, this.user.email, orks);

                var userData = {
                    id: signUpResult.vuid.toString(),
                    vendorKey: signUpResult.vendorKey.toString(),
                };

                await request.post(`${this.$store.getters.vendorUrl}/account`).send(userData);

                var jwt = await request.get(`${this.$store.getters.vendorUrl}/account/${userData.id}`);

                var decrypt = this.$tide.decryptToken(jwt.body);

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