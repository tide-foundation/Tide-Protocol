<template>
  <div id="app">
    <EndToggle v-if="$store.getters.loggedIn"></EndToggle>
    <Status></Status>
    <Loading></Loading>
    <Frontend v-if="$store.getters.mode == 'Frontend'"></Frontend>
    <Backend v-else></Backend>
  </div>
</template>


<script>
import Frontend from "@/components/Frontend/Frontend.vue";
import Backend from "@/components/Backend/Backend.vue";
import Loading from "@/components/Loading.vue";
import Status from "@/components/Status.vue";
import EndToggle from "@/components/EndToggle.vue";
export default {
    components: {
        Loading,
        Status,
        Frontend,
        Backend,
        EndToggle,
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
    align-items: center;
}
</style>
