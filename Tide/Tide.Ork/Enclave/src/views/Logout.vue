<template>
  <div id="logout">heyyyy</div>
</template>

<script lang="ts">
import Base from "@/assets/ts/Base";

export default class Login extends Base {
  created() {
    this.mainStore.logout();

    const urlParams = new URLSearchParams(window.location.search);
    const redirect = urlParams.get("redirect");

    if (redirect == null) return this.$router.push("/");

    window.location.href = redirect;
  }
}
</script>

<style lang="scss" scoped>
#logout {
}
</style>
