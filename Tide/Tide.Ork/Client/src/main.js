import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";

import "../src/assets/scss/main.scss";

Vue.config.productionTip = false;
Vue.prototype.$bus = new Vue();

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

window.addEventListener(
  "message",
  (e) => {
    if (e.data.type == "tide-init") {
      store.dispatch("initializeTide", e.data);
    }
  },
  false
);

window.onload = function() {
  window.opener.postMessage({ type: "tide-onload", isDone: true }, window.name);
};
