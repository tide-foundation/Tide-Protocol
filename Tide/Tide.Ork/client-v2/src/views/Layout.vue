<template>
  <div id="layout" class="full-height f-r">
    <img class="vendor-logo" src="../assets/img/rmit-logo.svg" alt="" />
    <div id="left" class="f-grow f-c">
      <img src="../assets/img/qr.svg" alt="" />
      <h3>Tide Authenticator</h3>
    </div>
    <div id="right" class="full-height f-c">
      <img class="vendor-logo mobile" src="../assets/img/rmit-logo-dark.svg" alt="" />
      <div id="header" class="full-width "></div>
      <div id="content" class="f-grow full-width ">
        <router-view></router-view>
      </div>

      <a id="footer" class="full-width f-c" href="https://tide.org" target="_blank">
        <img src="../assets/img/tide-inside.svg" alt="" />
      </a>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from "vue";
</script>

<style lang="scss" scoped>
#layout {
  background: url(../assets/img/rmit-background.jpg);
  position: relative;
  .vendor-logo {
    position: absolute;
    left: 10px;
    top: 10px;
    max-width: 180px;

    &.mobile {
      display: none;
      @media only screen and (max-width: $mobile-break) {
        display: initial;
      }
    }
  }

  #left {
    text-align: center;
    img {
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
    width: 100%;
    max-width: 394px;
    background: $background;

    backdrop-filter: blur(10px);
    -webkit-backdrop-filter: blur(10px);
    padding: 10px 20px;

    @media only screen and (max-width: $mobile-break) {
      max-width: 100%;
    }

    @media only screen and (max-width: 390px) {
      padding: 5px 10px;
    }

    #header {
      height: 50px;
    }

    #content {
    }

    #footer {
      height: 50px;
      img {
        height: 90%;
      }
    }
  }
}
</style>
