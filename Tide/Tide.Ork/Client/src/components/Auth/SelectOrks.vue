<template>
  <span>
    <h2>Select my ORKs</h2>
    <form @submit.prevent="clickedDone">
      <div id="orks">
        <input type="text" class="mt-50" v-for="(ork, index) in orks" :key="index" v-model="orks[index]" :placeholder="`ORK ${index + 1}`" />
      </div>
      <div class="center">
        <button id="random-button" class="mt-20">Randomise All</button>
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
#orks {
  overflow-y: auto;
  max-height: 240px !important;
  input {
    margin: 0px 0px 10px 0;
    height: 40px;
  }

  &::-webkit-scrollbar {
    width: 4px;
  }

  &::-webkit-scrollbar-track {
    background-color: rgb(190, 190, 190);
  }

  &::-webkit-scrollbar-thumb {
    background-color: rgb(66, 66, 66);
  }
}

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
