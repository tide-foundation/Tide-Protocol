import Vue from "vue";
import App from "./App.vue";
import store from "./store";
import "./assets/css/main.scss";
import Tide from "tide-js";
import config from "./assets/js/config";
import helper from "./assets/js/tide-helper";
import axios from "axios";

import router from "./router";
Vue.config.productionTip = false;

// Development profile
// store.commit('storeUser', {
//   username: "thrakmar@gmail.com",
//   account: "tidexhsuridk",
//   keys: {
//     tide: {
//       pub: "EOS54P6fHzGyr1SSTYusmA6BhuEhJYuY5uZUHqQBzMeadk2jdbJey",
//       priv: "5J9mJizKfGrFdnSZNswomzTeoVoLi3649YdrHGwT3EQTCTPLf3Z"
//     },
//     vendor: {
//       pub: "ANH+mAyO2/f9i53isvp6BG8mEcJyy2C5CvDlexRLHR+jOSJwIOFb2d3Kp5kiDlSzjcHn4ByjIuxzwlWrRTg2h/GUp8OdkQVviV1KUhIajEqtdMJVAZ7bCpn0nCzgmLG40A==",
//       priv: "AtH+mAyO2/f9i53isvp6BG8mEcJyy2C5CvDlexRLHR+jOSJwIOFb2d3Kp5kiDlSzjcHn4ByjIuxzwlWrRTg2h/E4HOBbB5EIoFFjxIcMQZ2L4uR5qCPVn0jv9w0sMAubmQ=="
//     }
//   },
//   trustee: false,
//   tide: 0
// });

store.commit("storeDetails", config.mockData[0]);

var urls = [...Array(3)].map((_, i) => `https://ork-${i}.azurewebsites.net`);

Vue.prototype.$tide = new Tide(
  "VendorId",
  "https://tidevendor.azurewebsites.net/vendor",
  urls
);

Vue.prototype.$loading = (a, m) =>
  store.commit("updateLoading", {
    active: a,
    text: m
  });
Vue.prototype.$config = config;
Vue.prototype.$bus = new Vue();
Vue.prototype.$helper = new helper(
  Vue.prototype.$tide,
  Vue.prototype.$bus,
  store
);
Vue.prototype.$http = axios;

Vue.prototype.$bus.$on("toggle-tide", () =>
  Vue.prototype.$helper.toggleTide(store.getters.user.keys.vendor)
);

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount("#app");
