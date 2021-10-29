<template> </template>

<script lang="ts">
import Base from "@/assets/ts/Base";
import { SESSION_ACCOUNT_KEY, SESSION_DATA_KEY } from "@/assets/ts/Constants";

export default class Initializing extends Base {
  stringData: string = "";

  created() {
    // Init config
    this.mainStore.initialize(this.parseConfig());

    // Apply styles
    this.applyOverrides();

    this.silentLogin();

    // If not logged in
    if (this.mainStore.getState.account == null) this.router.push("/login");
    // If silent logged in and has form data
    else if (this.mainStore.getState.config.formData != null) this.router.push("/form");
    // At this stage we silently logged in and there is no form data. Show an action screen
    else this.router.push("/actions");
  }

  parseConfig(): Config {
    try {
      // Fetch data from query
      const urlSearchParams = new URLSearchParams(window.location.search);

      this.stringData = Object.fromEntries(urlSearchParams.entries()).data;

      // Store into session data to preserve refresh
      if (this.stringData != null) {
        sessionStorage.setItem(SESSION_DATA_KEY, this.stringData);

        // Save url for change-ork flow
        var trueUrl = window.location.search;
        sessionStorage.setItem("true-url", trueUrl);
      } else {
        // If query is null, try get from session. User may have refreshed
        var sessionData = sessionStorage.getItem(SESSION_DATA_KEY);
        if (sessionData == null) throw "Missing config data";
        this.stringData = sessionData;
      }

      // Auth requires decoding, but form does not... need to find a way to
      // tell them apart. Because this double try catch is absolute garbage.
      // Return the decoded and parsed data
      return JSON.parse(decodeURIComponent(this.stringData)) as Config;
    } catch (e) {
      try {
        return JSON.parse(this.stringData) as Config;
      } catch (error) {
        throw e;
      }
    }
  }

  silentLogin() {
    // See if a session account is active
    var sessionAccount = sessionStorage.getItem(SESSION_ACCOUNT_KEY);
    if (sessionAccount != null) this.mainStore.setAccount(JSON.parse(sessionAccount), "Login");
  }

  applyOverrides() {
    try {
      const styleSheet = this.mainStore.getState.config.styles?.stylesheet;
      if (styleSheet != null) {
        var sheet = document.createElement("style");
        sheet.innerHTML = styleSheet;
        document.body.appendChild(sheet);
      }
    } catch (error) {
      console.log("Error occured in stylesheet", error);
    }
  }
}
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
