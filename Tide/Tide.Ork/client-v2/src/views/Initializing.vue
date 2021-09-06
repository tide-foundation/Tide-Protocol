<template>
  <div id="initializing" class="full-height f-c">
    <h1>INITIALIZING</h1>

    <div class="error" v-if="error != ''">
      {{ error }}
    </div>
  </div>
</template>

<script setup lang="ts">
import mainStore from "@/store/mainStore";
import { ref, onMounted } from "vue";
import router from "@/router/router";

var error = ref("");

onMounted(() => {
  mainStore.initialize(parseConfig());

  router.push("/login");
});

const parseConfig = (): Config => {
  try {
    // Fetch data from query
    const urlSearchParams = new URLSearchParams(window.location.search);
    let data = Object.fromEntries(urlSearchParams.entries()).data;

    // Store into session data to preserve refresh
    if (data != null) sessionStorage.setItem("data", data);
    else {
      // If query is null, try get from session. User may have refreshed
      var sessionData = sessionStorage.getItem("data");
      if (sessionData == null) throw "Missing config data";
      data = sessionData;
    }

    // Return the decoded and parsed data
    return JSON.parse(decodeURIComponent(data)) as Config;
  } catch (e) {
    //error.value = "sdfsfdssfd";
    throw e;
  }
};
</script>

<style lang="scss" scoped>
#initializing {
  padding: 20px;
  background-color: white;
  border-radius: $border-radius;

  h1 {
    font-size: 3rem;
  }
  .error {
  }
}
</style>
