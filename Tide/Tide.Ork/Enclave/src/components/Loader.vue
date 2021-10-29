<template>
  <div id="loader" class="f-c full-width full-height">
    <div class="loading-cubes">
      <div class="cube box1"></div>
      <div class="cube box2"></div>
      <div class="cube box3"></div>
      <div class="cube box4"></div>
      <div class="cube-shadows">
        <div class="shadow box1"></div>
        <div class="shadow box2"></div>
        <div class="shadow box3"></div>
        <div class="shadow box4"></div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import Base from "@/assets/ts/Base";

export default class Loader extends Base {}
</script>

<style lang="scss" scoped>
#loader {
  z-index: 10;
  position: absolute;
  left: 0;
  top: 0;
  background-color: $background;

  .toggle {
    display: inline-block;
    background-color: #6e829a;
    color: white;
    padding: 1em;
    cursor: pointer;
    &:hover {
      background-color: darken(#6e829a, 10%);
    }
  }

  $w: 36px;
  $h: 42px;
  $xspace: $w/2.25;
  $yspace: $h/4 - 1;
  $speed: 0.5s;

  .loading-cubes {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-$h, -$w);
  }

  .cube,
  .shadow {
    width: $w;
    height: $h;
    position: absolute;
    transition: all ease 0.3s;
  }

  .cube {
    background: url("../assets/img/cube.svg") no-repeat top center;
  }
  .shadow {
    top: 75px;
    background: url("../assets/img/cube-shadow.svg") no-repeat top center;
  }
  .cube-shadows {
    opacity: 0.15;
    position: relative;
  }

  .pause {
    animation-play-state: paused !important;
  }

  .box1 {
    animation: cube1 $speed ease-out infinite;
  }
  .box2 {
    animation: cube2 $speed ease-out infinite;
  }
  .box3 {
    animation: cube3 $speed ease-out infinite;
    z-index: 2;
  }
  .box4 {
    animation: cube4 $speed ease-out infinite;
  }

  @keyframes cube1 {
    0% {
      transform: translate(0, 0);
    }
    50% {
      transform: translate($xspace, -$yspace);
    }
    100% {
      transform: translate($xspace * 2, 0);
    }
  }

  @keyframes cube2 {
    0%,
    50% {
      transform: translate($xspace * 2, 0);
    }
    100% {
      transform: translate($xspace * 3, $yspace);
    }
  }

  @keyframes cube3 {
    0% {
      transform: translate($xspace * 3, $yspace);
    }
    50% {
      transform: translate($xspace * 2, $yspace * 2);
    }
    100% {
      transform: translate($xspace, $yspace);
    }
  }

  @keyframes cube4 {
    0%,
    50% {
      transform: translate($xspace, $yspace);
    }
    100% {
      transform: translate(0, 0);
    }
  }
}
</style>
