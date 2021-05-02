
<template>
  <v-stepper v-model="orderStepPage">
    <v-stepper-header>
      <v-stepper-step :complete="orderStepPage > 1" step="1">
        選擇訂單
      </v-stepper-step>

      <v-divider></v-divider>

      <v-stepper-step :complete="orderStepPage > 2" step="2">
        訂單建立
      </v-stepper-step>

      <v-divider></v-divider>
      <v-stepper-step :complete="orderStepPage > 3" step="3">
        訂單明細
      </v-stepper-step>

      <v-divider></v-divider>

      <v-stepper-step step="4"> 訂單結單 </v-stepper-step>
    </v-stepper-header>

    <v-stepper-items>
      <v-stepper-content step="1">
        <!-- <v-card class="mb-12" color="grey lighten-1" height="200px">  -->
        <v-container id="dropdown-example-1">
          <v-overflow-btn
            class="my-2"
            :items="this.$store.state.orderList"
            v-model="$store.state.order.orderId"
            label="選擇訂單"
            filled
            target="#dropdown-example-1"
          ></v-overflow-btn>
        </v-container>
        <!-- </v-card> -->

        <v-btn color="primary" @click="chooseOrder()"> 下一步 </v-btn>
      </v-stepper-content>

      <v-stepper-content step="2">
        <div class="box" v-if="orderStepPage == 2">
          <carousel-3d
            ref="mycarousel"
            :on-slide-change="onSlideChanged"
            :controls-visible="true"
            :controls-prev-html="'&#10092;'"
            :controls-next-html="'&#10093;'"
            :controls-width="30"
            :controls-height="60"
          >
            <slide v-for="(slide, i) in slides" :index="i" :key="i">
              <img :src="slide.src" />
            </slide>
          </carousel-3d>

          <v-chip class="ma-2" color="success" outlined x-large>
            <v-icon left> mdi-food </v-icon>
            {{ slides[currentImgIndex].title }} </v-chip
          ><br />
          <v-btn class="ma-2" outlined color="indigo" @click="pickOut()">
            隨機抽選
          </v-btn>

          <v-card elevation="2">
            <v-container>
              <v-row>
                <v-col cols="12" sm="6" md="4">
                  <v-text-field
                    v-model="$store.state.order.orderName"
                    label="訂單名稱"
                  ></v-text-field>
                </v-col>

                <v-col cols="12" sm="6" md="4">
                  <v-switch
                    :label="`是否系統結單`"
                    v-model="$store.state.order.isLimit"
                  ></v-switch>
                </v-col>
                <v-col cols="12" sm="6" md="4">
                  <v-menu
                    ref="menu"
                    v-model="menu2"
                    :close-on-content-click="false"
                    :nudge-right="40"
                    :return-value.sync="$store.state.order.limitTime"
                    transition="scale-transition"
                    offset-y
                    max-width="290px"
                    min-width="290px"
                  >
                    <template v-slot:activator="{ on, attrs }">
                      <v-text-field
                        v-model="$store.state.order.limitTime"
                        label="結單時間"
                        prepend-icon="mdi-clock-time-four-outline"
                        readonly
                        v-bind="attrs"
                        v-on="on"
                      ></v-text-field>
                    </template>
                    <v-time-picker
                      v-if="menu2"
                      v-model="$store.state.order.limitTime"
                      full-width
                      @click:minute="
                        $refs.menu.save($store.state.order.limitTime)
                      "
                    ></v-time-picker>
                  </v-menu>
                </v-col>
                <v-col cols="12" sm="12" md="12">
                  <v-text-field
                    v-model="$store.state.order.memo"
                    label="備註"
                  ></v-text-field>
                </v-col>
              </v-row>
            </v-container>
          </v-card>
        </div>
        <br />
        <v-row>
          <v-col cols="12" sm="6" md="3"></v-col>
          <v-col cols="12" sm="6" md="3">
            <v-btn color="primary" @click="$store.state.orderStepPage = 1">
              上一動
            </v-btn>
          </v-col>
          <v-col cols="12" sm="6" md="3">
            <v-btn color="primary" @click="addOrderDetail()"> 下一步 </v-btn>
          </v-col>
          <v-col cols="12" sm="6" md="3"></v-col>
        </v-row>
      </v-stepper-content>

      <v-stepper-content step="3">
        <v-row>
          <v-col cols="12" sm="12" md="9">
            <v-data-table
              :headers="headers"
              :items="orderDetail"
              :items-per-page="-1"
              hide-default-footer
              class="elevation-1"
            ></v-data-table
          ></v-col>
          <v-col cols="12" sm="12" md="3">
            <v-card class="mx-auto" max-width="500">
              <v-toolbar color="deep-purple accent-4" dark>
                <v-btn @click="refreshData()" icon color="green">
                  <v-icon>mdi-cached</v-icon>
                </v-btn>
                <v-toolbar-title>{{ orderDetailInfoType }}</v-toolbar-title>
                <v-spacer></v-spacer>
                <v-spacer></v-spacer>

                <v-switch
                  v-model="infoType"
                  inset
                  persistent-hint
                  style="height: 40%"
                ></v-switch>
              </v-toolbar>
              <v-list subheader v-if="infoType">
                <v-subheader>尚未訂餐</v-subheader>

                <v-list-item
                  v-for="chat in notYetOrderedUser"
                  :key="chat.title"
                >
                  <v-list-item-avatar>
                    <v-avatar
                      :color="chat.mycolor"
                      size="56"
                      v-text="chat.avatar"
                    ></v-avatar>
                  </v-list-item-avatar>

                  <v-list-item-content>
                    <v-list-item-title v-text="chat.title"></v-list-item-title>
                  </v-list-item-content>
                  <v-list-item-content> </v-list-item-content>
                </v-list-item>
              </v-list>
              <v-list subheader v-if="!infoType">
                <!-- <v-subheader>尚未訂餐</v-subheader> -->

                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title>店家名稱</v-list-item-title>
                  </v-list-item-content>
                  <v-list-item-content>{{
                    shopInfo.shopName
                  }}</v-list-item-content>
                </v-list-item>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title>店家地址</v-list-item-title>
                  </v-list-item-content>
                  <v-list-item-content>{{
                    shopInfo.shopAddr
                  }}</v-list-item-content>
                </v-list-item>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title>店家電話</v-list-item-title>
                  </v-list-item-content>
                  <v-list-item-content>{{
                    shopInfo.shopTel
                  }}</v-list-item-content>
                </v-list-item>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title>訂餐時間</v-list-item-title>
                  </v-list-item-content>
                  <v-list-item-content>{{
                    shopInfo.limitTime
                  }}</v-list-item-content>
                </v-list-item>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title>是否外送</v-list-item-title>
                  </v-list-item-content>
                  <v-list-item-content>{{
                    shopInfo.isDeliver
                  }}</v-list-item-content>
                </v-list-item>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title>最低消費</v-list-item-title>
                  </v-list-item-content>
                  <v-list-item-content>{{
                    shopInfo.minCost
                  }}</v-list-item-content>
                </v-list-item>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title>訂單總額</v-list-item-title>
                  </v-list-item-content>
                  <v-list-item-content>{{ orderSum }}</v-list-item-content>
                </v-list-item>
              </v-list>

              <v-divider></v-divider>

              <v-list subheader v-if="infoType">
                <v-subheader>Pass</v-subheader>

                <v-list-item v-for="chat in passUser" :key="chat.title">
                  <v-list-item-avatar>
                    <v-avatar
                      :color="chat.mycolor"
                      size="56"
                      v-text="chat.avatar"
                    ></v-avatar>
                  </v-list-item-avatar>

                  <v-list-item-content>
                    <v-list-item-title v-text="chat.title"></v-list-item-title>
                  </v-list-item-content>
                  <v-list-item-content>
                    <v-list-item-title v-text="chat.reason"></v-list-item-title>
                  </v-list-item-content>
                </v-list-item>
              </v-list> </v-card
          ></v-col>
        </v-row>

        <br />
        <v-row>
          <v-col cols="12" sm="6" md="3"></v-col>
          <v-col cols="12" sm="6" md="3">
            <v-btn color="primary" @click="$store.state.orderStepPage = 2">
              上一動
            </v-btn>
          </v-col>
          <v-col cols="12" sm="6" md="3">
            <v-btn color="primary" @click="orderEnd()"> 下一步 </v-btn>
          </v-col>
          <v-col cols="12" sm="6" md="3"></v-col>
        </v-row>
      </v-stepper-content>

      <v-stepper-content step="4">
        <v-card class="mb-12" color="grey lighten-1" height="200px"></v-card>

        <v-btn color="primary" @click="backHome()"> 回首頁 </v-btn>
      </v-stepper-content>
    </v-stepper-items>
    <v-row justify="center">
      <v-dialog v-model="dialog" persistent max-width="290">
        <v-card>
          <v-card-title class="headline"> 是否確認結單? </v-card-title>
          <v-card-text>確認結單後將進行扣款動作</v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="green darken-1" text @click="dialog = false">
              取消
            </v-btn>
            <v-btn color="green darken-1" text @click="orderCheckEnd()">
              確認
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-row>
    {{ this.$store.state.orderStepPage }}

    {{ this.$store.state.order }}
    {{ this.notYetOrderedUser }}
  </v-stepper>
</template>
<script>
import { Carousel3d, Slide } from "vue-carousel-3d";
export default {
  data() {
    return {
      dialog: false,
      menu2: false,
      infoType: true,
      order: {},
      slides: [],
      shopInfo: {},
      orderSum: 0,
      currentImgIndex: 0,
      headers: [
        {
          text: "訂購人",
          align: "start",
          value: "userName",
        },
        { text: "餐點", value: "mealName" },
        { text: "數量", value: "mealQuantity" },
        { text: "訂單價格", value: "orderPrice" },
        { text: "價格", value: "mealPrice" },
        { text: "備註", value: "memo" },
      ],
      orderDetail: [],
      notYetOrderedUser: [],
      passUser: [],
    };
  },
  created() {
    this.$store.state.order.orderId = "-1";
    let url = this.$store.state.api + "O00010/GetOrderList";
    this.axios.get(url).then((res) => {
      window.console.log(res);

      if (res.data.resultCode == "10") {
        this.$store.state.orderList.splice(0);
        this.$store.state.orderList.push({ text: "新建訂單", value: "-1" });
        console.log(res.data);
        res.data.order.forEach((e) => {
          this.$store.state.orderList.push(e);
        });
      } else {
        alert(res.data.errMsg);
      }
      this.dialogLoding = false;
    });
    url = this.$store.state.api + "O00010/GetShopList";
    this.axios.get(url).then((res) => {
      window.console.log(res);

      if (res.data.resultCode == "10") {
        this.slides = res.data.slides;
        console.log(res.data);
      } else {
        alert(res.data.errMsg);
      }
      this.dialogLoding = false;
    });

    window.console.log(this.$store.state.orderList);
    if (this.$store.state.orderStepPage === 2) {
      this.$store.state.orderStepPage = 1;
    }
  },
  methods: {
    onSlideChanged(index) {
      // console.log("onSlideChanged Callback Triggered", "Slide Index " + index);
      this.currentImgIndex = index;
      this.$store.state.order.shopId = this.slides[index].shopId;
    },
    pickOut() {
      window.console.log(this);
      // window.console.log(this.$refs.mycarousel)
      this.$refs.mycarousel.goSlide(
        Math.floor(Math.random() * this.slides.length)
      );
    },
    addOrderDetail() {
      //this.$store.state.orderStepPage = 3;
      let today = new Date();
      let tmpM = "0" + (today.getMonth() + 1);
      let tmpD = "0" + today.getDate();
      let formatted =
        today.getFullYear() + "-" + tmpM.substr(-2) + "-" + tmpD.substr(-2);
      let url = this.$store.state.api + "O00010/ActOrder";
      console.log(this.$store.state.order);
      let actRow = {
        orderId: this.$store.state.order.orderId,
        orderName: this.$store.state.order.orderName,
        shopId: this.$store.state.order.shopId,
        isLimit:
          this.$store.state.order.isLimit == undefined
            ? false
            : this.$store.state.order.isLimit,
        limitTime:
          this.$store.state.order.limitTime == undefined
            ? null
            : formatted + "T" + this.$store.state.order.limitTime + ":00",
        memo:
          this.$store.state.order.memo == undefined
            ? null
            : this.$store.state.order.memo,
      };
      window.console.log(actRow);
      this.axios.post(url, actRow).then((res) => {
        if (res.data.resultCode == "10") {
          console.log(res.data);
          if (res.data.orderId != undefined) {
            this.$store.state.order.orderId = res.data.orderId;
          }
          this.$store.state.orderStepPage = 3;
        } else {
          alert(res.data.errMsg);
        }
      });
    },
    chooseOrder() {
      this.$store.state.orderStepPage = 2;
      if (this.$store.state.order.orderId === "-1") {
        var today = new Date();
        this.$store.state.order.orderName =
          today.getFullYear() +
          "-" +
          (today.getMonth() + 1) +
          "-" +
          today.getDate() +
          "當日午餐";
      } else {
        if (
          this.$store.state.orderList[this.$store.state.order.orderId]
            .limitTime != null
        ) {
          let unixTimeZero = Date.parse(
            this.$store.state.orderList[this.$store.state.order.orderId]
              .limitTime
          );
          console.log(unixTimeZero);
          let tmpdate = new Date(unixTimeZero);
          let tmpH = "0" + tmpdate.getHours();
          let tmpM = "0" + tmpdate.getMinutes();
          let formatted = tmpH + ":" + tmpM.substr(-2);
          this.$store.state.orderList[
            this.$store.state.order.orderId
          ].limitTime = formatted;
        }

        this.$store.state.order = this.$store.state.orderList[
          this.$store.state.order.orderId
        ];
        window.console.log(this.order);
        //this.$store.state.order = this.order
      }
    },
    orderEnd() {
      this.dialog = true;
    },
    orderCheckEnd() {
      //TODO post act order end
      let actRow = {
        orderId: this.$store.state.order.orderId,
      };
      let url = this.$store.state.api + "O00010/OrderCheckEnd";
      this.axios.post(url, actRow).then((res) => {
        if (res.data.resultCode == "10") {
          this.dialog = false;
          this.$store.state.orderStepPage = 4;
        } else {
          alert(res.data.errMsg);
        }
      });
    },
    getOrderDetail() {
      let actRow = {
        orderId: this.$store.state.order.orderId,
        statusId1: "10",
      };
      let url = this.$store.state.api + "O00010/GetOrderDetail";
      this.axios.post(url, actRow).then((res) => {
        if (res.data.resultCode == "10") {
          let orderSum = 0;
          res.data.orderDetail.forEach((e) => {
            orderSum += e.orderPrice;
          });
          this.orderSum = orderSum;
          this.orderDetail = res.data.orderDetail;
        } else {
          alert(res.data.errMsg);
        }
      });
    },
    getNotYetOrderedUser() {
      let actRow = {
        orderId: this.$store.state.order.orderId,
      };
      let url = this.$store.state.api + "O00010/GetNotYetOrderedUser";
      this.axios.post(url, actRow).then((res) => {
        if (res.data.resultCode == "10") {
          this.notYetOrderedUser = [];
          window.console.log(res.data.NotYetOrderedUser);
          res.data.NotYetOrderedUser.forEach((e) => {
            e.avatar = e.title.substr(0, 1).toUpperCase();
            e.mycolor = "#" + ((Math.random() * 0xffffff) << 0).toString(16);
            this.notYetOrderedUser.push(e);
          });

          window.console.log(res.data.notYetOrderedUser);
          window.console.log(this.notYetOrderedUser);
        } else {
          alert(res.data.errMsg);
        }
      });
    },
    getPassUser() {
      let actRow = {
        orderId: this.$store.state.order.orderId,
        statusId1: "20",
      };
      let url = this.$store.state.api + "O00010/GetOrderDetail";
      this.axios.post(url, actRow).then((res) => {
        if (res.data.resultCode == "10") {
          this.passUser = [];
          window.console.log(res.data.orderDetail);
          res.data.orderDetail.forEach((e) => {
            e.title = e.userName;
            e.avatar = e.userName.substr(0, 1).toUpperCase();
            e.mycolor = "#" + ((Math.random() * 0xffffff) << 0).toString(16);
            this.passUser.push(e);
          });
        } else {
          alert(res.data.errMsg);
        }
      });
    },
    getShopDetail() {
      let actRow = {
        shopId: this.$store.state.order.shopId,
      };
      window.console.log(actRow);
      let url = this.$store.state.api + "O00010/GetShopDetail";
      this.axios.post(url, actRow).then((res) => {
        if (res.data.resultCode == "10") {
          window.console.log(res.data);
          res.data.ShopDetail[0].limitTime == null
            ? ""
            : res.data.ShopDetail[0];
          res.data.ShopDetail[0].isDeliver == true ? "是" : "否";
          this.shopInfo = res.data.ShopDetail[0];
        } else {
          alert(res.data.errMsg);
        }
      });
    },
    backHome() {
      this.$store.state.orderStepPage = 1;
      this.$router.push({ name: "Admin" });
    },
    refreshData() {
      this.getOrderDetail();
      this.getNotYetOrderedUser();
      this.getPassUser();
      this.getShopDetail();
    },
  },

  computed: {
    orderDetailInfoType: function () {
      if (this.infoType) {
        return "使用者清單";
      } else {
        return "店家資訊";
      }
    },
    orderStepPage: function () {
      switch (this.$store.state.orderStepPage) {
        case 1:
          break;
        case 2:
          this.$nextTick(() => {
            this.$refs.mycarousel.goSlide(this.currentImgIndex);
            console.log(this.$store.state.order.orderId);
            //this.$store.state.order.orderName = this.$store.state.orderList[this.$store.state.order.orderId].text
          });

          // this.slides.findIndex(isLargeNumber)
          // if(this.$store.state.order.orderId !== '-1' ){
          //   window.console.log('todo get shop orderdetail')
          // }
          break;
        case 3:
          this.getOrderDetail();
          this.getNotYetOrderedUser();
          this.getPassUser();
          this.getShopDetail();
          break;
        case 4:
          break;
      }
      return this.$store.state.orderStepPage;
    },
  },
  components: {
    Carousel3d,
    Slide,
  },
};
</script>

<style lang="scss">
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
.next span {
  color: #01579b;
}
.prev span {
  color: #01579b;
}
</style>