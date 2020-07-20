import Vue from "vue";
import VueRouter from "vue-router";
import Docs from "../views/docs/Docs.vue";

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Docs",
    component: Docs,
    children: [
      {
        path: "/docs/summary",
        component: () => import(/* webpackChunkName: "about" */ "../views/docs/pages/Summary.vue"),
      },
      {
        path: "/docs/high-level",
        component: () => import(/* webpackChunkName: "about" */ "../views/docs/pages/HighLevel.vue"),
      },
      {
        path: "/docs/how-it-works",
        component: () => import(/* webpackChunkName: "about" */ "../views/docs/pages/HowItWorks.vue"),
      },
    ],
  },
  {
    path: "/about",
    name: "About",
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import(/* webpackChunkName: "about" */ "../views/About.vue"),
  },
];

const router = new VueRouter({
  mode: "history",
  base: process.env.BASE_URL,
  routes,
});

export default router;
