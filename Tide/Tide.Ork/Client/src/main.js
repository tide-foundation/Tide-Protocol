import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import vueDebounce from "vue-debounce";

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

window.onload = function() {
  window.opener.postMessage({ type: "tide-onload", isDone: true }, window.name);
};

// window.addEventListener("message", handleMessage, false);

// function handleMessage(e) {
//   console.log("got this", e);
//   // // Reference to element for data display
//   // var el = document.getElementById('display');
//   // // Check origin
//   // if ( e.origin === 'http://www.example.com' ) {
//   //     // Retrieve data sent in postMessage
//   //     el.innerHTML = e.data;
//   //     // Send reply to source of message
//   //     e.source.postMessage('Message received', e.origin);
//   // }
// }
