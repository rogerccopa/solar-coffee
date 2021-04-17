<template>
  <div class="side-menu-container">
    <router-link to="/">
      <img
        id="imgLogo"
        alt="Solar Coffee logo"
        src="../assets/images/solar_coffee_logo.png"
      />
    </router-link>
    <h1>Management Portal</h1>
    <solar-button
      id="menuInventory"
      @button:click="gotoRoute('/inventory')"
      isFullWidth
    >
      Inventory
    </solar-button>
    <solar-button
      id="menuCustomers"
      @button:click="gotoRoute('/customers')"
      isFullWidth
    >
      Manage Customers
    </solar-button>
    <solar-button
      id="menuInvoice"
      @button:click="gotoRoute('/invoice/new')"
      isFullWidth
    >
      New Invoice
    </solar-button>
    <solar-button
      id="menuOrders"
      @button:click="gotoRoute('/orders')"
      isFullWidth
    >
      Orders
    </solar-button>
  </div>
</template>
    
<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import SolarButton from "@/components/SolarButton.vue";

@Component({
  name: "SideMenu",
  components: { SolarButton },
})
export default class SideMenu extends Vue {
  gotoRoute(route: string) {
    this.$router.push(route);
  }
}
</script>

<style scoped lang="scss">
// @ is alias for the /src directory
@import "@/scss/global.scss";

.side-menu-container {
  background-color: #fcfcfc;
  height: 100vh;
  width: $menu-width;
  display: flex;
  flex-direction: column;
  padding: 0.8rem;
  box-shadow: 0 1px 1px rgba(0, 0, 0, 0.12), 0 1px 1px rgba(0, 0, 0, 0.24);
  box-sizing: border-box;
}

#imgLogo {
  width: 100%;
}

h1 {
  font-size: 1.2rem;
  margin: 1rem 0;
}
</style>
