import Vue from "vue";
import VueRouter from "vue-router";
import Home from "../views/Home.vue";
import Store from "../store/";

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Home",
    component: Home,
    meta: [],
  },
  {
    path: "/auth",
    name: "Authenticate",
    component: () => import(/* webpackChunkName: "auth" */ "../views/Auth.vue"),
    meta: [],
  },
  {
    path: "/validate",
    name: "Validate",
    component: () => import(/* webpackChunkName: "Validate" */ "../views/Validate.vue"),
    meta: [],
  },
  {
    path: "/protected",
    name: "Protected",
    component: () => import(/* webpackChunkName: "protected" */ "../views/Protected.vue"),
    meta: ["auth"],
  },
];

const router = new VueRouter({
  mode: "history",
  base: process.env.BASE_URL,
  routes,
});

router.beforeEach((to, from, next) => {
  if (to.meta.includes("auth") && !Store.getters.loggedIn) {
    Vue.prototype.$bus.$emit("show-status", "Please login");

    return next({ name: "Authenticate" });
  } else next();
});

export default router;
