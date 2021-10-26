<template>
  <div id="auth-layout" class="full-height f-r">
    <div id="left" class="f-grow f-c">
      <img src="../../assets/img/qr.png" alt="" />
      <h3>Tide Authenticator</h3>
    </div>
    <div id="right" class="full-height f-c">
      <img
        v-if="mainStore.getState.config.styles?.logoDark != null"
        class="vendor-logo mobile"
        :src="mainStore.getState.config.styles?.logoDark"
        alt=""
      />

      <div id="header" class="full-width "></div>
      <div id="content" class="f-grow full-width ">
        <router-view></router-view>
      </div>

      <div id="footer" class="full-width f-c">
        <img @click="$router.push('options')" alt="" :src="pictureHover" @mouseover="hover = true" @mouseleave="hover = false" />
      </div>

      <transition name="fade" mode="out-in">
        <loader v-if="loading"></loader>
      </transition>
    </div>
    <img
      v-if="mainStore.getState.config.styles?.logoDark != null"
      class="vendor-logo desktop"
      :src="mainStore.getState.config.styles?.logoLight"
      alt=""
    />
  </div>
</template>

<script lang="ts">
import Base from "@/assets/ts/Base";
import { SET_LOADING_KEY } from "@/assets/ts/Constants";
import { Options } from "vue-class-component";
import Loader from "@/components/Loader.vue";
@Options({
  components: {
    Loader,
  },
})
export default class Forgot extends Base {
  loading: boolean = false;
  tideInside: any = require("../../assets/img/tide-inside-new.png");
  tideInsideHover: any = require("../../assets/img/tide-inside-new-hover.png");
  hover = false;

  get pictureHover() {
    return this.hover ? this.tideInsideHover : this.tideInside;
  }

  mounted() {
    this.bus.on(SET_LOADING_KEY, (data: any) => (this.loading = data));
  }
}
</script>

<style lang="scss" scoped>
#auth-layout {
  width: 100%;
  max-width: 800px;
  min-height: 500px;
  position: relative;
  box-shadow: rgba(0, 0, 0, 0.16) 0px 10px 36px 0px, rgba(0, 0, 0, 0.06) 0px 0px 0px 1px;
  .vendor-logo {
    position: absolute;
    left: 10px;
    top: 10px;
    max-width: 180px;
  }

  #left {
    text-align: center;
    border-radius: $border-radius 0 0 $border-radius;
    background: rgba(255, 255, 255, 0.253);

    backdrop-filter: blur(10px);
    -webkit-backdrop-filter: blur(10px);

    img {
      margin-top: 50px;
      max-width: 200px;
      width: 95%;
    }
    h3 {
      color: white;
    }

    @media only screen and (max-width: $mobile-break) {
      display: none;
    }
  }

  #right {
    position: relative;
    width: 100%;
    max-width: 394px;
    background: $background;
    border-radius: 0 $border-radius $border-radius 0;
    padding: 10px 20px;

    @media only screen and (max-width: $mobile-break) {
      max-width: 100%;
      height: $screen-height;
      border-radius: 0;
    }

    @media only screen and (max-width: 390px) {
      padding: 5px 10px;
    }

    #header {
      height: 0px;
    }

    #content {
    }

    #footer {
      height: 60px;

      img {
        cursor: pointer;
        margin-bottom: 10px;
        //  height: 200px;
      }
    }
  }
}
</style>
