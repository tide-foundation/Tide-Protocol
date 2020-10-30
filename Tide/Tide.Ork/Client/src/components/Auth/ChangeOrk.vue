<template>
  <span>
    <h2>Change Home ORK</h2>
    <form @submit.prevent="changeOrk">
      <input type="text" class="mt-50" v-model="homeOrk" placeholder="Enter ORK Address" />

      <div class="action-row mt-50">
        <p @click="$parent.changeMode('LoginUsername')">Back to sign in</p>
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
      homeOrk: "",
    };
  },
  created() {
    this.homeOrk = window.location.protocol + "//" + window.location.host;
  },
  methods: {
    changeOrk() {
      window.location.href = this.homeOrk;
    },
  },
};
</script>

<style lang="scss" scoped></style>
