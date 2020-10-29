<template>
  <div>
    <h1>Change Password</h1>
    <form @submit.prevent="changePassword">
      <div class="form-group">
        <label for="password">Password</label>
        <input type="password" id="password" v-model="user.password" />
      </div>
      <div class="form-group">
        <label for="new-password">New Password</label>
        <input type="password" id="new-password" v-model="user.newPassword" />
        <password-meter :password="user.newPassword" />
      </div>

      <div class="form-group">
        <button type="submit">CHANGE PASSWORD</button>
      </div>
      <p>OR</p>
      <p class="link" @click="$parent.changeMode('Overview')">Back to Dashboard</p>
    </form>
  </div>
</template>

<script>
import passwordMeter from "vue-simple-password-meter";
export default {
  components: { passwordMeter },
  data() {
    return {
      user: {
        password: "",
        newPassword: "",
      },
    };
  },
  methods: {
    async changePassword() {
      try {
        this.$loading(true, "Changing password...");

        var signUpResult = await this.$store.dispatch("changePassword", this.user);
        this.$bus.$emit("show-status", "Your password has been changed");
      } catch (error) {
        this.$bus.$emit("show-error", error);
      } finally {
        this.$loading(false, "");
      }
    },
  },
};
</script>

<style></style>
