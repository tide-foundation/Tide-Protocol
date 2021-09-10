<template>
  <div>
    <transition name="fade" mode="out-in">
      <Overview v-if="mode == 'Overview'"></Overview>
      <Change v-if="mode == 'Change'"></Change>
    </transition>
  </div>
</template>

<script>
import Overview from "../components/Account/Overview.vue";
import Change from "../components/Account/ChangePassword.vue";
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
