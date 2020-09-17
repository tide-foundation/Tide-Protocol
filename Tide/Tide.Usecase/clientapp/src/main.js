import Vue from "vue";
import App from "./App.vue";
import store from "./store";
import "./assets/css/main.scss";
import Tide from "../../../Tide.Js/src/Tide";
import config from "./assets/js/config";
import helper from "./assets/js/tide-helper";
import axios from "axios";

import router from "./router";
Vue.config.productionTip = false;

{
  // Init tide
  var vendorUrl = "https://tidevendor.azurewebsites.net";
  //var vendorUrl = "http://127.0.0.1:6001";

  var orks = [];
  for (let i = 0; i < 10; i++) {
    orks.push({
      id: i,
      url: `https://ork-${i}.azurewebsites.net`,
      // url: `http://localhost:500${i + 1}`,
      cmk: false,
      cvk: false,
    });
  }

  const shuffled = orks.sort(() => 0.5 - Math.random());
  for (let i = 0; i < 3; i++) {
    shuffled[i].cmk = true;
  }
  for (let i = 3; i < 7; i++) {
    shuffled[i].cvk = true;
  }
  Vue.prototype.$orks = orks;

  var urls = Vue.prototype.$orks.map((o) => o.url);
  Vue.prototype.$tide = new Tide("VendorId", vendorUrl, urls, "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAANvjzMxmyjGxse3fwkqajZxhf088eQRgS4l9wKsnm+A2+HRLt/4n6lA0cO6pmBqB9Le72HFSQ1s9cjv6HF3O2m", ["mandatory"]);
}

Vue.prototype.$loading = (a, m) =>
  store.commit("updateLoading", {
    active: a,
    text: m,
  });
Vue.prototype.$config = config;
Vue.prototype.$bus = new Vue();
Vue.prototype.$helper = new helper();
Vue.prototype.$http = axios;

Vue.use(require("vue-moment"));

new Vue({
  router,
  store,
  render: (h) => h(App),
}).$mount("#app");
