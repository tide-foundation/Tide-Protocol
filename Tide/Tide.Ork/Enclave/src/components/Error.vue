<template>
  <div id="error" class="f-c" v-if="show" @click="clicked">
    <div id="error-content" :class="[alert.type]">
      {{ alert.msg }}
    </div>
  </div>
</template>

<script lang="ts">
import Base from "@/assets/ts/Base";
import { SHOW_ERROR_KEY } from "@/assets/ts/Constants";

export default class Error extends Base {
  alert: Alert = { type: "success", msg: "" };
  show: boolean = false;
  timer: number = 0;

  created() {
    this.bus.on(SHOW_ERROR_KEY, (data: Alert) => {
      clearTimeout(this.timer);
      this.alert = data;
      this.show = true;
      this.timer = setTimeout(() => {
        this.show = false;
      }, 5000);
    });
  }

  clicked() {
    this.show = false;
    clearTimeout(this.timer);
  }
}
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
