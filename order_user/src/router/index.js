import Vue from "vue";
import VueRouter from "vue-router";
import store from '../store'
import axios from "axios";
import Order from "../views/Order.vue"
import Login from "../views/Login.vue"
Vue.use(VueRouter);

const routes = [
    { path: '/Login', name: 'Login', component: Login },
    { path: '/Order', name: 'Order', component: Order }

];

const router = new VueRouter({
    mode: "history",
    base: process.env.BASE_URL,
    routes
});


router.beforeEach((to, from, next) => {
    let isAuth = store.state.isAuthenticated
    if (to.name != 'Login' && !isAuth) {
        axios.get(store.state.api + 'Admin/islogin').then(res => {
            window.console.log(res.data)
            store.state.isAuthenticated = res.data
            if (res.data) {
                next({ name: 'Order' })
            } else {
                next({ name: 'Login' })
            }

        })
    } else if (to.name === 'Login' && !isAuth) {

        next()
    } else if (to.name === 'Login' && isAuth) {
        next({ name: 'Admin' })
    } else {
        next()
    }
})
export default router;