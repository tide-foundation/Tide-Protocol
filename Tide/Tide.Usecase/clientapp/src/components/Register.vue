<template>
  <section>
    <span>
      <i class="fa fa-sign-in" style="margin-bottom:20px"></i>Register
    </span>

    <form @submit.prevent="register">
      <div class="form-element">
        <label class="login-form-label">Username</label>
        <input
          required
          name="username"
          type="text"
          class="login-form-input login-form-input-common required"
          v-model="user.username"
          title="* Please enter a valid username."
          placeholder="Username"
        />
      </div>
      <div class="form-element">
        <label class="login-form-label">Password</label>
        <input
          required
          name="password"
          type="password"
          v-model="user.password"
          class="login-form-input login-form-input-common required"
          placeholder="Password"
        />
      </div>

      <div class="form-element">
        <input
          type="submit"
          class="login-form-submit login-form-input-common"
          value="Register"
        />
      </div>
    </form>
    <div class="error">{{ error }}</div>
    <div class="clearfix">
      <span class="sign-up pull-left">
        Already a member?
        <a
          href="#"
          @click="$bus.$emit('changeRegisterMode', false)"
          class="activate-section"
          data-section="register-section"
          >Sign in</a
        >
      </span>
    </div>
  </section>
</template>

<script>
// @ is an alias to /src
import TideInput from "../components/TideInput.vue";

export default {
  name: "home",
  components: {
    TideInput
  },
  data() {
    return {
      error: "",
      user: {
        username: `User${Math.floor(Math.random() * 1000000)}`,
        password: "12345678"
      }
    };
  },
  methods: {
    register() {
      if (this.user.password.length < 4)
        return (this.error = "Password requires at least 4 characters.");

      this.$loading(true, "Creating Tide account...");

      // Artificial wait to allow loading overlay to react
      setTimeout(async () => {
        try {
          // var signUp = await this.$tide.register(
          //   this.user.username,
          //   this.user.password,
          //   "tmp@tide.org"
          // );
  
          var signUp = await this.$tide.register(this.user.username, this.user.password, "tmp@tide.org");
console.log(signUp.key)
          // await this.$helper.sleep(2000);
          const registerTideResult = {
            privateKey: "5J9mJizKfGrFdnSZNswomzTeoVoLi3649YdrHGwT3EQTCTPLf3Z",
            publicKey: "EOS54P6fHzGyr1SSTYusmA6BhuEhJYuY5uZUHqQBzMeadk2jdbJey",
            accountName: "tidexhsuridk"
          };

          this.$loading(
            true,
            "Generating your vendor account and distributing your key fragments..."
          );

          await this.$helper.sleep(2000);
          const registerVendorResult = {
            cvkPublicKey:
              "ANH+mAyO2/f9i53isvp6BG8mEcJyy2C5CvDlexRLHR+jOSJwIOFb2d3Kp5kiDlSzjcHn4ByjIuxzwlWrRTg2h/GUp8OdkQVviV1KUhIajEqtdMJVAZ7bCpn0nCzgmLG40A==",
            cvkPrivateKey:
              "AtH+mAyO2/f9i53isvp6BG8mEcJyy2C5CvDlexRLHR+jOSJwIOFb2d3Kp5kiDlSzjcHn4ByjIuxzwlWrRTg2h/E4HOBbB5EIoFFjxIcMQZ2L4uR5qCPVn0jv9w0sMAubmQ=="
          };

          this.$store.commit("storeUser", {
            username: this.user.username,
            account: registerTideResult.accountName,
           aes:signUp.key,
            keys: {
              tide: {
                pub: registerTideResult.publicKey,
                priv: registerTideResult.privateKey
              },
              vendor: {
                pub: registerVendorResult.cvkPublicKey,
                priv: registerVendorResult.cvkPrivateKey
              }
            },
            trustee: false,
            tide: 0
          });

          this.$bus.$emit("show-message", "You have registered successfully");
          this.$bus.$emit("showLoginModal", false);
          this.$loading(false, "");
          this.$authAction();
        } catch (thrownError) {
          this.$bus.$emit("show-error",thrownError.response != null && thrownError.response.text != null ? thrownError.response.text : thrownError.status);
          this.$loading(false, "");
        }
      }, 100);
    }
  }
};
</script>

<style scoped>
.error {
  color: red;
}
</style>
