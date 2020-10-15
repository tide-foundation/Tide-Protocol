import Vue from "vue";
import VueRouter from "vue-router";
import Initializing from "../views/Initializing.vue";
import Store from "../store/";

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Initializing",
    component: Initializing,
  },
  {
    path: "/auth",
    name: "Auth",
    component: () => import(/* webpackChunkName: "auth" */ "../views/Auth.vue"),
  },
  {
    path: "/finished",
    name: "Finished",
    component: () => import(/* webpackChunkName: "finished" */ "../views/Finished.vue"),
  },
];

const router = new VueRouter({
  mode: "history",
  base: process.env.BASE_URL,
  routes,
});

router.beforeEach((to, from, next) => {
  if (to.path != "/" && !Store.getters.isInitialized) {
    return next({ name: "Initializing" });
  } else next();
});

export default router;
