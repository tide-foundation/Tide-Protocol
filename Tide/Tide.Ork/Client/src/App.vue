<template>
  <div id="app">
    <Status></Status>
    <Loading></Loading>
    <img class="logo" src="@/assets/img/tide-logo.svg" alt="tide logo" />
    <div id="content" v-if="$store.getters.sessionId != null">
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
    };
  },
  async created() {
    this.hub = await this.createHub();

    // Collect the sessionId
    this.hub.on("openSession", (id) => {
      this.$store.commit("UPDATE_SESSION_ID", id);
      console.log(id);
    });

    // Collect the generated token
    this.hub.on("deliver", async (data) => {
      console.log("TOKEN DELIVERED", data);

      await this.$store.dispatch("finalizeAuthentication", JSON.parse(data));
    });

    // Request an open session
    this.hub.invoke("RequestSession");
  },
  methods: {
    async createHub() {
      const connection = new HubConnectionBuilder()
        .withUrl(`${this.$store.getters.origin}/enclave-hub`)
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
