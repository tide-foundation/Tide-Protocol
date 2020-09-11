<template>
  <span class="auth-form">
    <section v-if="$parent.step == 0">
      <p>Enter your email to recover your fragments.</p>

      <form @submit.prevent="sendEmails">
        <input v-model="recoveryEmail" placeholder="Email" type="email" />
        <button class="gradiant-button">
          SEND REQUESTS &nbsp;&nbsp;
          <i class="fa fa-envelope"></i>
        </button>
      </form>

      <a href="#" @click="$parent.changeMode('Login')">Go back</a>
    </section>
    <section v-if="$parent.step == 1">
      <p>
        If that email exists, you should recieve key fragments from your
        selected orks. Please paste them into the fields below.
      </p>
      <input class="f-w" v-for="i in 3" :key="i" v-model="frags[`frag${i}`]" :placeholder="`Fragment ${i}`" autocomplete="new-password" />
      <!-- <input v-model="newPassword" placeholder="New Password" type="password" autocomplete="new-password" /> -->
      <password @score="$parent.showScore" v-model="newPassword" :toggle="true" placeholder="Password" autocomplete="new-password" />
      <button :class="{ disabled: !reconstructValid }" class="gradiant-button" @click="reconstruct">
        CHANGE PASSWORD &nbsp;&nbsp;
        <i class="fa fa-check"></i>
      </button>
      <br />
      <a href="#" @click="$parent.step = 0">Go back</a>
    </section>
  </span>
</template>


<script>
import Password from "vue-password-strength-meter";
export default {
    components: { Password },

    data: function() {
        return {
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
                frag10: ""
            },
            newPasswordScore: 0,
            recoveryEmail: "",
            newPassword: ""
        };
    },
    computed: {
        reconstructValid: function() {
            return this.$parent.passwordScore == 4 && this.frags.frag1 != "" && this.frags.frag2 != "" && this.frags.frag3 != "";
        }
    },
    methods: {
        sendEmails() {
            if (this.recoveryEmail == "") return (this.$parent.error = "Please input your email address.");
            this.$tide.recover(this.recoveryEmail);
            this.$parent.step = 1;
        },
        async reconstruct() {
            try {
                this.$loading(true, "Changing your password...");
                // var shares = `${this.frags.frag1}\n${this.frags.frag2}\n${this.frags.frag3}`;

                //var newKey = await this.$tide.reconstruct(this.recoveryEmail, shares, this.newPassword);

                this.$parent.changeMode("Login");
                this.$parent.step = 0;
                this.$loading(false, "");
                this.$bus.$emit("show-message", "Your password has been changed.");
            } catch (error) {
                this.$loading(false, "");
                this.$bus.$emit("show-message", error != "" && error != null ? error : "Failed");
            }
        }
    }
};
</script>

<style>
</style>