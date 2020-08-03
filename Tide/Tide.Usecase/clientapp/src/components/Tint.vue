<template>
  <div class="tint" :class="{ 'full-opacity': !tinted }"></div>
</template>

<script>
export default {
  data() {
    return {
      tinted: false
    };
  },
  created() {
    this.$bus.$on("tint", t => (this.tinted = t));
  }
};
</script>

<style scoped>
.tint {
  text-align: center;

  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: column;
  width: 100%;
  height: 100%;
  position: fixed;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  background-color: black;
  opacity: 0.7;
  transition: all 1s ease;
}

.full-opacity {
  pointer-events: none;
  opacity: 0 !important;
}
</style>
