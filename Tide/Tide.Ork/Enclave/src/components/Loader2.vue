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

<script setup lang="ts">
import { ref, computed } from "vue";
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
    background: url("data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9Im5vIj8+Cjxzdmcgd2lkdGg9IjM2cHgiIGhlaWdodD0iNDJweCIgdmlld0JveD0iMCAwIDM2IDQyIiB2ZXJzaW9uPSIxLjEiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgeG1sbnM6eGxpbms9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkveGxpbmsiIHhtbG5zOnNrZXRjaD0iaHR0cDovL3d3dy5ib2hlbWlhbmNvZGluZy5jb20vc2tldGNoL25zIj4KICAgIDwhLS0gR2VuZXJhdG9yOiBTa2V0Y2ggMy4yLjIgKDk5ODMpIC0gaHR0cDovL3d3dy5ib2hlbWlhbmNvZGluZy5jb20vc2tldGNoIC0tPgogICAgPHRpdGxlPkN1YmU8L3RpdGxlPgogICAgPGRlc2M+Q3JlYXRlZCB3aXRoIFNrZXRjaC48L2Rlc2M+CiAgICA8ZGVmcz48L2RlZnM+CiAgICA8ZyBpZD0iUGFnZS0xIiBzdHJva2U9Im5vbmUiIHN0cm9rZS13aWR0aD0iMSIgZmlsbD0ibm9uZSIgZmlsbC1ydWxlPSJldmVub2RkIiBza2V0Y2g6dHlwZT0iTVNQYWdlIj4KICAgICAgICA8ZyBpZD0iSWNvbnMiIHNrZXRjaDp0eXBlPSJNU0FydGJvYXJkR3JvdXAiIHRyYW5zZm9ybT0idHJhbnNsYXRlKC0zNDAuMDAwMDAwLCAtNDQyLjAwMDAwMCkiPgogICAgICAgICAgICA8ZyBpZD0iQ3ViZSIgc2tldGNoOnR5cGU9Ik1TTGF5ZXJHcm91cCIgdHJhbnNmb3JtPSJ0cmFuc2xhdGUoMzQwLjAwMDAwMCwgNDQyLjAwMDAwMCkiPgogICAgICAgICAgICAgICAgPHBhdGggZD0iTTE4LDIxLjMzMzUyMjcgTDE4LDQyIEwzNiwzMSBMMzYsMTEgTDE4LDIxLjMzMzUyMjcgWiIgaWQ9IkZpbGwtMiIgZmlsbD0iI0M1NzYyRiIgc2tldGNoOnR5cGU9Ik1TU2hhcGVHcm91cCI+PC9wYXRoPgogICAgICAgICAgICAgICAgPHBhdGggZD0iTTAsMzEgTDE4LDQyIEwxOCwyMS4zMzM1MjI3IEwwLDExIEwwLDMxIFoiIGlkPSJGaWxsLTMiIGZpbGw9IiNFM0EzMkQiIHNrZXRjaDp0eXBlPSJNU1NoYXBlR3JvdXAiPjwvcGF0aD4KICAgICAgICAgICAgICAgIDxwYXRoIGQ9Ik0wLDExIEwxOCwyMiBMMzYsMTEgTDE4LDAgTDAsMTEgWiIgaWQ9IkZpbGwtMSIgZmlsbD0iI0ZCRDc0RSIgc2tldGNoOnR5cGU9Ik1TU2hhcGVHcm91cCI+PC9wYXRoPgogICAgICAgICAgICA8L2c+CiAgICAgICAgPC9nPgogICAgPC9nPgo8L3N2Zz4=")
      no-repeat top center;
  }
  .shadow {
    top: 75px;
    background: url("data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9Im5vIj8+Cjxzdmcgd2lkdGg9IjM2cHgiIGhlaWdodD0iMjJweCIgdmlld0JveD0iMCAwIDM2IDIyIiB2ZXJzaW9uPSIxLjEiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgeG1sbnM6eGxpbms9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkveGxpbmsiIHhtbG5zOnNrZXRjaD0iaHR0cDovL3d3dy5ib2hlbWlhbmNvZGluZy5jb20vc2tldGNoL25zIj4KICAgIDwhLS0gR2VuZXJhdG9yOiBTa2V0Y2ggMy4yLjIgKDk5ODMpIC0gaHR0cDovL3d3dy5ib2hlbWlhbmNvZGluZy5jb20vc2tldGNoIC0tPgogICAgPHRpdGxlPkZpbGwgNTwvdGl0bGU+CiAgICA8ZGVzYz5DcmVhdGVkIHdpdGggU2tldGNoLjwvZGVzYz4KICAgIDxkZWZzPjwvZGVmcz4KICAgIDxnIGlkPSJQYWdlLTEiIHN0cm9rZT0ibm9uZSIgc3Ryb2tlLXdpZHRoPSIxIiBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiIHNrZXRjaDp0eXBlPSJNU1BhZ2UiPgogICAgICAgIDxnIGlkPSJJY29ucyIgc2tldGNoOnR5cGU9Ik1TQXJ0Ym9hcmRHcm91cCIgdHJhbnNmb3JtPSJ0cmFuc2xhdGUoLTMwOC4wMDAwMDAsIC01MTMuMDAwMDAwKSIgZmlsbD0iIzAwMDAwMCI+CiAgICAgICAgICAgIDxwYXRoIGQ9Ik0zMDgsNTI0IEwzMjYsNTM1IEwzNDQsNTI0IEwzMjYsNTEzIEwzMDgsNTI0IFoiIGlkPSJGaWxsLTUiIHNrZXRjaDp0eXBlPSJNU1NoYXBlR3JvdXAiPjwvcGF0aD4KICAgICAgICA8L2c+CiAgICA8L2c+Cjwvc3ZnPg==")
      no-repeat top center;
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
