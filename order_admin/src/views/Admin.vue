<template>
  <!-- App.vue -->

  <v-app>
    <v-navigation-drawer
      app
      :clipped="$vuetify.breakpoint.smAndUp"
      v-model="drawer"
    >
      <v-list dense expand class="z-app-list">
        <v-row>
          <v-list-item class="pl-6">
            <v-list-item-avatar>
              <v-img src="../assets/angry.png"></v-img>
            </v-list-item-avatar>
            <v-list-item-content class="pl-1">
              <v-list-item-title>{{this.$store.state.claims.name}}</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </v-row>
        <v-divider></v-divider>
        <template>
          <!-- v-list-group 模板：套用有 children 屬性的資料 -->
          <v-list>
            <v-list-group
              v-for="item in this.$store.state.menuTree"
              :key="item.menuId"
              v-model="item.active"
              :prepend-icon="item.icon"
              no-action
            >
              <template v-slot:activator>
                <v-list-item-content>
                  <v-list-item-title v-text="item.menuName"></v-list-item-title>
                </v-list-item-content>
              </template>

              <v-list-item
                v-for="child in item.detail"
                :key="child.sysMenuId"
                link
                :to="child.url"
              >
                <v-list-item-content>
                  <v-list-item-title
                    v-text="child.menuName"
                  ></v-list-item-title>
                </v-list-item-content>
              </v-list-item>
            </v-list-group>
          </v-list>
          <!-- v-list-item 模板：套用未被上述條件捕獲資料 -->
        </template>
        <v-divider></v-divider>

        <v-list-item link :to="'/'">
          <v-list-item-action>
            <v-icon>mdi-home</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title>回首頁</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <v-list-item @click="logout()">
          <v-list-item-action>
            <v-icon>mdi-logout</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title>登出</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-app-bar
      :clipped-left="$vuetify.breakpoint.smAndUp"
      app
      color="light-blue darken-4"
      dark
    >
      <v-app-bar-nav-icon @click.stop="drawer = !drawer" />
      <v-toolbar-title>
        <span class="hidden-sm-and-down">點餐系統後臺</span>
      </v-toolbar-title>
      <v-spacer></v-spacer>
      <v-switch
        v-model="$vuetify.theme.dark"
        inset
        persistent-hint
        style="height: 40%"
      ></v-switch>
    </v-app-bar>

    <!-- Sizes your content based upon application components -->
    <v-main class="pt-2 pt-sm-2 pt-xs-2 pt-md-0 pt-lg-0 pt-xl-0">
      <v-container fluid>
        <!-- {{this.$store.state.menuTree}} -->
        <!-- If using vue-router -->
        <router-view></router-view>
      </v-container>
    </v-main>

    <!-- <v-footer app>
      
    </v-footer> -->
  </v-app>
</template>
<script>
export default {
  created() {    
    this.$store.dispatch("getMenuTree");
    //this.$store.dispatch("IsLogin");
    //this.$store.dispatch("GetUserInfo");
  },
  data: () => ({
    drawer: null,
  }),
  methods: {
    logout() {
      let url = this.$store.state.api + "Admin/Logout";
      this.axios.post(url).then((res) => {
        window.console.log(res);
        if (res.data == "ok") {
          this.$store.state.isAuthenticated = false;
          this.$router.push({ name: "Login" });
        } else {
          alert(res.data.errMsg);
        }
        this.dialogLoding = false;
      });

      //window.console.log(this.$store.dispatch('IsLogin'))
    },
  },
};
</script>
<style lang="scss" scoped>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

#nav {
  padding: 30px;

  a {
    font-weight: bold;
    color: #2c3e50;

    &.router-link-exact-active {
      color: #42b983;
    }
  }
}
.z-app-list .v-list-item {
  padding-top: 4px !important;
  padding-bottom: 4px !important;
  font-size: 24px;
}
.v-application--wrap {
  min-height: calc(100vh - 60px) !important;
}
</style>
