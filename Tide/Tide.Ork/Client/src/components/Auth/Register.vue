<template>
  <div>
    <h2>Create your Account</h2>
    <form @submit.prevent="register">
      <input type="text" v-model="user.username" placeholder="Username" />
      <div class="password-row mt-10">
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
      this.$loading(true, "Registering...");
      this.$nextTick(async () => {
        try {
          var signUpResult = await this.$store.dispatch("registerAccount", this.user);

          await this.$store.dispatch("finalizeAuthentication", signUpResult);
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

<style lang="scss" scoped>
h1 {
  text-align: center;
}

#recovery-emails {
  max-height: 200px;
  overflow-y: auto;
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
