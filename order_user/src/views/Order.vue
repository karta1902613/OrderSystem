<template>
  <v-app id="inspire">
    <v-app-bar app color="blue-grey darken-2" dark>
      <v-app-bar-nav-icon @click="drawer = !drawer"></v-app-bar-nav-icon>

      <v-toolbar-title>訂單</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-switch
        v-model="$vuetify.theme.dark"
        inset
        persistent-hint
        style="height: 40%"
      ></v-switch>

      <v-btn outlined text @click="logout()"> 登出 </v-btn>
    </v-app-bar>

    <v-navigation-drawer v-model="drawer" fixed temporary>
      <v-row justify="center">
        <v-expansion-panels :disabled="true">
          <v-expansion-panel>
            <v-expansion-panel-header
              >總金額:{{ sumMoney }}</v-expansion-panel-header
            >
          </v-expansion-panel>
        </v-expansion-panels>
        <v-expansion-panels>
          <v-expansion-panel v-for="(item, orderId) in order" :key="orderId">
            <v-expansion-panel-header
              >{{ item.mealName }}*{{ item.mealQuantity }} NT${{
                item.mealPrice * item.mealQuantity
              }}</v-expansion-panel-header
            >
            <v-expansion-panel-content>
              <span>備註:{{ item.Memo }}</span>
            </v-expansion-panel-content>
          </v-expansion-panel>
        </v-expansion-panels>
      </v-row>
    </v-navigation-drawer>

    <v-main>
      <v-container>
        <v-row>
          <v-col v-for="item in this.$store.state.menu" :key="item.mealsId" sm="24" md="4" lg="2">
            <v-card class="mx-auto" max-width="344" outlined>
              <v-list-item three-line>
                <v-list-item-content>
                  <div class="overline mb-4">
                    {{ item.statusName }}
                  </div>
                  <v-list-item-title class="headline mb-1">
                    {{ item.mealName }}
                  </v-list-item-title>
                  <v-list-item-subtitle
                    >NT${{ item.mealPrice }}</v-list-item-subtitle
                  >
                </v-list-item-content>
              </v-list-item>

              <v-card-actions>
                <v-btn outlined rounded text @click="dialogEdit(item)">
                  加入訂單
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </v-main>
    <v-dialog v-model="dialog" persistent max-width="600px">
      <v-card>
        <v-card-title>
          <span class="headline">{{ this.tempItem.mealName }}</span>
        </v-card-title>
        <v-card-text>
          <v-container>
            <v-row>
              <v-col cols="12">
                <v-text-field
                  label="備註"
                  v-model="tempItem.Memo"
                ></v-text-field>
              </v-col>
              <v-col cols="12">
                <v-btn
                  class="mx-2"
                  fab
                  dark
                  small
                  color="indigo"
                  @click="btnMinus()"
                >
                  <v-icon dark>mdi-minus</v-icon>
                </v-btn>

                <v-avatar fab color="teal" size="40">
                  <span class="white--text headline">{{ count }}</span>
                </v-avatar>
                <v-btn
                  class="mx-2"
                  fab
                  dark
                  small
                  color="indigo"
                  @click="btnPlus()"
                >
                  <v-icon dark>mdi-plus</v-icon>
                </v-btn>

                {{ money }}
              </v-col>
            </v-row>
          </v-container>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click="dialog = false">
            取消
          </v-btn>
          <v-btn color="blue darken-1" text @click="btnAddOrder()">
            確認
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-alert v-show="alertFlag" outlined text type="success"
      >成功加入訂單</v-alert
    >
  </v-app>
</template>

<script>
export default {
  created(){
    let url = this.$store.state.api + 'Order/GetOrderData';
    this.axios.get(url).then((res)=>{
      window.console.log(res.data)
       res.data.forEach(e => {
          this.$store.state.menu.push(e);
       });
    })
  },
  data() {
    return {
      alertFlag: false,
      orderId: 0,
      count: 1,
      sumMoney: 0,
      money: 0,
      drawer: null,
      dialog: false,
      tempItem: {},
      menu: [
        {
          type: "主食",
          mealsId: 1,
          mealsName: "古早味乾麵",
          mealsPrice: "45",
        },
        {
          type: "主食",
          mealsId: 2,
          mealsName: "麻醬乾麵",
          mealsPrice: 45,
        },
        {
          type: "主食",
          mealsId: 3,
          mealsName: "紹辣乾麵",
          mealsPrice: 49,
        },
        {
          type: "湯品",
          mealsId: 4,
          mealsName: "酸辣湯",
          mealsPrice: 30,
        },
        {
          type: "湯品",
          mealsId: 5,
          mealsName: "玉米濃湯",
          mealsPrice: 30,
        },
        {
          type: "湯品",
          mealsId: 7,
          mealsName: "蕈菇湯",
          mealsPrice: 30,
        },
        {
          type: "湯品",
          mealsId: 8,
          mealsName: "旗魚花枝丸湯",
          mealsPrice: 30,
        },
      ],
      order: [],
    };
  },
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
    },
    dialogEdit(item) {
      this.dialog = true;
      this.count = 1;
      this.tempItem = item;
      this.tempItem.mealsPriceSum = this.tempItem.mealPrice;
      this.money = this.tempItem.mealPrice;
      this.tempItem.mealQuantity = 1;
      this.tempItem.Memo = "";
    },
    btnMinus() {
      if (this.count <= 1) {
        return;
      }
      this.count--;
      this.tempItem.mealQuantity--;
      this.money = this.count * this.tempItem.mealPrice;
    },
    btnPlus() {
      this.count++;
      this.tempItem.mealQuantity++;
      this.money = this.count * this.tempItem.mealPrice;
    },
    btnAddOrder() {
      this.tempItem.mealsPriceSum =
        this.tempItem.mealPrice * this.tempItem.mealQuantity;
      this.sumMoney += this.tempItem.mealsPriceSum;
      this.dialog = false;
      this.tempItem.orderId = this.orderId;
      this.order.push({
        orderId: this.orderId,
        menuId: this.tempItem.menuId,
        mealName: this.tempItem.mealName,
        mealQuantity: this.tempItem.mealQuantity,
        mealPrice: this.tempItem.mealPrice,
        mealsPriceSum: this.tempItem.mealPriceSum,
        Memo: this.tempItem.Memo,
      });
      this.orderId++;
      this.alertFlag = true;
      var timeoutID = window.setTimeout(() => (this.alertFlag = false), 1337);
    },
  },
  
};
</script>