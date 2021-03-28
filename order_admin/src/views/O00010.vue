
<template>
  <v-stepper v-model="e1">
    <v-stepper-header>
      <v-stepper-step :complete="e1 > 1" step="1"> 選擇訂單 </v-stepper-step>

      <v-divider></v-divider>

      <v-stepper-step :complete="e1 > 2" step="2"> 選擇店家 </v-stepper-step>

      <v-divider></v-divider>
      <v-stepper-step :complete="e1 > 3" step="3"> 訂單建立 </v-stepper-step>

      <v-divider></v-divider>

      <v-stepper-step step="4"> 訂單結單 </v-stepper-step>
    </v-stepper-header>

    <v-stepper-items>
      <v-stepper-content step="1">
        <v-card class="mb-12" color="grey lighten-1" height="200px"> </v-card>

        <v-btn color="primary" @click="e1 = 2"> Continue </v-btn>

        <v-btn text> Cancel </v-btn>
      </v-stepper-content>

      <v-stepper-content step="2">
        <div class="box" v-if="e1==2"> 
          

   <carousel-3d :controls-visible="true"  :on-slide-change="onSlideChanged" :on-last-slide="onLastSlide"
                   :on-main-slide-click="onMainSlideClick" :controls-prev-html="'&#10092;'" :controls-next-html="'&#10093;'"
                   :controls-width="30" :controls-height="60">
        <slide v-for="(slide, i) in slides" :index="i" :key="i">
          <img :src="slide.src">          
        </slide>
        
      </carousel-3d>
        
      {{slides[currentImgIndex].title}}
      
        </div>
        <v-btn color="primary" @click="e1 = 3"> Continue </v-btn>

        <v-btn text> Cancel </v-btn>
      </v-stepper-content>

      <v-stepper-content step="3">
        <v-card class="mb-12" color="grey lighten-1" height="200px"></v-card>

        <v-btn color="primary" @click="e1 = 4"> Continue </v-btn>

        <v-btn text> Cancel </v-btn>
      </v-stepper-content>

      <v-stepper-content step="4">
        <v-card class="mb-12" color="grey lighten-1" height="200px"></v-card>

        <v-btn color="primary" @click="e1 = 4"> Continue </v-btn>

        <v-btn text> Cancel </v-btn>
      </v-stepper-content>
    </v-stepper-items>
  </v-stepper>
</template>
<script>
import { Carousel3d, Slide } from "vue-carousel-3d";
export default {
  data() {
    return {
      e1: 1,
      slides: [
        {
          title: "八方雲集",          
          src: "https://images.1111.com.tw/news/news103618.jpg",
        },
        {
          title: "來碗拉麵",        
          src: "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSUMASJzSXrxLE_bF63Fxv4mtj29KpRfDyAsw&usqp=CAU",
        },
        {
          title: "小原草魚湯",          
          src: "https://placehold.it/360x270",
        },
        {
          title: "十勝嵐",          
          src: "https://placehold.it/360x270",
        },
        {
          title: "羊肉羹魷魚羹",          
          src: "https://placehold.it/360x270",
        },
        {
          title: "焢肉堯",          
          src: "https://placehold.it/360x270",
        },      
      ],
      currentImgIndex: 0
    };
  },
  methods: {
    onSlideChanged(index) {
      console.log("onSlideChanged Callback Triggered", "Slide Index " + index);
      this.currentImgIndex = index;
    },
    onLastSlide(index) {
      console.log("onLastSlide Callback Triggered", "Slide Index " + index);
    },
    onMainSlideClick() {
      console.log("onMainSlideClick Callback Triggered");
    },
    onAfterSlideChanged(index) {
      console.log(
        "@after-slide-changed Callback Triggered",
        "Slide Index " + index
      );
    },
    onSlideChange(index) {
      console.log(
        "@before-slide-change Callback Triggered",
        "Slide Index " + index
      );
    },
    addSlide() {
      this.slideCount++;
    },
    removeSlide() {
      if (this.slideCount > 1) {
        this.slideCount--;
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
</style>