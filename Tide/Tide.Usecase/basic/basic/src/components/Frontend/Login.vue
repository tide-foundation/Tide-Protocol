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
      <p>OR</p>
      <p class="link" @click="$parent.changeMode('Forgot')">Forgot Password</p>
    </form>
  </div>
</template>

<script>
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
        async login() {
            try {
                this.$loading(true, "Logging in...");
                var user = await this.$tide.loginV2(this.user.email, this.user.password, this.$store.getters.tempOrksToUse);
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