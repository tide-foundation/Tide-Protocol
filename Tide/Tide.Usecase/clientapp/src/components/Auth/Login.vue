<template>
  <span class="auth-form">
    <p>
      Login to access your
      <span style="color:#29AAFC">Future Places</span> dashboard.
    </p>
    <p>
      Did you
      <a href="#" class="link" @click="$parent.changeMode('Forgot Password')">Forget your password?</a>
    </p>

    <section>
      <form @submit.prevent="login">
        <input v-model="user.email" placeholder="Email" type="email" />
        <input v-model="user.password" placeholder="Password" type="password" />
        <button class="gradiant-button">
          LOGIN &nbsp;&nbsp;
          <i class="fa fa-sign-in"></i>
        </button>
      </form>

      <a href="#" @click="$parent.changeMode('Register')">Need an account?</a>
    </section>
  </span>
</template>


<script>
export default {
    props: ["user"],
    data: function() {
        return {
            // user: {
            //     // email: `${Math.floor(Math.random() * 1000000)}@gmail.com`,
            //     // password: "mLwRGT7uY6tbsoB41",
            //     // confirm: "mLwRGT7uY6tbsoB41"
            //     // email: `matt@gmail.com`,
            //     // password: "Ff09&QcBWEXk",
            //     // confirm: "Ff09&QcBWEXk"
            //     email: ``,
            //     password: "",
            //     confirm: ""
            // }
        };
    },

    methods: {
        login() {
            this.$loading(true, "Logging you in...");

            // Artificial wait to allow loading overlay to react
            setTimeout(async () => {
                try {
                    var login = await this.$tide.loginV2(this.user.email, this.user.password, this.$parent.getTemporaryOrkList());

                    this.$loading(true, "Gathering Vendor account...");

                    var userData = {
                        id: login.vuid.toString(),
                        cvkPub: login.publicKey
                    };

                    var result = await this.$http.get(`${this.$tide.serverUrl}/account/${userData.id}`);

                    this.$parent.setBearer(result.data);

                    this.$store.commit("storeUser", userData);

                    this.$bus.$emit("show-message", "You have logged in successfully");
                    this.$bus.$emit("showLoginModal", false);
                    this.$loading(false, "");

                    this.$router.push("/profile");
                } catch (thrownError) {
                    if (thrownError.response != null && thrownError.response.text != null) {
                        if (thrownError.response.text.includes("Bad Request")) {
                            this.$bus.$emit("show-error", "Invalid Credentials");
                        } else {
                            this.$bus.$emit("show-error", thrownError.response.text);
                        }
                    } else {
                        this.$bus.$emit("show-error", thrownError);
                    }
                    this.$loading(false, "");
                }
            }, 100);
        }
    }
};
</script>

<style>
</style>