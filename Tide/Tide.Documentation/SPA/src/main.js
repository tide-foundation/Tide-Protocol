import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store.js";
import "./assets/css/main.scss";

import VueHighlightJS from "vue-highlightjs";
Vue.use(VueHighlightJS);

Vue.config.productionTip = false;

Vue.prototype.$bus = new Vue();

new Vue({
  router,
  store,
  render: (h) => h(App),
}).$mount("#app");
