import { BUS_KEY } from "./assets/ts/Constants";
import { createApp } from "vue";
import App from "./App.vue";
import router from "./router/router";

import VueTippy from "vue-tippy";
import "tippy.js/dist/tippy.css"; // optional for styling

import Bus from "./assets/ts/Bus";

import "@/assets/styles/main.scss";

var app = createApp(App)
  .provide(BUS_KEY, Bus)
  .use(router)
  .use(VueTippy, {});

app.mount("#app");
