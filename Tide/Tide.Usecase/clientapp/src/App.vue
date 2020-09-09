<template>
  <section>
    <navbar />
    <transitionBox>
      <router-view />
    </transitionBox>

    <transitionBox>
      <loading />
    </transitionBox>

    <transitionBox>
      <Auth />
    </transitionBox>

    <transitionBox>
      <message />
    </transitionBox>

    <transitionBox>
      <error />
    </transitionBox>
  </section>
</template>

<script>
import transitionBox from "./components/TransitionBox.vue";
import navbar from "./components/Nav.vue";
import Auth from "./components/Auth.vue";
import message from "./components/Message.vue";
import loading from "./components/Loading.vue";
import error from "./components/Error.vue";

export default {
    components: {
        transitionBox,
        navbar,
        Auth,
        message,
        loading,
        error
    }
};
</script>

<style lang="scss">
.list-item {
    display: inline-block;
    margin-right: 10px;
}
.list-enter-active,
.list-leave-active {
    transition: all 1s;
}
.list-enter, .list-leave-to /* .list-leave-active below version 2.1.8 */ {
    opacity: 0;
    transform: translateY(30px);
}
</style>
