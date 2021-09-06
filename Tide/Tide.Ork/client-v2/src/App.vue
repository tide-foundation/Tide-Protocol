<template>
  <div id="app" class="f-c">
    <div id="wrapper">
      <router-view></router-view>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from "vue";

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
    max-width: 800px;
    min-height: 500px;
  }
}
</style>
