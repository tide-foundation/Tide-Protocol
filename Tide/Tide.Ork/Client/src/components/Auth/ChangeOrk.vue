<template>
  <span>
    <h2>Change Home ORK</h2>
    <form @submit.prevent="changeOrk">
      <input type="text" class="mt-50" v-model="homeOrk" placeholder="Enter ORK Address" />

      <div class="action-row mt-50">
        <p @click="$parent.changeMode('LoginUsername')">Back to sign in</p>
        <button type="submit">Change</button>
      </div>
    </form>
  </span>
</template>

<script>
export default {
  props: ["user"],
  data() {
    return {
      homeOrk: "",
    };
  },
  created() {
    this.homeOrk = "http://172.26.17.60:8082"; //`${this.$store.getters.origin}`
  },
  methods: {
    changeOrk() {
      this.$store.dispatch("changeOrkWindow", this.homeOrk);
      //   window.location.href = this.homeOrk;
    },
  },
};
</script>

<style lang="scss" scoped></style>
