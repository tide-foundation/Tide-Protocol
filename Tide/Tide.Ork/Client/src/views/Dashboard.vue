<template>
  <div>
    <h1>DASHBOARD</h1>
    <Overview v-if="mode == 'Overview'"></Overview>
    <Change v-if="mode == 'Change'"></Change>
  </div>
</template>

<script>
import Overview from "../components/Dashboard/Overview.vue";
import Change from "../components/Dashboard/ChangePassword.vue";
export default {
  components: { Change, Overview },
  data() {
    return {
      mode: "Overview",
    };
  },
  methods: {
    changeMode(newMode) {
      this.mode = newMode;
    },
  },
};
</script>

<style></style>
