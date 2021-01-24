import axios from "axios";
import Vue from "vue";
import Vuex from "vuex";

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    menuTree: [],
    api: "https://localhost:44362/api/"
  },
  mutations: {},
  actions: {
    async getMenuTree() {
      window.console.log("start to get menuTree");
      await axios.get(this.state.api + "Admin/getMenuTree").then(res => {
        window.console.log("axios ok");
        this.state.menuTree = res.data.menu;
        console.log(this.state.menuTree);
      });
    }
  },
  modules: {}
});
