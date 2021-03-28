<template>
  <v-data-table
    :headers="headers"
    :items="this.$store.state.shopData"
    sort-by="shopAddr"
    class="elevation-1"
  >
    <template v-slot:top>
      <v-toolbar flat>
        <v-toolbar-title>店家資料</v-toolbar-title>
        <v-divider class="mx-4" inset vertical></v-divider>
        <v-col cols="12" sm="6" md="2">
          <v-text-field
            label="店家名稱"
            outlined
            hide-details
            v-model="shopNameCon"
            dense
          ></v-text-field>
        </v-col>
        <v-btn color="primary" dark class="mb-2" @click="query">查詢</v-btn>
        <v-spacer></v-spacer>

        <v-dialog v-model="dialog" max-width="500px">
          <template v-slot:activator="{ on, attrs }">
            <v-btn color="primary" dark class="mb-2" v-bind="attrs" v-on="on">
              新增店家
            </v-btn>
          </template>
          <v-card>
            <v-card-title>
              <span class="headline">{{ formTitle }}</span>
            </v-card-title>

            <v-card-text>
              <v-container>
                <v-row>
                  <v-col cols="12" sm="6" md="4">
                    <v-text-field
                      v-model="editedItem.shopName"
                      label="店家名稱"
                    ></v-text-field>
                  </v-col>

                  <v-col cols="12" sm="6" md="4">
                    <v-text-field
                      v-model="editedItem.shopTel"
                      label="店家電話"
                    ></v-text-field>
                  </v-col>
                  <v-col cols="12" sm="6" md="4">
                    <v-select
                      :items="items"
                      v-model="selectShopType"
                      item-text="name"
                      item-value="value"
                      return-object
                      label="店家類型"
                    ></v-select>
                  </v-col>
                  <v-col cols="12" sm="12" md="12">
                    <v-text-field
                      v-model="editedItem.shopAddr"
                      label="店家地址"
                    ></v-text-field>
                  </v-col>
                  <v-col cols="12" sm="6" md="4">
                    <v-switch
                      v-model="editedItem.isDeliver"
                      :label="`是否外送`"
                    ></v-switch>
                  </v-col>
                  <v-col cols="12" sm="6" md="4">
                    <v-text-field
                      v-model="editedItem.minCost"
                      label="外送金額"
                    ></v-text-field>
                  </v-col>
                  <v-col cols="12" sm="4">
                    <v-menu
                      ref="menu"
                      v-model="menu2"
                      :close-on-content-click="false"
                      :nudge-right="40"
                      :return-value.sync="editedItem.limitTime"
                      transition="scale-transition"
                      offset-y
                      max-width="290px"
                      min-width="290px"
                    >
                      <template v-slot:activator="{ on, attrs }">
                        <v-text-field
                          v-model="editedItem.limitTime"
                          label="最晚訂餐時間"
                          prepend-icon="mdi-clock-time-four-outline"
                          readonly
                          v-bind="attrs"
                          v-on="on"
                        ></v-text-field>
                      </template>
                      <v-time-picker
                        v-if="menu2"
                        v-model="editedItem.limitTime"
                        full-width
                        @click:minute="$refs.menu.save(editedItem.limitTime)"
                      ></v-time-picker>
                    </v-menu>
                  </v-col>
                </v-row>
              </v-container>
            </v-card-text>

            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="blue darken-1" text @click="close"> 取消 </v-btn>
              <v-btn color="blue darken-1" text @click="save"> 儲存 </v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <v-dialog v-model="dialogDelete" max-width="500px">
          <v-card>
            <v-card-title class="headline">是否刪除?</v-card-title>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="blue darken-1" text @click="closeDelete"
                >取消</v-btn
              >
              <v-btn color="blue darken-1" text @click="deleteItemConfirm"
                >確認</v-btn
              >
              <v-spacer></v-spacer>
            </v-card-actions>
          </v-card>
        </v-dialog>
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
      </v-toolbar>
    </template>
    <template v-slot:item.actions="{ item }">
      <v-icon small class="mr-2" @click="editItem(item)"> mdi-pencil </v-icon>
      <v-icon small @click="deleteItem(item)"> mdi-delete </v-icon>
    </template>
    <template v-slot:no-data> </template>
  </v-data-table>
</template>
<script>
export default {
  data: () => ({
    dialog: false,
    dialogDelete: false,
    dialogLoding: false,
    shopNameCon: "",
    time: null,
    menu2: false,
    modal2: false,
    selectShopType: { name: "請選擇", value: null, placeholder: "" },
    items: [
      { name: "一般店家", value: "10" },
      { name: "飲料店", value: "20" },
    ],
    headers: [
      {
        text: "店家名稱",
        align: "start",
        value: "shopName",
      },
      { text: "店家地址", value: "shopAddr" },
      { text: "店家電話", value: "shopTel" },
      { text: "是否外送", value: "isDeliverName" },
      { text: "外送金額", value: "minCost" },
      { text: "最晚訂餐時間", value: "limitTime" },
      { text: "店家類型", value: "statusName" },
      { text: "動作", value: "actions", sortable: false },
    ],
    editedIndex: -1,
    editedItem: {},
    defaultItem: {},
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "新增店家資料" : "編輯店家資料";
    },
  },

  watch: {
    dialog(val) {
      val || this.close();
    },

    dialogDelete(val) {
      val || this.closeDelete();
    },
  },

  created() {
    this.initialize();
  },

  methods: {
    initialize() {},
    query() {
      this.dialogLoding = true;
      //清除store舊資料1
      this.$store.state.shopData.splice(0);
      let url = this.$store.state.api + "C00010/QueryShop";
      let actRow = {
        shopName: this.shopNameCon,
      };
      this.axios.post(url, actRow).then((res) => {
        if (res.data.resultCode == "10") {
          res.data.shopData.forEach((e) => {
            e.isDeliverName = e.isDeliver == "True" ? "是" : "否";
            let timeSplit = e.limitTime.split(":");
            if (e.limitTime != "") {
              e.limitTime = timeSplit[0] + ":" + timeSplit[1];
            }

            //window.console.log(e)
            this.$store.state.shopData.push(e);
            this.dialogLoding = false;
          });
        } else {
          this.dialogLoding = false;
          alert(res.data.errMsg);
        }
      });
    },
    editItem(item) {
      window.console.log(item);
      window.console.log(this.$store.state.shopData.indexOf(item));
      this.editedIndex = this.$store.state.shopData.indexOf(item);
      this.editedItem = Object.assign({}, item);
      (this.editedItem.isDeliver = Boolean(this.editedItem.isDeliver)),
        (this.selectShopType = {
          name: this.editedItem.statusName,
          value: this.editedItem.shopStatus,
        });

      this.dialog = true;
    },

    deleteItem(item) {
      this.editedIndex = this.$store.state.shopData.indexOf(item);
      this.editedItem = Object.assign({}, item);
      this.dialogDelete = true;
    },

    deleteItemConfirm() {
      let url = this.$store.state.api + "C00010/DeleteShop";
      let actRow = {
        shopId: this.editedItem.shopId,
      };
      this.axios.post(url, actRow).then((res) => {
        if (res.data.resultCode == "10") {
          this.$store.state.shopData.splice(this.editedIndex, 1);
          this.closeDelete();
        } else {
          alert(res.data.errMsg);
        }
        this.dialogLoding = false;
      });
    },

    close() {
      this.dialog = false;
      this.$nextTick(() => {
        this.editedItem = Object.assign({}, this.defaultItem);
        this.editedIndex = -1;
      });
    },

    closeDelete() {
      this.dialogDelete = false;
      this.$nextTick(() => {
        this.editedItem = Object.assign({}, this.defaultItem);
        this.editedIndex = -1;
      });
    },

    save() {
      this.dialogLoding = true;
      this.editedItem.statusName = this.selectShopType.name;
      this.editedItem.shopStatus = this.selectShopType.value;
      this.editedItem.isDeliverName =
        this.editedItem.isDeliver == true ? "是" : "否";
      if (this.editedIndex > -1) {
        console.log(this.editedItem);
        let url = this.$store.state.api + "C00010/UpdateShop";
        let actRow = {
          shopId: this.editedItem.shopId,
          shopName: this.editedItem.shopName,
          shopTel: this.editedItem.shopTel,
          shopAddr: this.editedItem.shopAddr,
          shopStatus: this.editedItem.shopStatus,
          isDeliver: this.editedItem.isDeliver,
          limitTime: this.editedItem.limitTime,
          minCost: this.editedItem.minCost,
        };
        this.axios.post(url, actRow).then((res) => {
          if (res.data.resultCode == "10") {
            Object.assign(
              this.$store.state.shopData[this.editedIndex],
              this.editedItem
            );
            this.close();
          } else {
            alert(res.data.errMsg);
          }
          this.dialogLoding = false;
        });
      } else {
        window.console.log(this.editedItem);
        let url = this.$store.state.api + "C00010/AddShop";
        let actRow = {
          shopName: this.editedItem.shopName,
          shopTel: this.editedItem.shopTel,
          shopAddr: this.editedItem.shopAddr,
          shopStatus: this.editedItem.shopStatus,
          isDeliver: this.editedItem.isDeliver,
          limitTime: this.editedItem.limitTime,
          minCost: this.editedItem.minCost,
          statusId: "10",
        };
        window.console.log(actRow);
        this.axios.post(url, actRow).then((res) => {
          if (res.data.resultCode == "10") {
            Object.assign(this.$store.state.shopData.push(this.editedItem));
            this.close();
          } else {
            alert(res.data.errMsg);
          }
          this.dialogLoding = false;
        });
      }
    },
  },
};
</script>
