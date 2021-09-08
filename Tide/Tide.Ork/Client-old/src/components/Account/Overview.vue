<template>
  <div>
    <h2>Account</h2>
    <form>
      <button type="button" @click="$parent.changeMode('Change')">Change Password</button>
      <button type="button" @click="getMasterKey">Show my Vendor Key</button>
      <button type="button" @click="getMasterKey">Show my Master Key</button>
    </form>
  </div>
</template>

<script>
export default {
  created() {
    this.$store.dispatch("closeWindow");
  },
  methods: {
    getMasterKey() {
      this.$bus.$emit("show-status", "Hey guys, we don't really get these keys. Check console");
    },
  },
};
</script>

<style lang="scss" scoped>
form {
  width: 100%;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  button {
    height: 40px;
    margin-bottom: 15px;
    max-width: 270px;
  }
}
</style>
