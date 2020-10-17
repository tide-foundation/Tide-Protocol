import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";

import Tide from "../../../Tide.Js/src/Sdk/TideAuthentication";

import "../src/assets/scss/main.scss";

window.addEventListener(
  "message",
  (e) => {
    if (e.data.type == "tide-init") {
      store.dispatch("initializeTide", e.data);
    }
  },
  false
);

Vue.config.productionTip = false;
Vue.prototype.$bus = new Vue();

Vue.prototype.$tide = new Tide(
  "VendorId",
  store.getters.vendorUrl,
  store.getters.orks.map((o) => o.url)
);

Vue.prototype.$loading = (a, m) =>
  store.commit("UPDATE_LOADING", {
    active: a,
    text: m,
  });

new Vue({
  router,
  store,
  render: (h) => h(App),
}).$mount("#app");
window.onload = function() {
  window.opener.postMessage({ type: "tide-onload", isDone: true }, "http://192.168.0.205:8081/");
};
