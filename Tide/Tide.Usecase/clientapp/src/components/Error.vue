<template>
  <div id="error-container" v-if="error != ''">{{error}}</div>
</template>

<script>
export default {
  data() {
    return {
      error: ""
    };
  },
  created() {
    this.$bus.$on("show-error", e => {
      console.log(e);
      this.error = e;
      setTimeout(async () => {
        this.error = "";
      }, 7000);
    });
  }
};
</script>

<style lang="scss" scoped>
#error-container {
  position: fixed;
  top: 15px;
  right: 15px;
  display: flex;
  align-items: center;
  color: white;
  z-index: 89000000000000000;
  background-color: rgba(0, 0, 0, 0.8);
  padding: 5px 10px;
  pointer-events: none;
}

img {
  height: 13px;
  margin-right: 10px;
}
</style>
