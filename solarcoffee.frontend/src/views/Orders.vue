<template>
  <div>
    <h1 id="ordersTitle">Sales Orders</h1>
    <hr />
    <table id="sales-orders" class="table" v-if="orders.length">
      <thead>
        <tr>
          <th>CustomerID</th>
          <th>OrderID</th>
          <th>Order Total</th>
          <th>Order Status</th>
          <th>Mark Complete</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="order in orders" :key="order.id">
          <td>{{ order.customer.id }}</td>
          <td>{{ order.id }}</td>
          <td>{{ getTotal(order) | pricefilter }}</td>
          <td :class="{ green: order.isPaid }">
            {{ getStatus(order.isPaid) }}
          </td>
          <td>
            <div
              v-if="!order.isPaid"
              class="lni lni-checkmark-circle order-complete green"
              @click="markComplete(order.id)"
            ></div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { OrderService } from "@/services/order-service";
import { ISalesOrder } from "@/types/SalesOrder";

const orderService = new OrderService();

@Component({ name: "Orders" })
export default class Orders extends Vue {
  orders: ISalesOrder[] = [];

  getTotal(order: ISalesOrder) {
    return order.salesOrderItems.reduce(
      (accum, item) => accum + item["product"]["price"] * item["quantity"],
      0
    );
  }

  getStatus(isPaid: boolean) {
    return isPaid ? "Paid in full" : "Unpaid";
  }

  async markComplete(orderId: number) {
    await orderService.markOrderComplete(orderId);
    await this.initialize();
  }

  async initialize() {
    this.orders = await orderService.getOrders();
    console.log(this.orders);
    console.log(this.orders.length);
  }
  async created() {
    this.initialize();
  }
}
</script>

<style scoped lang="scss">
@import "@/scss/global.scss";

.green {
  font-weight: bold;
  color: $solar-green;
}

.order-complete {
  width: 50%;
  cursor: pointer;
  text-align: center;
}
</style>
