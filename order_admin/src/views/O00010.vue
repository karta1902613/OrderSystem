
<template>
  <v-stepper v-model="$store.state.orderStepPage">
    <v-stepper-header>
      <v-stepper-step :complete="$store.state.orderStepPage > 1" step="1">
        選擇訂單
      </v-stepper-step>

      <v-divider></v-divider>

      <v-stepper-step :complete="$store.state.orderStepPage > 2" step="2">
        訂單建立
      </v-stepper-step>

      <v-divider></v-divider>
      <v-stepper-step :complete="$store.state.orderStepPage > 3" step="3">
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
        <div class="box" v-if="$store.state.orderStepPage == 2">
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
              hide-default-footer
              class="elevation-1"
            ></v-data-table
          ></v-col>
          <v-col cols="12" sm="12" md="3">
            <v-card class="mx-auto" max-width="500">
              <v-toolbar color="deep-purple accent-4" dark>
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

                <v-list-item v-for="chat in recent" :key="chat.title">
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
                  <v-list-item-content>八方雲集</v-list-item-content>
                </v-list-item>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title>店家地址</v-list-item-title>
                  </v-list-item-content>
                  <v-list-item-content>台中市沙鹿區</v-list-item-content>
                </v-list-item>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title>店家電話</v-list-item-title>
                  </v-list-item-content>
                  <v-list-item-content>04-26360636</v-list-item-content>
                </v-list-item>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title>訂餐時間</v-list-item-title>
                  </v-list-item-content>
                  <v-list-item-content>11:00</v-list-item-content>
                </v-list-item>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title>是否外送</v-list-item-title>
                  </v-list-item-content>
                  <v-list-item-content>是</v-list-item-content>
                </v-list-item>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title>最低消費</v-list-item-title>
                  </v-list-item-content>
                  <v-list-item-content>400</v-list-item-content>
                </v-list-item>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title>訂單總額</v-list-item-title>
                  </v-list-item-content>
                  <v-list-item-content>410</v-list-item-content>
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

        <v-btn color="primary" @click="$store.state.orderStepPage = 4">
          Continue
        </v-btn>

        <v-btn text> Cancel </v-btn>
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
    {{ slides[currentImgIndex].shopId }}
    {{ this.$store.state.order }}
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

      slides: [
        {
          shopId: "1",
          title: "八方雲集",
          src: "https://images.1111.com.tw/news/news103618.jpg",
        },
        {
          shopId: "2",
          title: "來碗拉麵",
          src:
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSUMASJzSXrxLE_bF63Fxv4mtj29KpRfDyAsw&usqp=CAU",
        },
        {
          shopId: "3",
          title: "小原草魚湯",
          src:
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQwTqn1ODDo1F9mufocVIyn3DNn-omZzTMkohqz-0L2nPL9OvBv8mmjVMPPxoorC40SDgA&usqp=CAU",
        },
        {
          shopId: "6",
          title: "十勝嵐",
          src:
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQwTqn1ODDo1F9mufocVIyn3DNn-omZzTMkohqz-0L2nPL9OvBv8mmjVMPPxoorC40SDgA&usqp=CAU",
        },
        {
          shopId: "4",
          title: "羊肉羹魷魚羹",
          src:
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQwTqn1ODDo1F9mufocVIyn3DNn-omZzTMkohqz-0L2nPL9OvBv8mmjVMPPxoorC40SDgA&usqp=CAU",
        },
        {
          shopId: "5",
          title: "焢肉堯",
          src:
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQwTqn1ODDo1F9mufocVIyn3DNn-omZzTMkohqz-0L2nPL9OvBv8mmjVMPPxoorC40SDgA&usqp=CAU",
        },
      ],
      currentImgIndex: 0,
      headers: [
        {
          text: "訂購人",
          align: "start",
          value: "name",
        },
        { text: "餐點", value: "mealName" },
        { text: "數量", value: "mealQuantity" },
        { text: "訂單價格", value: "orderPrice" },
        { text: "價格", value: "mealPrice" },
        { text: "備註", value: "memo" },
      ],
      orderDetail: [
        {
          name: "kev!n",
          mealName: "韭菜鍋貼",
          mealPrice: 5.5,
          orderPrice: 55,
          mealQuantity: 10,
          memo: "",
        },
        {
          name: "kev!n",
          mealName: "貢丸湯",
          mealPrice: 25,
          mealQuantity: 1,
          memo: "",
        },
        {
          name: "peter",
          mealName: "韭菜水餃",
          mealPrice: 5.5,
          orderPrice: 66,
          mealQuantity: 12,
          memo: "",
        },
        {
          name: "tobin",
          mealName: "韭菜水餃",
          mealPrice: 5.5,
          orderPrice: 66,
          mealQuantity: 12,
          memo: "",
        },
        {
          name: "Patty",
          mealName: "韭菜水餃",
          mealPrice: 5.5,
          orderPrice: 66,
          mealQuantity: 12,
          memo: "",
        },
        {
          name: "wayne",
          mealName: "韭菜水餃",
          mealPrice: 5.5,
          orderPrice: 66,
          mealQuantity: 12,
          memo: "",
        },
        {
          name: "amy",
          mealName: "韭菜水餃",
          mealPrice: 5.5,
          orderPrice: 66,
          mealQuantity: 12,
          memo: "",
        },
      ],
      recent: [
        {
          avatar: "Iren",
          title: "Iren",
          mycolor: "#" + ((Math.random() * 0xffffff) << 0).toString(16),
        },

        {
          avatar: "Sylvia",
          title: "Sylvia",
          reason: "",
          mycolor: "#" + ((Math.random() * 0xffffff) << 0).toString(16),
        },
      ],
      passUser: [
        {
          title: "Fay",
          avatar: "Fay",
          reason: "預設",
          mycolor: "#" + ((Math.random() * 0xffffff) << 0).toString(16),
        },
        {
          avatar: "B",
          title: "Blackie",
          reason: "預設",
          mycolor: "#" + ((Math.random() * 0xffffff) << 0).toString(16),
        },
        {
          avatar: "Cindy",
          title: "Cindy",
          reason: "手動",
          mycolor: "#" + ((Math.random() * 0xffffff) << 0).toString(16),
        },
      ],
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
      this.$store.state.orderStepPage = 3;
      /*
        let url = this.$store.state.api + "O00010/ActOrder";
      
      this.axios.post(url, this.$store.state.order).then((res) => {
        if (res.data.resultCode == "10") {    
          this.$store.state.orderStepPage = 3;     
          console.log(123)
        } else {          
          alert(res.data.errMsg);
        }
      });
      */
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
      }
    },
    orderEnd() {
      this.dialog = true;
    },
    orderCheckEnd() {
      //TODO post act order end
      this.dialog = false;
      this.$store.state.orderStepPage = 4;
    },
  },
  watch: {
    "$store.state.orderStepPage": function () {
      switch (this.$store.state.orderStepPage) {
        case 1:
          break;
        case 2:
          this.$nextTick(() => {
            this.$refs.mycarousel.goSlide(this.currentImgIndex);
          });
          // this.slides.findIndex(isLargeNumber)          
          // if(this.$store.state.order.orderId !== '-1' ){
          //   window.console.log('todo get shop orderdetail')
          // }
          break;
        case 3:
          break;
        case 4:
          break;
      }
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