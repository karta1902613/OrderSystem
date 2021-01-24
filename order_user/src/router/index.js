import Vue from "vue";
import VueRouter from "vue-router";
import Home from "../views/Home.vue";
import Order from "../views/Order.vue"
Vue.use(VueRouter);

const routes = [

    { path: '/Order', name: 'Order', component: Order }

];

const router = new VueRouter({
    mode: "history",
    base: process.env.BASE_URL,
    routes
});



export default router;