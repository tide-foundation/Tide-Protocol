<template>
  <div id="content">
    <div class="padded-container">
      <router-view />
    </div>
  </div>
</template>

<script>
export default {
  name: "Content",
};
</script>

<style scoped lang="scss">
#content {
  width: 100%;
}
</style>
