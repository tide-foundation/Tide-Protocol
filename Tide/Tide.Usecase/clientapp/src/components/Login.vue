<template>
  <section>
    <span> <i class="fa fa-sign-in" style="margin-bottom:20px"></i>Login </span>

    <form @submit.prevent="login">
      <div class="form-element">
        <label class="login-form-label" for="login-username">Username</label>
        <input
          id="login-username"
          name="username"
          type="text"
          class="login-form-input login-form-input-common required"
          v-model="user.username"
          title="Please enter a valid username."
          placeholder="Username"
        />
      </div>
      <div class="form-element">
        <label class="login-form-label" for="password">Password</label>
        <input
          id="password"
          name="password"
          type="password"
          class="login-form-input login-form-input-common required"
          v-model="user.password"
          placeholder="Password"
        />
      </div>
      <div class="form-element">
        <input
          type="submit"
          class="login-form-submit login-form-input-common"
          value="Login"
        />
      </div>
    </form>
    <div class="clearfix">
      <span class="sign-up pull-left">
        Not a Member?
        <a
          href="#"
          @click="$bus.$emit('changeRegisterMode', true)"
          class="activate-section"
          data-section="register-section"
          >Sign up now</a
        >
      </span>
    </div>
  </section>
</template>

<script>
import TideInput from "../components/TideInput.vue";

export default {
  name: "home",
  components: {
    TideInput
  },
  data() {
    return {
      user: {
        username: "",
        password: "12345678"
      }
    };
  },
  methods: {
    login() {
      this.$loading(true, "Logging you in...");

      // Artificial wait to allow loading overlay to react
      setTimeout(async () => {
        try {
          var user = {};
          if (false) {
            user = {
              username: "user285366@email.com",
              account: "tidegwhfxvls",
              nodes: [
                {
                  ork_node: "tideorkxxxx2",
                  ork_url: "https://tide-ork-02.azurewebsites.net/"
                },
                {
                  ork_node: "tideorkxxxx1",
                  ork_url: "https://tide-ork-01.azurewebsites.net/"
                }
              ],
              keys: {
                tide: {
                  pub: "ungathered",
                  priv: "5HsH3U6DG8VAmVHdyPcekLkuRQ1LUmsiaBZMWYSeE4uXe3chcTT"
                },
                vendor: {
                  pub: "AIqX6YNpQPBzCQ2GUg==",
                  priv: "AoqX6YNpQPBzErzaEQ=="
                }
              },
              tide: 0,
              trustee: false
            };
          } else {
            // const tideCredentialResult = await this.$tide.getTideCredentials(
            //   this.user.username,
            //   this.user.password
            // );

            var loginResult = await this.$tide.login(this.user.username, this.user.password);
            console.log(loginResult.key)
            this.$loading(true, "Gathering Vendor account...");
            // const vendorResult = await this.$tide.getVendorCredentials(
            //   tideCredentialResult.userNodes,
            //   tideCredentialResult.account
            // );
            user = {
              username: this.user.username,
              account: "tidegwhfxvls",
              aes:loginResult.key,
              nodes: [
                {
                  ork_node: "tideorkxxxx2",
                  ork_url: "https://tide-ork-02.azurewebsites.net/"
                },
                {
                  ork_node: "tideorkxxxx1",
                  ork_url: "https://tide-ork-01.azurewebsites.net/"
                }
              ],
              keys: {
                tide: {
                  pub: "ungathered",
                  priv: "5HsH3U6DG8VAmVHdyPcekLkuRQ1LUmsiaBZMWYSeE4uXe3chcTT"
                },
                vendor: {
                  pub: "AIqX6YNpQPBzCQ2GUg==",
                  priv: "AoqX6YNpQPBzErzaEQ=="
                }
              },
              tide: 0,
              trustee: false
            };
          }

          this.$store.commit("storeUser", user);

          this.$bus.$emit("show-message", "You have logged in successfully");
          this.$bus.$emit("showLoginModal", false);
          this.$loading(false, "");
          this.$authAction();
        } catch (thrownError) {
         this.$bus.$emit("show-error", thrownError.response.text != null ? thrownError.response.text : thrownError.status);
          this.$loading(false, "");
        }
      }, 100);
    }
  }
};
</script>
