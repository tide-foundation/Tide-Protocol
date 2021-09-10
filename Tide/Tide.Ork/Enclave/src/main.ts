import { BUS_KEY } from "./assets/ts/Constants";
import { createApp } from "vue";
import App from "./App.vue";
import router from "./router/router";

import "@/assets/styles/main.scss";
import Bus from "./assets/ts/Bus";

createApp(App)
  .provide(BUS_KEY, Bus)
  .use(router)
  .mount("#app");
