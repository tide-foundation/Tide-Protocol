<template>
  <div>
    <h2>Create your Account</h2>
    <form @submit.prevent="register">
      <input type="text" v-model="user.username" placeholder="Username" />
      <div id="password-row" class="mt-10">
        <input type="password" v-model="user.password" placeholder="Password" />
        <input type="password" v-model="user.confirm" placeholder="Confirm" />
      </div>

      <password-meter :password="user.password" class="mt-10" />

      <div id="recovery-emails">
        <div class="recovery-email mb-10" v-for="(email, index) in user.recoveryEmails" :key="index">
          <input type="email" :placeholder="`Recovery Email Address ${index + 1}`" required v-model="user.recoveryEmails[index]" /> <i v-if="index == 0" class="fas fa-plus" @click="user.recoveryEmails.push('')"></i>
          <i v-if="index != 0" class="fas fa-minus" @click="user.recoveryEmails.splice(index, 1)"></i>
        </div>
      </div>

      <div class="action-row mt-50">
        <p @click="$parent.changeMode('LoginUsername')">Sign in instead</p>
        <button type="submit">REGISTER</button>
      </div>

      <div class="advanced-options" @click="$parent.changeMode('SelectOrks')"><p>Advanced Options</p></div>
    </form>
  </div>
</template>

<script>
import passwordMeter from "vue-simple-password-meter";
import request from "superagent";
export default {
  props: ["user"],
  components: { passwordMeter },

  created() {},
  methods: {
    async register() {
      try {
        this.$loading(true, "Registering...");

        // setTimeout(() => {
        //     this.$store.dispatch("finalizeAuthentication", { jwt: "Give me a JoseWT" });
        // }, 2000);

        var signUpResult = await this.$store.dispatch("registerAccount", this.user);

        await this.$store.dispatch("finalizeAuthentication", signUpResult);
      } catch (error) {
        this.$bus.$emit("show-error", error);
      } finally {
        this.$loading(false, "");
      }
    },
  },
};
</script>

<style lang="scss" scoped>
h1 {
  text-align: center;
}
.po-password-strength-bar {
  border-radius: 0px;
  transition: all 0.2s linear;
  border: 1px solid #999999;
  margin-bottom: 10px;
  margin-right: -1px;
  width: calc(100% - 2px);
}

#password-row {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: space-between;
  input {
    width: 48.5%;
  }
}

#recovery-emails {
  .recovery-email {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
    align-items: center;
    i {
      width: 20px;
      margin: 0 10px;
      color: #999999;
      cursor: pointer;
      &:hover {
        color: orange;
      }
    }
  }
}
</style>
