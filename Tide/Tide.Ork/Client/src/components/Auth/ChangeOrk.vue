<template>
  <span>
    <h2>Change Home ORK</h2>
    <form @submit.prevent="login">
      <input type="text" class="mt-50" v-model="user.homeOrk" placeholder="Enter ORK Address" />

      <div class="action-row mt-50">
        <p @click="$parent.changeMode('LoginUsername')">Back to sign in</p>
        <button @click="$bus.$emit('show-error', 'Hey Matt, you need to do this logic')">Done</button>
      </div>
    </form>
  </span>
</template>

<script>
export default {
  props: ["user"],
};
</script>

<style lang="scss" scoped></style>
