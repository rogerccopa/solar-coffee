import moment from "moment";
import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";

Vue.config.productionTip = false;

Vue.filter('pricefilter', function (n: number) {
  if (isNaN(n)) {
    return '-';
  }

  return '$ ' + n.toFixed(2);
});

Vue.filter('humanizeDate', function (dt: Date) {
  return moment(dt).format("MMMM Do YYYY");
});

new Vue({
  router,
  store,
  render: (h) => h(App),
}).$mount("#app");
