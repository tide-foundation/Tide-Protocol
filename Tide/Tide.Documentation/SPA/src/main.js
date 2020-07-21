import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import "./assets/css/main.scss";

Vue.config.productionTip = false;

Vue.prototype.$bus = new Vue();

new Vue({
  router,
  render: (h) => h(App),
}).$mount("#app");