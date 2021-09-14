<template>
  <!-- <div id="initializing" class="full-height f-c">
    <h1>INITIALIZING</h1>

    <div class="error" v-if="error != ''">
      {{ error }}
    </div>
  </div> -->
</template>

<script setup lang="ts">
import mainStore from "@/store/mainStore";
import { ref, onMounted } from "vue";
import router from "@/router/router";
import { SESSION_ACCOUNT_KEY, SESSION_DATA_KEY } from "@/assets/ts/Constants";

var error = ref("");
var data = "";
onMounted(() => {
  // Init config
  mainStore.initialize(parseConfig());

  // Apply styles
  applyOverrides();

  if (mainStore.getState.account == null) router.push("/login");
  else router.push("/form");
});

const parseConfig = (): Config => {
  try {
    // See if a session account is active
    // var sessionAccount = sessionStorage.getItem(SESSION_ACCOUNT_KEY);
    // if (sessionAccount != null) mainStore.setAccount(JSON.parse(sessionAccount));

    // Fetch data from query
    const urlSearchParams = new URLSearchParams(window.location.search);

    data = Object.fromEntries(urlSearchParams.entries()).data;

    // Store into session data to preserve refresh
    if (data != null) sessionStorage.setItem(SESSION_DATA_KEY, data);
    else {
      // If query is null, try get from session. User may have refreshed
      var sessionData = sessionStorage.getItem(SESSION_DATA_KEY);
      if (sessionData == null) throw "Missing config data";
      data = sessionData;
    }

    // Auth requires decoding, but form does not... need to find a way to
    // tell them apart. Because this double try catch is absolute garbage.
    // Return the decoded and parsed data
    return JSON.parse(decodeURIComponent(data)) as Config;
  } catch (e) {
    try {
      return JSON.parse(data) as Config;
    } catch (error) {
      throw e;
    }
  }
};

const applyOverrides = () => {
  try {
    const styleSheet = mainStore.getState.config.styles?.stylesheet;
    if (styleSheet != null) {
      var sheet = document.createElement("style");
      sheet.innerHTML = styleSheet;
      document.body.appendChild(sheet);
    }
  } catch (error) {
    console.log("Error occured in stylesheet", error);
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
