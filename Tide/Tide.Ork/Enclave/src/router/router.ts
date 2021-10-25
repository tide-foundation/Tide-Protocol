import mainStore from "@/store/mainStore";
import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";

import AuthLayout from "@/views/auth/Auth-Layout.vue";
import Login from "@/views/auth/Login.vue";
import Register from "@/views/auth/Register.vue";
import Forgot from "@/views/auth/Forgot.vue";
import Reconstruct from "@/views/auth/Reconstruct.vue";

import Layout from "@/views/Layout.vue";
import Form from "@/views/Form.vue";
import Actions from "@/views/Actions.vue";
import Options from "@/views/Options.vue";

import Account from "@/views/Account.vue";
import ChangePassword from "@/views/ChangePassword.vue";

import Initializing from "../views/Initializing.vue";

const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    name: "Initializing",
    component: Initializing,
  },
  {
    path: "/auth-layout",
    component: AuthLayout,
    meta: {
      requiresInit: true,
    },
    children: [
      {
        path: "/login",
        name: "Login",
        component: Login,
      },
      {
        path: "/register",
        name: "Register",
        component: Register,
      },
      {
        path: "/forgot",
        name: "Forgot",
        component: Forgot,
      },
    ],
  },
  {
    path: "/layout",
    component: Layout,
    meta: {
      requiresInit: true,
    },
    children: [
      {
        path: "/form",
        name: "Form",
        component: Form,
      },
      {
        path: "/actions",
        name: "Actions",
        component: Actions,
      },
      {
        path: "/reconstruct",
        name: "Reconstruct",
        component: Reconstruct,
      },
      {
        path: "/account",
        name: "Account",
        component: Account,
      },
      {
        path: "/change-password",
        name: "Change Password",
        component: ChangePassword,
      },
      {
        path: "/options",
        name: "Options",
        component: Options,
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
