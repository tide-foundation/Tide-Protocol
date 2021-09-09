<template>
  <div id="loader" class="f-c full-width full-height">
    <div
      v-for="(bolt, i) in bolts"
      :key="i"
      class="lighting-container"
      :style="{ transform: `translate(${bolt.translateX}px, 6px) rotate(${bolt.rotation}deg)` }"
    >
      <div class="lightning-holder full-width full-height f-c">
        <div class="lightning" :style="{ color: bolt.color, animationDelay: `${bolt.delay}s` }"></div>
      </div>
    </div>
    <div
      v-for="(bolt, i) in bolts"
      :key="i"
      class="destination-circle"
      :style="{
        transform: `translate(${bolt.boomTranslateX}px, ${bolt.boomTranslateY}px) rotate(0deg)`,
        transformOrigin: `0px 100px`,
      }"
    ></div>

    <div id="home-node"></div>
  </div>
</template>

<script setup lang="ts">
interface BoltSetup {
  translateX: number;
  rotation: number;
  color: string;
  delay: number;
  boomTranslateX: number;
  boomTranslateY: number;
  boomOriginX: number;
  boomOriginY: number;
}

import { ref, computed } from "vue";

const bolts: BoltSetup[] = [
  {
    translateX: 0,
    rotation: 0,
    color: "#2f77e4",
    delay: 0,
    boomTranslateX: 0,
    boomTranslateY: -210,
    boomOriginX: 20,
    boomOriginY: 70,
  },
  //   {
  //     translateX: 26,
  //     rotation: 30,
  //     color: "#2f77e4",
  //     delay: 0.1,
  //     boomTranslateX: 130,
  //     boomTranslateY: -185,
  //   },
  //   {
  //     translateX: -26,
  //     rotation: -30,
  //     color: "#2f77e4",
  //     delay: 0.2,
  //     boomTranslateX: -130,
  //     boomTranslateY: -185,
  //   },
  //   {
  //     translateX: 12,
  //     rotation: 15,
  //     color: "#2f77e4",
  //     delay: 0.4,
  //     boomTranslateX: 65,
  //     boomTranslateY: -200,
  //   },
  //   {
  //     translateX: -12,
  //     rotation: -15,
  //     color: "#2f77e4",
  //     delay: 0.5,
  //     boomTranslateX: -65,
  //     boomTranslateY: -200,
  //   },
];
</script>

<style lang="scss" scoped>
#loader {
  z-index: 10;
  position: absolute;
  left: 0;
  top: 0;
  background-color: $background;

  opacity: 0.95;

  #home-node {
    border-radius: 20px;
    width: 25px;
    height: 25px;

    background-color: rgb(53, 171, 250);
    transform: translate(0px, 40px);
  }

  body {
    display: flex;
    align-items: center;
    position: relative;
    background: linear-gradient(to bottom right, #070630 0%, #060454 100%);
    min-height: 100vh;
  }
  .lighting-container {
    .lightning-holder {
      display: flex;

      $blue: #2f77e4;

      .lightning {
        position: absolute;
        display: block;
        height: 4px;
        width: 12px;
        border-radius: 12px;
        margin-top: 100px;
        animation-name: woosh;
        animation-duration: 0.8s;
        animation-iteration-count: infinite;
        animation-timing-function: cubic-bezier(0.445, 0.05, 0.55, 0.95);
        //animation-direction: alternate;
        background-color: $blue;
      }
    }
  }

  .destination-circle {
    position: absolute;
    background-color: transparent;
    border: 2px solid blue;
    width: 20px;
    height: 20px;
    border-radius: 20px;
    animation-name: boom-circle;
    animation-duration: 1s;
    animation-timing-function: ease-out;
    animation-iteration-count: infinite;
  }

  @keyframes woosh {
    0% {
      width: 12px;
      transform: translate(0px, 0px) rotate(90deg);
    }
    50% {
      width: 30px;
    }

    100% {
      width: 12px;
      transform: translate(0, -250px) rotate(90deg);
    }
  }

  @keyframes boom-circle {
    0% {
      opacity: 0;
    }
    5% {
      opacity: 1;
    }
    30% {
      opacity: 0;
      transform: scale(3);
    }
    100% {
    }
  }
}
</style>
