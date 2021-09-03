import mainStore from "@/store/mainStore";
import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import Initializing from "../views/Initializing.vue";

const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    name: "Initializing",
    component: Initializing,
  },
  {
    path: "/layout",
    component: () => import(/* webpackChunkName: "layout" */ "../views/Layout.vue"),
    meta: {
      requiresInit: true,
    },
    children: [
      {
        path: "/login",
        name: "Login",
        component: () => import(/* webpackChunkName: "login" */ "../views/Login.vue"),
      },
    ],
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

router.beforeEach(async (to, from, next) => {
  // await mainStore.waitForInitialization();

  //if (to.matched.some((record) => record.meta.requiresInit) && mainStore.getState.user == null) return next("/initializing");
  return next();
});

export default router;
