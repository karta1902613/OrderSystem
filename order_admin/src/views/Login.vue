<template>
  <v-app id="inspire">
    <v-content>
      <v-container fluid fill-height>
        <v-layout align-center justify-center>
          <v-flex xs12 sm8 md4>
            <v-card class="elevation-12">
              <v-toolbar dark color="primary">
                <v-toolbar-title>登入</v-toolbar-title>
                <v-spacer></v-spacer>
                <v-switch
                  v-model="$vuetify.theme.dark"
                  inset
                  persistent-hint
                  style="height: 40%"
                ></v-switch>
                <v-tooltip right> </v-tooltip>
              </v-toolbar>
              <v-card-text>
                <v-form>
                  <v-text-field
                    prepend-icon="person"
                    name="login"
                    label="帳號"
                    type="text"
                  ></v-text-field>
                  <v-text-field
                    id="password"
                    prepend-icon="lock"
                    name="password"
                    label="密碼"
                    type="password"
                  ></v-text-field>
                </v-form>
              </v-card-text>
              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn @click="Login" color="primary">登入</v-btn>
              </v-card-actions>
            </v-card>
          </v-flex>
        </v-layout>
      </v-container>
    </v-content>
  </v-app>
</template>

<script>
export default {
  created(){
this.$store.dispatch("IsLogin");
  },
  data: () => ({
    test:false,
    drawer: null,
  }),
  methods: {
    Login() {      
      let url = this.$store.state.api + "Admin/Login";
        let actRow = {
          userId: 'kevin',
          userPassword: '24369238',
        };
        this.axios.post(url, actRow).then((res) => {
          window.console.log(res)
          if (res.data == "ok") {
           this.$store.state.isAuthenticated = true;
           this.$router.push({ name: "Admin" });
          } else {
            alert(res.data.errMsg);
          }        
        });
     
    },
  },
  props: {
    source: String,
  },
};
</script>