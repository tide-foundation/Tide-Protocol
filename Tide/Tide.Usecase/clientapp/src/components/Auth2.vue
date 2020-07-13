<template>
  <div id="content" v-if="show">
    <div id="logo">
      <img src="../assets/img/FuturePlacesLogo.png" alt="logo" />
    </div>
    <img id="deco-1" src="../assets/img/auth/deco-1.png" alt="decoration" />
    <img id="deco-2" src="../assets/img/auth/deco-2.png" alt="decoration" />
    <button id="auto-fill" @click="autoFill">Auto Fill</button>
    <div id="auth-box">
      <div id="title">
        <h1>{{ loginMode }}</h1>
        <img id="divider" src="../assets/img/divider.png" alt="divider" />

        <span v-if="loginMode == 'Login'" key="1">
          <p>Login to access your <span style="color:#29AAFC">Future Places</span> dashboard.</p>
          <p>
            Did you
            <a href="#" class="link" @click="loginMode = 'Forgot Password'">Forget your password?</a>
          </p>
        </span>

        <span v-if="loginMode == 'Register'" key="2">
          <p v-if="step == 0">
            Create your account to access all of <span style="color:#29AAFC">Future Places</span>
            great features.
          </p>
          <p v-if="step == 1">We have randomly selected 3 Ork nodes to be used in the secret sharing of your <span style="color:#29AAFC">Tide account</span>. Feel free to change them to your liking.</p>
          <p v-if="step == 2">We've chosen 3 random orks for your <span style="color:#29AAFC">Future Places</span> account. Feel free to change them to your liking.</p>
        </span>
      </div>
      <!-- <transition name="slide" mode="out-in"> -->
      <section v-if="loginMode == 'Login'" key="1">
        <form @submit.prevent="login">
          <input v-model="user.email" placeholder="Email" type="email" />
          <input v-model="user.password" placeholder="Password" type="password" />
          <button class="gradiant-button" @click="login">LOGIN &nbsp;&nbsp; <i class="fa fa-sign-in"></i></button>
        </form>

        <a href="#" @click="loginMode = 'Register'">Need an account?</a>
      </section>
      <section v-if="loginMode == 'Register'" key="2">
        <section v-if="step == 0" key="reg0">
          <form @submit.prevent="registerButtonClicked">
            <input v-model="user.email" placeholder="Email" type="email" />
            <password @score="showScore" v-model="user.password" :toggle="true" placeholder="Password" />
            <input v-model="user.confirm" placeholder="Confirm Password" type="password" />
            <div id="advanced-checkbox" @click="advancedSecurity = !advancedSecurity" :class="{ checked: advancedSecurity }">
              <div id="holder">
                <div class="slideOne" :class="{ checked: advancedSecurity }">
                  <input type="checkbox" value="None" id="sec" :checked="advancedSecurity" />
                  <label for="sec"></label>
                </div>
              </div>
              Advanced Security
            </div>

            <button :class="{ disabled: !passwordsValid }" class="gradiant-button">{{ advancedSecurity ? "CONTINUE" : "SIGN UP" }} &nbsp;&nbsp; <i :class="{ 'fa-arrow-right': advancedSecurity, 'fa-sign-in': !advancedSecurity }" class="fa "></i></button>
          </form>
        </section>
        <section v-if="step == 1" key="reg1">
          <div class="accordian">
            <div class="accordian-button" @click="showCMKorks = !showCMKorks">
              <span>Select your Master Orks</span>
              <i class="fa" :class="{ 'fa-chevron-down': !showCMKorks, 'fa-chevron-up': showCMKorks }" aria-hidden="true"></i>
            </div>
            <div class="accordian-rows" v-if="showCMKorks">
              <div class="accordian-row" v-for="ork in CMKorks" :key="ork.id" @click="ork.enabled = !ork.enabled" :class="{ disabled: !ork.enabled && CMKSelectedCount >= 3 }">
                <div class="enabled-col">
                  <div class="slideOne" :class="{ checked: ork.enabled }">
                    <input type="checkbox" value="None" :id="`cmk-${ork.orkId}`" :checked="ork.enabled" />
                    <label :for="`cmk-${ork.orkId}`"></label>
                  </div>
                </div>
                <div class="id-col">{{ ork.orkId }}</div>
                <div class="endpoint-col">{{ ork.endpoint }}</div>
              </div>
            </div>
          </div>

          <div id="btn-bar">
            <button class="gradiant-button back" @click="step = 0"><i class="fa fa-arrow-left"></i> &nbsp;&nbsp; BACK</button>
            <button :class="{ disabled: CMKSelectedCount < 3 }" class="gradiant-button" @click="step = 2">NEXT &nbsp;&nbsp; <i class="fa fa-arrow-right"></i></button>
          </div>
        </section>
        <a href="#" @click="loginMode = 'Login'">Have an account?</a>
      </section>
      <section v-if="step == 2" key="3">
        <div class="accordian">
          <div class="accordian-button" @click="showCVKorks = !showCVKorks">
            <span>Select your Vendor Orks</span>
            <i class="fa" :class="{ 'fa-chevron-down': !showCVKorks, 'fa-chevron-up': showCVKorks }" aria-hidden="true"></i>
          </div>
          <div class="accordian-rows" v-if="showCVKorks">
            <div class="accordian-row" v-for="ork in CVKorks" :key="ork.id" @click="ork.enabled = !ork.enabled" :class="{ disabled: !ork.enabled && CVKSelectedCount >= 3 }">
              <div class="slideOne" :class="{ checked: ork.enabled }">
                <input type="checkbox" value="None" :id="`cvk-${ork.orkId}`" :checked="ork.enabled" />
                <label :for="`cvk-${ork.orkId}`"></label>
              </div>
              <div class="id-col">{{ ork.orkId }}</div>
              <div class="endpoint-col">{{ ork.endpoint }}</div>
            </div>
          </div>
        </div>

        <div id="btn-bar">
          <button class="gradiant-button back" @click="step = 1"><i class="fa fa-arrow-left"></i> &nbsp;&nbsp; BACK</button>
          <button :class="{ disabled: CVKSelectedCount < 3 }" class="gradiant-button" @click="register">SIGN UP &nbsp;&nbsp; <i class="fa fa-sign-in"></i></button>
        </div>
      </section>

      <section v-if="loginMode == 'Forgot Password'" key="4">
        <section v-if="step == 0">
          <p style="margin-top:-40px">Enter your email to recover your fragments.</p>

          <form @submit.prevent="sendEmails">
            <input v-model="recoveryEmail" placeholder="Email" type="email" />
            <button class="gradiant-button">SEND REQUESTS &nbsp;&nbsp; <i class="fa fa-envelope"></i></button>
          </form>

          <a href="#" @click="loginMode = 'Login'">Go back</a>
        </section>
        <section v-if="step == 1">
          <p style="margin-top:-40px">If that email exists, you should recieve key fragments from your selected orks. Please paste them into the fields below.</p>
          <input v-for="i in 3" :key="i" v-model="frags[`frag${i}`]" :placeholder="`Fragment ${i}`" autocomplete="new-password" />
          <!-- <input v-model="newPassword" placeholder="New Password" type="password" autocomplete="new-password" /> -->
          <password @score="showScore" v-model="newPassword" :toggle="true" placeholder="Password" autocomplete="new-password" />
          <button :class="{ disabled: !reconstructValid }" class="gradiant-button" @click="reconstruct">CHANGE PASSWORD &nbsp;&nbsp; <i class="fa fa-check"></i></button>
          <a href="#" @click="step = 0">Go back</a>
        </section>
      </section>
      <!-- </transition> -->
      <p class="red"><span style="color:transparent">.</span> {{ error }}</p>
    </div>
  </div>
</template>

<script>
import Password from "vue-password-strength-meter";
import Tide from "tide-js";
export default {
  components: { Password },
  data: function() {
    return {
      show: false,
      loginMode: "Login",
      step: 0,
      passwordScore: 0,
      newPasswordScore: 0,
      advancedSecurity: false,
      user: {
        email: "",
        password: "",
        confirm: "",
        // email: `${Math.floor(Math.random() * 1000000)}@gmail.com`,
        // password: "Ff09&QcBWEXk",
        // confirm: "Ff09&QcBWEXk",
      },
      frags: {
        frag1: "",
        frag2: "",
        frag3: "",
        // frag1: "p+trV2jYbDLnWnhQjPU/WAq99DdAF1ygOF2MV3B7FXfr4M8PIvyLZFkURyK8lcij",
        // frag2: "crWGb17q/2K/f5CNMQ1ldACCBsbRVTCALeM0BX5GQm+wNGkEttTy4ZJYhAJWBP68",
        // frag3: "jOmbx5gZYdg2oZFae9qDAgWl35b4/zTjKUY18ollSJaTJbaaa2l7d67pjF+UihTx",
        frag4: "",
        frag5: "",
        frag6: "",
        frag7: "",
        frag8: "",
        frag9: "",
        frag10: "",
      },
      showCMKorks: false,
      showCVKorks: false,
      recoveryEmail: "",
      newPassword: "",
      error: "",
      expandedOrks: false,
      CMKorks: [],
      CVKorks: [],
    };
  },
  created() {
    this.CMKorks = this.generateOrks();
    this.CVKorks = this.generateOrks();
    this.$bus.$on("show-auth", (s) => {
      this.loginMode = "Login";
      this.step = 0;
      this.show = s;
    });
  },
  watch: {
    step: function() {
      this.error = " ";
    },
    loginMode: function() {
      this.error = " ";
      this.step = 0;
    },
  },
  computed: {
    passwordsValid: function() {
      return this.passwordScore == 4 && this.user.password == this.user.confirm;
    },
    reconstructValid: function() {
      return this.passwordScore == 4 && this.frags.frag1 != "" && this.frags.frag2 != "" && this.frags.frag3 != "";
    },
    confirmError: function() {
      return this.user.password == this.user.confirm ? " " : "Passwords do not match.";
    },
    CMKSelectedCount: function() {
      var length = this.CMKorks.filter((o) => o.enabled).length;
      this.error = length < 3 ? "You must select 3 ork nodes." : "";
      return length;
    },
    CVKSelectedCount: function() {
      var length = this.CVKorks.filter((o) => o.enabled).length;
      this.error = length < 3 ? "You must select 3 ork nodes." : "";
      return length;
    },
  },
  methods: {
    autoFill() {
      this.user = {
        email: `${Math.floor(Math.random() * 1000000)}@gmail.com`,
        password: "Ff09&QcBWEXk",
        confirm: "Ff09&QcBWEXk",
      };
    },
    showScore(score) {
      this.passwordScore = score;
      if (this.passwordScore < 4) this.error = "Please choose a stronger password.";
      else this.error = "";
    },
    generateOrks() {
      var orks = [...Array(10)].map((_, i) => {
        return {
          id: i,
          enabled: false,
          orkId: `ork-${i}`,
          endpoint: `https://ork-${i}.azurewebsites.net`,
        };
      });

      const shuffled = orks.sort(() => 0.5 - Math.random());

      for (let i = 0; i < 3; i++) {
        shuffled[i].enabled = true;
      }

      orks = shuffled.sort(function(a, b) {
        return a.id - b.id;
      });
      return orks;
    },
    registerButtonClicked() {
      if (this.advancedSecurity) {
        this.step = 1;
        console.log("hey");
      } else this.register();
    },
    register() {
      if (this.user.password.length < 4) return (this.error = "Password requires at least 4 characters.");

      this.$loading(true, "Creating Tide account...");

      // Artificial wait to allow loading overlay to react
      setTimeout(async () => {
        try {
          var tide = new Tide("VendorId", "https://tidevendor.azurewebsites.net/vendor");

          var orkIds = this.CMKorks.filter((o) => o.enabled).map((n) => n.orkId);
          // var orkIds = ["ork-0", "ork-1", "ork-2", "ork-3", "ork-4", "ork-5"];

          var signUp = await tide.register(this.user.email, this.user.password, this.user.email, orkIds);

          console.log(signUp.key);

          const registerTideResult = {
            privateKey: "5J9mJizKfGrFdnSZNswomzTeoVoLi3649YdrHGwT3EQTCTPLf3Z",
            publicKey: "EOS54P6fHzGyr1SSTYusmA6BhuEhJYuY5uZUHqQBzMeadk2jdbJey",
            accountName: "tidexhsuridk",
          };

          this.$loading(true, "Generating your vendor account and distributing your key fragments...");

          await this.$helper.sleep(2000);
          const registerVendorResult = {
            cvkPublicKey: "ANH+mAyO2/f9i53isvp6BG8mEcJyy2C5CvDlexRLHR+jOSJwIOFb2d3Kp5kiDlSzjcHn4ByjIuxzwlWrRTg2h/GUp8OdkQVviV1KUhIajEqtdMJVAZ7bCpn0nCzgmLG40A==",
            cvkPrivateKey: "AtH+mAyO2/f9i53isvp6BG8mEcJyy2C5CvDlexRLHR+jOSJwIOFb2d3Kp5kiDlSzjcHn4ByjIuxzwlWrRTg2h/E4HOBbB5EIoFFjxIcMQZ2L4uR5qCPVn0jv9w0sMAubmQ==",
          };

          this.$store.commit("storeUser", {
            username: this.user.username,
            account: registerTideResult.accountName,
            aes: signUp.key,
            keys: {
              tide: {
                pub: registerTideResult.publicKey,
                priv: registerTideResult.privateKey,
              },
              vendor: {
                pub: registerVendorResult.cvkPublicKey,
                priv: registerVendorResult.cvkPrivateKey,
              },
            },
            trustee: false,
            tide: 0,
          });

          this.$bus.$emit("show-message", "You have registered successfully");
          this.$bus.$emit("showLoginModal", false);
          this.$loading(false, "");
          this.$authAction();

          this.$bus.$emit("show-auth", false);
        } catch (thrownError) {
          this.$bus.$emit("show-error", thrownError.response != null && thrownError.response.text != null ? thrownError.response.text : thrownError);
          this.$loading(false, "");
        }
      }, 100);
    },
    login() {
      this.$loading(true, "Logging you in...");

      // Artificial wait to allow loading overlay to react
      setTimeout(async () => {
        try {
          var loginResult = await this.$tide.login(this.user.email, this.user.password);
          console.log(loginResult.key);
          this.$loading(true, "Gathering Vendor account...");

          var user = {
            username: this.user.email,
            account: "tidegwhfxvls",
            aes: loginResult.key,
            nodes: [
              {
                ork_node: "tideorkxxxx2",
                ork_url: "https://tide-ork-02.azurewebsites.net/",
              },
              {
                ork_node: "tideorkxxxx1",
                ork_url: "https://tide-ork-01.azurewebsites.net/",
              },
            ],
            keys: {
              tide: {
                pub: "ungathered",
                priv: "5HsH3U6DG8VAmVHdyPcekLkuRQ1LUmsiaBZMWYSeE4uXe3chcTT",
              },
              vendor: {
                pub: "AIqX6YNpQPBzCQ2GUg==",
                priv: "AoqX6YNpQPBzErzaEQ==",
              },
            },
            tide: 0,
            trustee: false,
          };

          this.$store.commit("storeUser", user);

          this.$bus.$emit("show-message", "You have logged in successfully");
          this.$bus.$emit("showLoginModal", false);
          this.$loading(false, "");
          this.$authAction();

          this.$bus.$emit("show-auth", false);
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
    },
    sendEmails() {
      if (this.recoveryEmail == "") return (this.error = "Please input your email address.");
      this.$tide.recover(this.recoveryEmail);
      this.step = 1;
    },
    async reconstruct() {
      try {
        this.$loading(true, "Changing your password...");
        var shares = `${this.frags.frag1}\n${this.frags.frag2}\n${this.frags.frag3}`;

        var newKey = await this.$tide.reconstruct(this.recoveryEmail, shares, this.newPassword);

        this.loginMode = "Login";
        this.step = 0;
        this.$loading(false, "");
        this.$bus.$emit("show-message", "Your password has been changed.");
      } catch (error) {
        this.$loading(false, "");
        this.$bus.$emit("show-message", error != "" && error != null ? error : "Failed");
      }
    },
  },
};
</script>

<style lang="scss" scoped>
img {
  pointer-events: none;
}
#content {
  top: 0;
  width: 100%;
  position: fixed;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  height: 100vh;
  background: white;
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 9999;

  #deco-1 {
    position: absolute;
    right: 0;
    top: 0;
    max-width: 550px;
    z-index: 0;
  }

  #deco-2 {
    position: absolute;
    left: 0;
    bottom: 0;
    max-width: 400px;
    z-index: 0;
  }

  #logo {
    position: absolute;
    top: 10px;
    left: 10px;
    z-index: 99999;
    img {
      width: 150px;
    }
  }

  #auth-box {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    color: #5b4e74;
    z-index: 999999999999999999999999;
    width: 380px;
    #title {
      flex-direction: column;
      justify-content: center;
      align-items: center;
      text-align: center;
      margin-bottom: 40px;
      h1 {
        margin-bottom: 7px;

        font-weight: 900;
        font-family: "Roboto";
      }

      #divider {
        display: block;
        margin: 0 auto;
        margin-bottom: 40px;
      }

      p {
        margin: 5px;
      }
    }

    section {
      width: 100%;
      max-width: 600px;
      display: flex;
      flex-direction: column;
      text-align: center;

      input {
        height: 40px;
        border: 0px;
        border-bottom: 2px solid #e4e4e4 !important;
        margin-bottom: 25px;
        padding-left: 5px;

        &:hover {
          border-bottom: 2px solid #0dbae8 !important;
        }
      }

      .gradiant-button {
        transition: all 0.5s;
        margin-top: 20px;
        border: 0px;
        border-radius: 2px;
        height: 50px;
        font-size: 16px;
        color: white;
        margin-bottom: 10px;
        // background: linear-gradient(142deg, rgba(0, 155, 177, 1) 0%, rgba(6, 60, 141, 1) 34%, rgba(9, 9, 121, 1) 60%, rgba(16, 0, 255, 1) 100%);
        background: #3d90df;
        i {
          margin-right: 6px;
        }

        &:hover {
          background: #235d94;
        }

        &.back {
          background: #ffa722;

          &:hover {
            background: #bd8a00;
          }
        }

        &.disabled {
          background: black;
          pointer-events: none;
          opacity: 0.5;
        }
      }
    }
    table {
      thead {
        width: 100% !important;
      }
      width: 100% !important;
    }
  }

  .link {
    color: orange;

    &:hover {
      color: #0dbae8;
    }
  }

  .fade-enter-active,
  .fade-leave-active {
    transition: opacity 0.5s;
  }
  .fade-enter, .fade-leave-to /* .fade-leave-active below version 2.1.8 */ {
    opacity: 0;
  }
}

.accordian {
  transition: all 0.4s;

  margin: 0px;
  margin-top: -30px;
  width: 100%;
  .accordian-button {
    height: 40px;
    cursor: pointer;
    border: 1px solid rgb(66, 62, 255);
    border-bottom: 0px solid rgb(66, 62, 255);
    border-radius: 3px 3px 0 0;
    background: rgb(66, 62, 255);
    color: white;
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding-left: 7px;
    padding-right: 7px;
  }

  .accordian-rows {
    max-height: 200px;
    overflow: scroll;
    overflow-x: hidden;

    border: 1px solid rgb(66, 62, 255);
    border-radius: 0 0 3px 3px;
    padding-bottom: -1px;
  }

  .accordian-row {
    cursor: pointer;
    width: 100%;
    height: 40px;
    display: flex;
    flex-direction: row;
    align-items: center;
    border: 1px solid rgb(66, 62, 255);
    //border-top: 0px solid rgb(66, 62, 255);
    margin-top: -1px;
    margin-left: -1px;
    margin-right: -1px;
    user-select: none;

    &:hover {
      background: rgb(243, 243, 243);
    }

    .enabled-col {
      padding-left: 7px !important;
      width: 10%;
      pointer-events: none;
    }
    .id-col {
      width: 20%;
    }
    .endpoint-col {
      width: 70%;
    }

    &.disabled {
      background: rgb(221, 221, 221);
      pointer-events: none;
      cursor: unset;
      opacity: 0.3;
    }
  }
}

::-webkit-scrollbar {
  width: 5px;
}

/* Track */
::-webkit-scrollbar-track {
  background: #f1f1f1;
}

/* Handle */
::-webkit-scrollbar-thumb {
  background: rgb(66, 62, 255);
}

/* Handle on hover */
::-webkit-scrollbar-thumb:hover {
  background: rgb(66, 62, 255);
}

#btn-bar {
  width: 100%;
  display: flex;
  justify-content: space-between;
  button {
    width: 48%;
  }
}

form {
  display: flex;
  flex-direction: column;
}

input[type="checkbox"] {
  pointer-events: none;
  visibility: hidden !important;
}

/* SLIDE ONE */
.slideOne {
  pointer-events: none;
  width: 30px;
  height: 10px;
  background: #ffa722;
  margin: 20px auto;

  border-radius: 50px;
  position: relative;

  box-shadow: inset 0px 1px 1px rgba(139, 139, 139, 0.3), 0px 1px 0rgba (110, 110, 110, 0.1);

  &.checked {
    background: rgb(66, 62, 255);
  }
}

.slideOne label {
  pointer-events: none;
  display: block;
  width: 16px;
  height: 16px;

  border-radius: 50px;

  transition: all 0.4s ease;
  cursor: pointer;
  position: absolute;
  top: -4px;
  left: -3px;

  box-shadow: 0px 2px 5px 0px rgba(0, 0, 0, 0.3);
  background: #fcfff4;

  background: linear-gradient(top, #fcfff4 0%, #dfe5d7 40%, #b3bead 100%);
}

.slideOne input[type="checkbox"]:checked + label {
  pointer-events: none;
  left: 17px;
}

#advanced-checkbox {
  width: 100%;
  display: flex;
  flex-direction: row;
  justify-content: flex-start;
  align-items: center;
  cursor: pointer;
  opacity: 0.6;
  &.checked {
    opacity: 1;
  }

  #holder {
    width: 50px;
  }
}

#auto-fill {
  position: absolute;
  bottom: 10px;
  left: 10px;
  border: 1px solid gray;
}
</style>
