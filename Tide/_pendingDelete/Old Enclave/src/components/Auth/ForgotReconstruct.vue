<template>
  <div>
    <h2>Account Recovery</h2>
    <form @submit.prevent="$parent.changeMode('ForgotReset')">
      <!-- <div class="input-list">
        <input
          type="text"
          class="mt-50"
          v-for="(frag, index) in user.frags"
          :key="index"
          v-model="user.frags[index]"
          placeholder="Paste fragment here"
        />

      </div> -->

      <div id="frags">
        <div class="frag mb-10" v-for="(frag, index) in user.frags" :key="index">
          <input type="text" placeholder="Paste fragment here" required v-model="user.frags[index]" />
          <i v-if="index == 0" class="fas fa-plus" @click="user.frags.push('')"></i>
          <i v-if="index != 0" class="fas fa-minus" @click="user.frags.splice(index, 1)"></i>
        </div>
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

<style lang="scss" scoped>
#frags {
  max-height: 200px;
  overflow-y: auto;
  .frag {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
    align-items: center;
    i {
      width: 20px;
      margin: 0 10px;
      color: #999999;
      cursor: pointer;
      &:hover {
        color: orange;
      }
    }
  }
}
</style>
