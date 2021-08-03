<template>
  <div>
    <h2>Account Recovery</h2>
    <form @submit.prevent="$parent.changeMode('ForgotReset')">
      <div class="input-list">
        <input
          type="text"
          class="mt-50"
          v-for="(frag, index) in user.frags"
          :key="index"
          v-model="user.frags[index]"
          placeholder="Paste fragment here"
        />
      </div>

      <div class="action-row mt-50">
        <p @click="$parent.changeMode('LoginPassword')">Back to Sign-in</p>
        <button type="submit">Assemble</button>
      </div>
    </form>
  </div>
</template>

<script>
export default {
  props: ["user"],
};
</script>

<style lang="scss" scoped></style>
