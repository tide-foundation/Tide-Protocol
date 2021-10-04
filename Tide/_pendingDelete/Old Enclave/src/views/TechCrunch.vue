<template>
  <div id="tech-crunch">
    <img src="../assets/img/tech-crunch.png" alt="" />
  </div>
</template>

<script>
export default {
  data() {
    return {
      error: null,
    };
  },
  created() {},
};
</script>

<style lang="scss" scoped>
#tech-crunch {
  display: flex;
  flex-direction: column;

  justify-content: center;
  align-items: center;
  img {
    margin-top: 100px;
    width: 300px;
  }
}
</style>
