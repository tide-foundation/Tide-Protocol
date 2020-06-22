<template>
  <div id="app">
      Hey
    <!--<div class="layout center">
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
            <p v-if="status != ''">{{status}}</p>
          </div>
        </div>
      </div>
    </div>-->
  </div>
</template>


<!--<script>
import Tide from "../../../Tide.Js/src/Tide" // TODO: Change to npm package for production. Does not seem to be a conditional import
export default {
  props: {},
  data: function () {
    return {
      tide: null,
      nodeCount: 3,
      user: {
        username: Math.floor(Math.random() * 1000000000),
        // username: 11111333,
        password: "password",
      },
      status: "",
      dAuthFlow: null,
    };
  },
  async created() {
    var urls = [...Array(3)].map((_, i) => `https://ork-${i}.azurewebsites.net`);
    this.tide = new Tide("VendorId", "http://localhost:50598/vendor", urls);
    this.register();
  },
  methods: {
    async register() {
      try {
        var response = await this.tide.register(this.user.username, this.user.password);
        console.log(response)
      } catch (error) {
        console.log('External error: ', error)
        this.status = error
      }
    },
    login() { },
    guid() {
      return "xxxxxxxx".replace(/[xy]/g, function (c) {
        var r = (Math.random() * 16) | 0,
          v = c == "x" ? r : (r & 0x3) | 0x8;
        return v.toString(16);
      });
    }
  },
};
</script>-->

<style lang="scss"></style>
