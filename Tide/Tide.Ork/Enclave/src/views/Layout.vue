<template>
  <div id="layout" class="full-height f-c">
    <div id="header" class="full-width "></div>
    <div id="content" class="f-grow full-width ">
      <router-view></router-view>
    </div>

    <a id="footer" class="full-width f-c" href="https://tide.org" target="_blank">
      <img src="../assets/img/tide-inside.png" alt="" />
    </a>
    <transition name="fade" mode="out-in">
      <loader v-if="loading"></loader>
    </transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, inject } from "vue";
import Loader from "@/components/Loader.vue";
import { BUS_KEY, SET_LOADING_KEY } from "@/assets/ts/Constants";

const bus = inject(BUS_KEY) as IBus;
var loading = ref(false);
bus.on(SET_LOADING_KEY, (data: any) => (loading.value = data));
</script>

<style lang="scss" scoped>
#layout {
  position: relative;
  box-shadow: rgba(0, 0, 0, 0.16) 0px 10px 36px 0px, rgba(0, 0, 0, 0.06) 0px 0px 0px 1px;
  background: $background;
  border-radius: $border-radius;
  padding: 10px 20px;

  .vendor-logo {
    position: absolute;
    left: 20px;
    top: 20px;
    max-width: 180px;
  }

  #header {
    height: 100px;
  }

  #content {
  }

  #footer {
    height: 60px;

    img {
      margin-bottom: 10px;
      //  height: 200px;
    }
  }
}
</style>
