import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import vueDebounce from "vue-debounce";
import { getHashParams } from "./assets/js/helpers";

import "../src/assets/scss/main.scss";

Vue.config.productionTip = false;
Vue.prototype.$bus = new Vue();

Vue.prototype.$loading = (a, m) =>
  store.commit("UPDATE_LOADING", {
    active: a,
    text: m,
  });

Vue.use(vueDebounce);

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

var isIframe = window.self !== window.top;

Vue.prototype.$bus.source = isIframe ? window.top : window.opener;
Vue.prototype.$bus.origin = isIframe ? getHashParams().origin : window.name;

Vue.prototype.$bus.source.postMessage({ type: "tide-onload", isDone: true }, Vue.prototype.$bus.origin);
