import Vue from "vue";
import VueRouter from "vue-router";
import admin from "../views/Admin.vue";
import C00010 from "../views/C00010.vue";
import C00020 from "../views/C00020.vue";
import O00010 from "../views/O00010.vue";
import O00020 from "../views/O00020.vue";
import M00010 from "../views/M00010.vue";
import R00010 from "../views/R00010.vue";

Vue.use(VueRouter);

const routes = [
  // config <router-view /> under App.vue

  // config <router-view /> under component Admin
  {
    path: "/",
    name: "Admin",
    component: admin,
    children: [
      { path: "C00010", name: "C00010", component: C00010 },
      { path: "C00020", name: "C00020", component: C00020 },
      { path: "O00010", name: "O00010", component: O00010 },
      { path: "O00020", name: "O00020", component: O00020 },
      { path: "M00010", name: "M00010", component: M00010 },
      { path: "R00010", name: "R00010", component: R00010 }
    ]
  }
];
const router = new VueRouter({
  mode: "history",
  base: "/Admin",
  routes
});

export default router;
