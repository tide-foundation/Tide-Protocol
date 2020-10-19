<template>
  <div id="home">
    <h1>THIS IS AN EXAMPLE VENDOR WEBSITE</h1>
    <h2>PLEASE LOGIN OR REGISTER</h2>
    <div id="btn-bar"><button @click="$router.push('/dashboard')">Dashboard</button> <span id="tide"></span></div>
  </div>
</template>

<script>
export default {
  name: "Home",
};
</script>

<style lang="scss">
#home {
  display: flex;
  justify-content: center;
  flex-direction: column;
  text-align: center;

  #btn-bar {
    display: flex;
    justify-content: center;
    flex-direction: row;
    button {
      margin: 10px;
    }
  }
}
</style>
