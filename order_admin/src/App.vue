<template>
  <router-view />
</template>

<script>
export default {
  async created() {
    // 動態載入主要顏色
    
    // 確認登入狀態
    // let url = this.$store.state.api + '/Admin/CheckLogin'
    // let res = await this.axios.post(url)
    // if (res.data.resultCode == '10') {
    //   this.$store.state.user.isLogin = true
    //   this.$router.push({ name: 'Admin' })
    // }
  }
}
</script>

<style lang="scss"></style>
