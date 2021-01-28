<template>
  <div id="app">
    <Status></Status>
    <Loading></Loading>
    <img class="logo" src="https://upload.wikimedia.org/wikipedia/commons/5/51/RMIT_University_Logo.svg" alt="tide logo" />
    <div id="content" v-if="!useTwoFactor || $store.getters.sessionId != null">
      <router-view />
    </div>
  </div>
</template>

<script>
import Loading from "@/components/Loading.vue";
import Status from "@/components/Status.vue";
import { HubConnectionBuilder, LogLevel } from "@aspnet/signalr";
export default {
  components: {
    Loading,
    Status,
  },
  data() {
    return {
      hub: null,
      useTwoFactor: false,
    };
  },
  async created() {
    // if (this.useTwoFactor) {
    //   this.hub = await this.createHub();
    //   // Collect the sessionId
    //   this.hub.on("openSession", (id) => {
    //     this.$store.commit("UPDATE_SESSION_ID", id);
    //   });
    //   // Collect the generated token
    //   this.hub.on("deliver", async (data) => {
    //     await this.$store.dispatch("finalizeAuthentication", JSON.parse(data));
    //   });
    //   // Request an open session
    //   this.hub.invoke("RequestSession");
    // }
  },
  methods: {
    async createHub() {
      var location = process.env.NODE_ENV == "development" ? process.env.VUE_APP_SOCKET_ENDPOINT : this.$store.getters.origin;

      const connection = new HubConnectionBuilder()
        .withUrl(`${location}/enclave-hub`)
        .configureLogging(LogLevel.Information)
        .build();

      await connection.start();
      return connection;
    },
  },
};
</script>

<style lang="scss">
#app {
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  flex-direction: column;
  align-items: center;

  .logo {
    width: 150px;
  }
}
</style>
