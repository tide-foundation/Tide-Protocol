import Vue from "vue";
import Router from "vue-router";
import Layout from "./views/Layouts/Layout.vue";
import Home from "./views/Home.vue";
import Property from "./views/Property.vue";
import Thanks from "./views/Thanks.vue";
import NotReal from "./views/NotReal.vue";
import Apply from "./views/Apply.vue";
import Auth from "./views/Auth.vue";
import Dashboard from "./views/Dashboard.vue";
import Profile from "./views/Profile.vue";
import VendorBackend from "./views/VendorBackend.vue";

import Revert from "./views/Revert.vue";
import Store from "./store";

Vue.use(Router);

var router = new Router({
  mode: "history",
  base: process.env.BASE_URL,
  routes: [
    {
      path: "/",
      component: Layout,
      children: [
        {
          path: "/",
          name: "home",
          component: Home,
          meta: [],
        },

        {
          path: "/property",
          name: "property",
          component: Property,
          meta: [],
        },
        {
          path: "/apply",
          name: "apply",
          component: Apply,
          meta: [],
        },
        {
          path: "/thanks",
          name: "thanks",
          component: Thanks,
          meta: [],
        },
        {
          path: "/profile",
          name: "profile",
          component: Profile,
          meta: ["auth"],
        },
        {
          path: "/dashboard",
          name: "dashboard",
          component: Dashboard,
          meta: [],
        },
        {
          path: "/not-real",
          name: "notreal",
          component: NotReal,
          meta: [],
        },
        {
          path: "/revert",
          name: "revert",
          component: Revert,
          meta: [],
        },
      ],
    },

    {
      path: "/auth",
      name: "Auth",
      component: Auth,
      meta: [],
    },
    {
      path: "/vendorBackend",
      name: "VendorBackend",
      component: VendorBackend,
      meta: [],
    },
  ],
});

router.beforeEach(async (to, from, next) => {
  if (to.meta.includes("auth") && Store.getters.user == null) return router.app.$bus.$emit("route-change", "auth");
  router.app.$bus.$emit("route-change", to.name);
  next();
});

export default router;
