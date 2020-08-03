<template>
  <div id="message-container" v-if="message != ''">
    {{ message }}
  </div>
</template>

<script>
export default {
  data() {
    return {
      message: ""
    };
  },
  created() {
    this.$bus.$on("show-message", m => {
      this.message = m;
      setTimeout(async () => {
        this.message = "";
      }, 5000);
    });
  }
};
</script>

<style lang="scss" scoped>
#message-container {
  position: fixed;
  top: 15px;
  left: 15px;
  display: flex;
  align-items: center;
  z-index: 99999999999999999999999;
  color: white;
  background-color: rgba(0, 0, 0, 0.8);
  padding: 5px 10px;
  pointer-events: none;
}
</style>
