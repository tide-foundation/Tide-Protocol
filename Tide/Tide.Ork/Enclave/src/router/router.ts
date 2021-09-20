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
    path: "/auth-layout",
    component: () => import(/* webpackChunkName: "auth-layout" */ "../views/auth/Auth-Layout.vue"),
    meta: {
      requiresInit: true,
    },
    children: [
      {
        path: "/login",
        name: "Login",
        component: () => import(/* webpackChunkName: "login" */ "../views/auth/Login.vue"),
      },
      {
        path: "/register",
        name: "Register",
        component: () => import(/* webpackChunkName: "register" */ "../views/auth/Register.vue"),
      },
    ],
  },
  {
    path: "/layout",
    component: () => import(/* webpackChunkName: "layout" */ "../views/Layout.vue"),
    meta: {
      requiresInit: true,
    },
    children: [
      {
        path: "/form",
        name: "Form",
        component: () => import(/* webpackChunkName: "form" */ "../views/Form.vue"),
      },
      {
        path: "/actions",
        name: "Actions",
        component: () => import(/* webpackChunkName: "actions" */ "../views/Actions.vue"),
      },
    ],
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

router.beforeEach(async (to, from, next) => {
  if (to.matched.some((record) => record.meta.requiresInit) && mainStore.getState.tide == null) return next("/");
  return next();
});

export default router;
