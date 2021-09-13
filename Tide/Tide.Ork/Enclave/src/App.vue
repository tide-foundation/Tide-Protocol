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

onMounted(async () => {
  window.addEventListener("resize", () => fetchHeight());
  fetchHeight();
});

const fetchHeight = () => {
  let vh = window.innerHeight * 0.01;
  document.documentElement.style.setProperty("--vh", `${vh}px`);
};
</script>

<style lang="scss">
#app {
  height: $screen-height;
  background-color: #0072c6;
  background: bottom cover url(./assets/img/test-5.jpg);

  #wrapper {
    width: 100%;
  }
}
</style>
