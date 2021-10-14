<template>
  <div id="error" class="f-c" v-if="show" @click="clicked">
    <div id="error-content" :class="[alert.type]">
      {{ alert.msg }}
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, inject } from "vue";

import { BUS_KEY, SHOW_ERROR_KEY } from "@/assets/ts/Constants";

var alert = ref<Alert>({ type: "success", msg: "" });
var show = ref(false);
const bus = inject(BUS_KEY) as IBus;
var timer = 0;
bus.on(SHOW_ERROR_KEY, (data: Alert) => {
  clearTimeout(timer);
  alert.value = data;
  show.value = true;
  timer = setTimeout(() => {
    show.value = false;
  }, 5000);
});

const clicked = () => {
  show.value = false;
  clearTimeout(timer);
};
</script>

<style lang="scss" scoped>
#error {
  position: absolute;
  top: 20px;
  width: 100%;
  margin-bottom: 20px;
  #error-content {
    text-align: center;
    background-color: $background;
    border-radius: $border-radius;
    padding: 20px;
    box-shadow: rgba(0, 0, 0, 0.16) 0px 10px 36px 0px, rgba(0, 0, 0, 0.06) 0px 0px 0px 1px;

    max-width: 500px;

    &.error {
      color: white;
      background-color: rgb(187, 58, 58);
    }

    &.success {
      color: white;
      background-color: rgb(88, 187, 58);
    }
  }
}
</style>
