<template>
  <div>
    <h2>Change Password</h2>
    <form @submit.prevent="changePassword">
      <input type="password" required v-model="user.password" placeholder="Current Password" />
      <div class="mt-10 password-row">
        <input type="password" required v-model="user.newPassword" placeholder="New Password" />
        <input type="password" required v-model="user.confirm" placeholder="Confirm" />
      </div>

      <password-meter :password="user.newPassword" class="mt-10" />

      <div class="action-row mt-50">
        <p @click="$parent.changeMode('Overview')">Back</p>
        <button type="submit">Change Password</button>
      </div>
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
        confirm: "",
      },
    };
  },
  methods: {
    async changePassword() {
      if (this.user.newPassword != this.user.confirm) return this.$bus.$emit("show-error", "Passwords do not match");
      this.$loading(true, "Changing password...");
      this.$nextTick(async () => {
        try {
          var signUpResult = await this.$store.dispatch("changePassword", this.user);
          this.$bus.$emit("show-status", "Your password has been changed");
          this.$parent.changeMode("Overview");
        } catch (error) {
          this.$bus.$emit("show-error", error);
        } finally {
          this.$loading(false, "");
        }
      });
    },
  },
};
</script>

<style></style>
