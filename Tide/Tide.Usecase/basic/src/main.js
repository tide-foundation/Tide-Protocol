import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";

import Tide from "../../../../Tide.Js/dist/tide.umd";

var tide = new Tide();
tide.sayHello();

import "./tide.web.js";
console.log(Tide);
console.log(Tide);
import Tide from "../../../../Tide.Js/src/Tide";
import "../src/assets/scss/main.scss";

Vue.config.productionTip = false;
Vue.prototype.$bus = new Vue();

Vue.prototype.$tide = new Tide(
    "VendorId",
    store.getters.vendorUrl,
    store.getters.orks.map((o) => o.url),
    "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAANvjzMxmyjGxse3fwkqajZxhf088eQRgS4l9wKsnm+A2+HRLt/4n6lA0cO6pmBqB9Le72HFSQ1s9cjv6HF3O2m", [{ name: "mandatory", condition: "true" }]
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
