import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import Tide from "../../../../Tide.Js/src/Tide";
import "../src/assets/scss/main.scss";

Vue.config.productionTip = false;
Vue.prototype.$bus = new Vue();
Vue.prototype.$tide = new Tide(
  "VendorId",
  "https://tidevendor.azurewebsites.net/",
  store.getters.orks.map((o) => o.url),
  "publickey"
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
