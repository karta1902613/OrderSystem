import Vue from 'vue'
import Vuetify from 'vuetify/lib'
// src/plugins/vuetify.js
import '@mdi/font/css/materialdesignicons.css' // Ensure you are using css-loader
import 'material-design-icons-iconfont/dist/material-design-icons.css'

Vue.use(Vuetify)

export default new Vuetify({
    theme: {
        themes: {
            light: {
                // https://material.io/resources/color/#!/?view.left=0&view.right=1&primary.color=f0aa00
                // primary: colors.red.darken1 // #E53935
                primary: '#f0aa00'
                    // secondary: colors.red.lighten4, // #FFCDD2
                    // accent: colors.indigo.base // #3F51B5
            }
        }
    }
})