<template>
  <div id="app">
    <div class="layout center">
      <div class="center">
        <div class="center col">
          Register
          <div class="mb-2">
            <input v-model="user.username" type="text" value="admin" placeholder="Username" />
          </div>
          <div class="mb-2">
            <input v-model="user.password" type="password" value="123456" placeholder="Password" />
          </div>
          <button @click="register" type="button">Register</button>
          <button @click="login" type="button">Login</button>
          <div>
            <p id="register-status"></p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {},
  data: function() {
    return {
      nodeCount: 3,
      user: {
        username: "admin",
        password: "pass",
      },
      dAuthFlow: null,
    };
  },
  created() {},
  methods: {
    createAuthFlow() {
      var urls = [...Array(this.nodeCount)].map((_, i) => `https://ork-${i}.azurewebsites.net`);
      return new cryptide.DAuthFlow(urls, this.user.username);
    },
    async register() {
      var flow = this.createAuthFlow();
      var key = await flow.signUp(this.user.password, this.nodeCount);

      console.log(key);
      console.log(`new authentication key for user ${this.user.username}:`, key.toString());
    },
    login() {},
  },
};
</script>

<style lang="scss"></style>
