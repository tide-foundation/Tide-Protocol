<template>
  <div>
    <h1>
      INITIALIZING
    </h1>
    <br />
    <h2>
      PLEASE WAIT
    </h2>

    <p v-if="error != null">{{ error.msg }}</p>
  </div>
</template>

<script>
export default {
  data() {
    return {
      error: null,
    };
  },
  created() {
    this.$bus.$on("initError", (e) => {
      this.error = e;
      console.log(this.error);
    });
  },
};
</script>

<style scoped lang="scss">
h1 {
  text-align: center;
  font-weight: 400;

  margin: 70px 0 0 0;
  line-height: 20px;
}

h2 {
  font-weight: 400;
  font-size: 14px;
  margin: 0px;
}

p {
  color: red;
}
</style>
