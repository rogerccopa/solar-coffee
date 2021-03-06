<template>
  <div>
    <h1 id="customersTitle">Manage Customers</h1>
    <hr />
    <div class="customer-actions">
      <solar-button @button:click="showNewCustomerModal">
        Add Customer
      </solar-button>
    </div>
    <table id="customers" class="table">
      <tr>
        <th>Customer</th>
        <th>Address</th>
        <th>State</th>
        <th>Since</th>
        <th>Delete</th>
      </tr>
      <tr v-for="customer in customers" :key="customer.id">
        <td>{{ customer.firstName }} {{ customer.lastName }}</td>
        <td>
          {{ customer.primaryAddress.addressLine1 }}
          {{ customer.primaryAddress.addressLine2 }}
        </td>
        <td>
          {{ customer.primaryAddress.state }}
        </td>
        <td>
          {{ customer.createdOn | humanizeDate }}
        </td>
        <td>
          <span
            class="lni lni-cross-circle product-archive"
            @click="deleteCustomer(customer.id)"
          ></span>
        </td>
      </tr>
    </table>
    <new-customer-modal
      v-if="isNewCustomerVisible"
      @save:customer="saveNewCustomer"
      @close="closeModals"
    />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import SolarButton from "@/components/SolarButton.vue";
import NewCustomerModal from "@/components/modals/NewCustomerModal.vue";
import { ICustomer } from "@/types/Customer";
import { CustomerService } from "@/services/customer-service";

@Component({ name: "Customers", components: { SolarButton, NewCustomerModal } })
export default class Customers extends Vue {
  isNewCustomerVisible = false;
  customers: ICustomer[] = [];

  customerService: CustomerService = new CustomerService();

  showNewCustomerModal() {
    this.isNewCustomerVisible = true;
  }

  async saveNewCustomer(newCustomer: ICustomer) {
    await this.customerService.addCustomer(newCustomer);
    this.isNewCustomerVisible = false;
    await this.initialize();
  }

  async deleteCustomer(customerId: number) {
    await this.customerService.deleteCustomer(customerId);
    await this.initialize();
  }

  closeModals() {
    this.isNewCustomerVisible = false;
  }

  async initialize() {
    this.customers = await this.customerService.getCustomers();
  }

  async created() {
    await this.initialize();
  }
}
</script>

<style scoped lang="scss">
@import "@/scss/global.scss";

.product-archive {
  cursor: pointer;
  font-weight: bold;
  font-size: 1.2rem;
  color: $solar-red;
}
</style>