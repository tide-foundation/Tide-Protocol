<template>
  <div id="app">
    <Status></Status>
    <Loading></Loading>
    <div v-if="$store.getters.loggedIn">
      <div id="nav">
        <router-link to="/">Home</router-link>|
        <router-link to="/about">About</router-link>
      </div>
      <router-view />
    </div>
    <Auth v-else></Auth>
  </div>
</template>


<script>
import Auth from "@/components/Auth.vue";
import Loading from "@/components/Loading.vue";
import Status from "@/components/Status.vue";
export default {
    components: {
        Auth,
        Loading,
        Status,
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
</style>
