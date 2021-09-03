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
  height: calc(var(--vh, 1vh) * 100);

  #wrapper {
    width: 100%;
    max-width: 800px;
    height: 600px;
    border: 1px solid rgb(219, 219, 219);
    box-shadow: rgba(0, 0, 0, 0.16) 0px 10px 36px 0px, rgba(0, 0, 0, 0.06) 0px 0px 0px 1px;
  }
}
</style>
