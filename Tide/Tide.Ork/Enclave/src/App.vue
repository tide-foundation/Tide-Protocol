<template>
  <div id="app" class="f-c">
    <div id="wrapper" class="f-c">
      <router-view></router-view>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from "vue";
import mainStore from "./store/mainStore";

var loggedIn = computed(() => {
  return mainStore.getState.account != null;
});

onMounted(async () => {
  window.addEventListener("resize", () => fetchHeight());
  fetchHeight();
});

const fetchHeight = () => {
  let vh = window.innerHeight * 0.01;
  document.documentElement.style.setProperty("--vh", `${vh}px`);
};
</script>

<style lang="scss" scoped>
#app {
  height: $screen-height;
  background: url(./assets/img/rmit-background-2.jpg);
  background-size: cover;
  background-position: bottom;
  #wrapper {
    width: 100%;
  }
}
</style>
