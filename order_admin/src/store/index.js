import axios from "axios";
import Vue from "vue";
import Vuex from "vuex";
//import router from '../router/index.js'

Vue.use(Vuex);

export default new Vuex.Store({
    state: {
        menuTree: [],
        shopData: [],
        api: "https://localhost:44362/api/",
        isAuthenticated: false,
        claims: {},
    },
    mutations: {},
    actions: {
        async getMenuTree() {
            window.console.log("start to get menuTree");
            await axios.get(this.state.api + "Admin/getMenuTree").then(res => {
                window.console.log("axios ok");
                this.state.menuTree = res.data.menu;
                window.console.log(this.state.menuTree);
            });
        },
        IsLogin() {
            window.console.log('isLogin');
            axios.get(this.state.api + 'Admin/islogin').then(res => {
                window.console.log(res.data)
                this.state.isAuthenticated = res.data
                if (res.data) {
                    //router.push({ name: "Admin" });
                } else {
                    //router.push({ name: "Login" });
                }
                return res.data
            })
        },
        GetUserInfo() {
            return axios.get(this.state.api + 'Admin/getuserinfo')
                .then(res => {
                    this.state.claims = res.data.claims
                    console.log(res.data)
                })
        }

    },
    modules: {}
});