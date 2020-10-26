<template>
  <div id="app">
    <Status></Status>
    <Loading></Loading>
    <h1>THIS IS AN ORK NODE</h1>
    <router-view />
  </div>
</template>

<script>
import Loading from "@/components/Loading.vue";
import Status from "@/components/Status.vue";

export default {
  components: {
    Loading,
    Status,
  },

  data() {
    return {};
  },
};
</script>

<style lang="scss">
body,
html {
  width: 100%;
  padding: 0;
  margin: 0;
}

#app {
  width: 100%;
  height: 100vh;
  display: flex;
  justify-content: center;
  flex-direction: column;
  align-items: center;
}
</style>
