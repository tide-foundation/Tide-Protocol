<template>
  <span>
    <h2>Select my ORKs</h2>
    <form @submit.prevent="clickedDone">
      <div class="input-list">
        <input type="text" class="mt-50" v-for="(ork, index) in orks" :key="index" v-model="orks[index].url" :placeholder="`ORK ${index + 1}`" />
      </div>
      <div class="center">
        <button type="button" @click="$bus.$emit('show-status', 'Lets get a global ork list going before I do this (:')" id="random-button" class="mt-20">Randomise All</button>
      </div>

      <div class="action-row mt-50">
        <p @click="$parent.changeMode('Register')">Back to Register</p>
        <button type="submit">Done</button>
      </div>
    </form>
  </span>
</template>

<script>
export default {
  props: ["user"],
  data() {
    return {
      orks: [],
    };
  },
  created() {
    this.orks = Array.from(this.user.selectedOrks);
  },
  methods: {
    clickedDone() {
      this.user.selectedOrks = this.orks;
      this.$parent.changeMode("Register");
    },
  },
};
</script>

<style lang="scss" scoped>
#random-button {
  height: 40px;
  width: auto;
  padding: 0 20px;
  background: orange;
  border-color: orange;
  &:hover {
    background: rgb(226, 147, 0);
    border-color: rgb(226, 147, 0);
  }
}
</style>
