import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import qs from 'qs'
import vuetify from "./plugins/vuetify";
import axios from "axios";
import VueAxios from "vue-axios";

axios.defaults.withCredentials = true;
Vue.use(VueAxios, axios);
Vue.prototype.qs = qs
Vue.config.productionTip = false

Vue.config.productionTip = false;

new Vue({
    router,
    store,
    vuetify,
    render: h => h(App)
}).$mount("#app");