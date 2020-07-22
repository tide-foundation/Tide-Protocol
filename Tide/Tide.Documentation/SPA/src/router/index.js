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
        path: "/docs/about",
        component: () => import(/* webpackChunkName: "about" */ "../views/docs/pages/About.vue"),
      },
      {
        path: "/docs/tech-summary",
        component: () => import(/* webpackChunkName: "techsummary" */ "../views/docs/pages/TechSummary.vue"),
      },
      {
        path: "/docs/high-level",
        component: () => import(/* webpackChunkName: "highlevel" */ "../views/docs/pages/HighLevel.vue"),
      },
      {
        path: "/docs/how-it-works",
        component: () => import(/* webpackChunkName: "howitworks" */ "../views/docs/pages/HowItWorks.vue"),
      },
      {
        path: "/docs/not-created",
        component: () => import(/* webpackChunkName: "null" */ "../views/docs/pages/Null.vue"),
      },
      {
        path: "/docs/tide-js",
        component: () => import(/* webpackChunkName: "null" */ "../views/docs/api/TideJS.vue"),
      },
      {
        path: "/docs/tide-csharp",
        component: () => import(/* webpackChunkName: "null" */ "../views/docs/api/TideCSharp.vue"),
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
