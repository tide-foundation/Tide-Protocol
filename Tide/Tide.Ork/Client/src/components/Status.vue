<template>
  <div id="status" :class="{ error: isError }">{{ status }}</div>
</template>

<script>
export default {
  data() {
    return {
      status: "",
      isError: false,
    };
  },
  created() {
    this.$bus.$on("show-status", (text) => {
      this.isError = false;
      this.status = text;
      setTimeout(async () => {
        this.status = "";
      }, 5000);
    });

    this.$bus.$on("show-error", (error) => {
      this.isError = true;
      this.status = error;
      setTimeout(async () => {
        this.status = "";
      }, 5000);
    });
  },
};
</script>

<style lang="scss" scoped>
#status {
  color: green;
  position: absolute;
  top: 10px;
  &.error {
    color: red;
  }
}
</style>
