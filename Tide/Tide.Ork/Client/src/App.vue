<template>
  <div id="app">
    <Status></Status>
    <Loading></Loading>
    <img class="logo" src="@/assets/img/tide-logo.svg" alt="tide logo" />
    <div id="content">
      <router-view />
    </div>
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
