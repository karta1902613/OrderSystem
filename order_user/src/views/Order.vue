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
          <v-expansion-panel v-for="(item, rowId) in order" :key="rowId">
            <v-expansion-panel-header>
              <template v-slot:actions>
                <v-btn
                  color="primary"
                  @click.native.stop="btnCheckRemove(item)"
                  fab
                  small
                  dark
                >
                  <v-icon>mdi-delete</v-icon>
                </v-btn>
              </template>
              {{ item.mealName }}*{{ item.mealQuantity }} NT${{
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
          <v-col
            v-for="item in this.$store.state.menu"
            :key="item.mealsId"
            sm="24"
            md="4"
            lg="2"
          >
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
    <v-dialog v-model="dialogDelete" max-width="500px">
      <v-card>
        <v-card-title class="headline">是否刪除?</v-card-title>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click="closeDelete">取消</v-btn>
          <v-btn color="blue darken-1" text @click="btnRemoveOrder">確認</v-btn>
          <v-spacer></v-spacer>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-alert v-show="alertFlag" outlined text type="success"
      >成功加入訂單</v-alert
    >
    <v-dialog v-model="dialogLoding" hide-overlay persistent width="300">
      <v-card color="primary" dark>
        <v-card-text>
          Loading...
          <v-progress-linear
            indeterminate
            color="white"
            class="mb-0"
          ></v-progress-linear>
        </v-card-text>
      </v-card>
    </v-dialog>
  </v-app>
</template>

<script>
export default {
  created() {
    this.dialogLoding = true;
    let url = this.$store.state.api + "Order/GetOrderData";
    this.axios.get(url).then((res) => {
      this.dialogLoding = false;
      window.console.log(res.data);
      this.orderId = res.data.orderId;
      this.$store.state.menu.splice(0);
      this.order.splice(0);
      res.data.menu.forEach((e) => {
        window.console.log(e)
        this.$store.state.menu.push(e);
      });
      this.$store.state.menu.push({
        mealId: -1,
        mealName:  'PASS',
        mealPrice: 0,
        orderId: res.data.orderId,
        shopId:-1,
        statusId: '10',
        statusId1: '20',
        statusName:'其他'       
      })
      res.data.orderDetail.forEach((e) => {
        this.order.push(e);
        this.sumMoney += e.orderPrice;
      });
    });
  },
  data() {
    return {
      alertFlag: false,
      orderId: null,
      count: 1,
      sumMoney: 0,
      money: 0,
      drawer: null,
      dialog: false,
      dialogDelete: false,
      dialogLoding: false,
      tempItem: {},
      rmItem: {},
      menu: [],
      order: [],
    };
  },
  watch: {
    dialogDelete(val) {
      val || this.closeDelete();
    },
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
      let url = this.$store.state.api + "Order/ActOrderDetail";
      let actRow = {
        orderId: this.orderId,
        shopId: this.tempItem.shopId,
        mealId: this.tempItem.mealId,
        mealPrice: this.tempItem.mealPrice,
        mealQuantity: this.tempItem.mealQuantity,
        memo: this.tempItem.Memo,
        statusId1: this.tempItem.statusId1,
      };
      window.console.log(actRow);
      this.dialogLoding = true;
      this.axios.post(url, actRow).then((res) => {
        this.dialogLoding = false;
        if (res.data.resultCode == "10") {
          this.tempItem.mealsPriceSum =
            this.tempItem.mealPrice * this.tempItem.mealQuantity;
          this.sumMoney += this.tempItem.mealsPriceSum;
          this.dialog = false;
          this.tempItem.orderId = this.orderId;
          this.order.push({
            rowId: res.data.rowId,
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
          var timeoutID = window.setTimeout(
            () => (this.alertFlag = false),
            1337
          );
          this.closeDelete();
        } else {
          alert(res.data.errMsg);
        }
      });
      window.console.log(actRow);
    },
    btnCheckRemove(item) {
      this.dialogDelete = true;
      this.rmItem = item;
      console.log(this.rmItem);
    },
    btnRemoveOrder() {
      window.console.log(this.rmItem);
      let url = this.$store.state.api + "Order/DeleteOrderDetail";
      let rowId = this.rmItem.rowId;
      let actRow = {
        rowId: this.rmItem.rowId,
      };

      this.axios.post(url, actRow).then((res) => {
        window.console.log(res);
        if (res.data.resultCode === "10") {
          this.order = this.order.filter(function (obj) {
            return obj.rowId !== rowId;
          });

          this.sumMoney -= this.rmItem.mealPrice * this.rmItem.mealQuantity;
        } else {
          alert(res.data.errMsg);
        }
        this.dialogDelete = false;
        this.dialogLoding = false;
      });
    },
    closeDelete() {
      this.dialogDelete = false;
    },
  },
};
</script>