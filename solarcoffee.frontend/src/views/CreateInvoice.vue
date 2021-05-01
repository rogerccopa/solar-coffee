<template>
  <div>
    <h1 id="invoiceTitle">Create Invoice</h1>
    <hr />
    <div class="invoice-step" v-if="invoiceStep === 1">
      <h2>Step 1: Select Customer</h2>
      <!-- show ONLY if customers.length > 0 -->
      <div v-if="customers.length" class="invoice-step-detail">
        <label for="customer">Customer:</label>
        <select
          v-model="selectedCustomerId"
          class="invoice-customer"
          id="customer"
        >
          <option disabled value="">Please select a Customer</option>
          <option
            v-for="customer in customers"
            :value="customer.id"
            :key="customer.id"
          >
            {{ customer.firstName + " " + customer.lastName }}
          </option>
        </select>
      </div>
    </div>
    <div class="invoice-step" v-if="invoiceStep === 2">
      <h2>Step 2: Create Order</h2>
      <div v-if="inventory.length" class="invoice-step-detail">
        <label for="product">Product:</label>
        <select v-model="newItem.product" class="invoiceLineItem" id="product">
          <option disabled value="">Please select a Product</option>
          <option v-for="i in inventory" :value="i.product" :key="i.product.id">
            {{ i.product.name }}
          </option>
        </select>
        <label for="quantity">Quantity</label>
        <input v-model="newItem.quantity" id="quantity" type="number" min="0" />
      </div>
      <div class="invoice-item-actions">
        <solar-button
          :disable="!newItem.product || !newItem.quantity"
          @button:click="addLineItem"
        >
          Add Line Item
        </solar-button>
        <solar-button
          :disabled="!lineItems.length"
          @button:click="finalizeOrder"
        >
          Finalize Order
        </solar-button>
      </div>

      <!-- show ONLY if lineItems has items -->
      <div class="invoice-order-list" v-if="lineItems.length">
        <div class="runningTotal">
          <h3>Running Total:</h3>
          {{ runningTotal | pricefilter }}
        </div>
      </div>
      <hr />
      <table class="table">
        <thead>
          <tr>
            <th>Product</th>
            <th>Description</th>
            <th>Qty.</th>
            <th>Price</th>
            <th>Subtotal</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="lineItem in lineItems"
            :key="`index_${lineItem.product.id}`"
          >
            <td>{{ lineItem.product.name }}</td>
            <td>{{ lineItem.product.description }}</td>
            <td>{{ lineItem.quantity }}</td>
            <td>{{ lineItem.product.price }}</td>
            <td>
              {{ (lineItem.product.price * lineItem.quantity) | pricefilter }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <div class="invoice-step" v-if="invoiceStep === 3">
      <h2>Step 3: Review and Submit</h2>
      <solar-button @button:click="submitInvoice">Submit Invoice</solar-button>
      <hr />
      <div class="invoice-step-detail" id="invoice" ref="invoice">
        <div class="invoice-logo">
          <img
            id="imgLogo"
            alt="Solar Coffee logo"
            src="../assets/images/solar_coffee_logo.png"
          />
          <h3>1337 Solar Lane</h3>
          <h3>San Somewhere, CA 90000</h3>
          <h3>USA</h3>
          <!-- show div ONLY if lineItems has items -->
          <div class="invoice-order-list" v-if="lineItems.length">
            <div class="invoice-header">
              <h3>Invoice: {{ new Date() | humanizeDate }}</h3>
              <h3>
                Customer:
                {{
                  this.selectedCustomer.firstName +
                  " " +
                  this.selectedCustomer.lastName
                }}
              </h3>
              <h3>
                Address: {{ this.selectedCustomer.primaryAddress.addressLine1 }}
              </h3>
              <h3 v-if="this.selectedCustomer.primaryAddress.addressLine2">
                {{ this.selectedCustomer.primaryAddress.addressLine2 }}
              </h3>
              <h3>
                {{ this.selectedCustomer.primaryAddress.city }}
                {{ this.selectedCustomer.primaryAddress.state }}
                {{ this.selectedCustomer.primaryAddress.postalCode }}
              </h3>
              <h3>
                {{ this.selectedCustomer.primaryAddress.country }}
              </h3>
            </div>
            <table class="table">
              <thead>
                <tr>
                  <th>Product</th>
                  <th>Description</th>
                  <th>Qty.</th>
                  <th>Price</th>
                  <th>Subtotal</th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="lineItem in lineItems"
                  :key="`index_${lineItem.product.id}`"
                >
                  <td>{{ lineItem.product.name }}</td>
                  <td>{{ lineItem.product.description }}</td>
                  <td>{{ lineItem.quantity }}</td>
                  <td>{{ lineItem.product.price }}</td>
                  <td>
                    {{
                      (lineItem.product.price * lineItem.quantity) | pricefilter
                    }}
                  </td>
                </tr>
                <tr>
                  <th colspan="4"></th>
                  <th>Grand Total</th>
                </tr>
              </tbody>
              <tfoot>
                <tr>
                  <td colspan="4" class="due">Balance due upon receipt:</td>
                  <td class="price-final">{{ runningTotal | pricefilter }}</td>
                </tr>
              </tfoot>
            </table>
          </div>
        </div>
      </div>
    </div>
    <div class="invoice-steps-actions">
      <solar-button @button:click="prev" :disabled="!canGoPrev"
        >Previous</solar-button
      >
      <solar-button @button:click="next" :disabled="!canGoNext"
        >Next</solar-button
      >
      <solar-button @button:click="startOver">Start Over</solar-button>
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { IInvoice, ILineItem } from "@/types/IInvoice";
import { ICustomer } from "@/types/Customer";
import { IProductInventory } from "@/types/Product";
import { CustomerService } from "@/services/customer-service";
import { InventoryService } from "@/services/inventory-service";
import { InvoiceService } from "@/services/invoice-service";
import SolarButton from "@/components/SolarButton.vue";

// installed 3rd party libraries
import jsPDF from "jspdf";
import html2canvas from "html2canvas";

const customerService = new CustomerService();
const inventoryService = new InventoryService();
const invoiceService = new InvoiceService();

@Component({ name: "CreateInvoice", components: { SolarButton } })
export default class CreateInvoice extends Vue {
  invoiceStep = 1;
  invoice: IInvoice = this.generateNewInvoice();
  customers: ICustomer[] = [];
  selectedCustomerId = 0;
  inventory: IProductInventory[] = [];
  lineItems: ILineItem[] = [];
  newItem: ILineItem = this.generateNewLineItem();

  // define computed properties
  get canGoPrev() {
    return this.invoiceStep !== 1;
  }
  get canGoNext() {
    if (this.invoiceStep === 1) {
      return this.selectedCustomerId !== 0;
    }

    if (this.invoiceStep === 2) {
      return this.lineItems.length > 0;
    }

    if (this.invoiceStep === 3) {
      return false;
    }

    return false;
  }
  get runningTotal() {
    // reduce() initializes 'accumulator' with value zero (2nd parameter)
    return this.lineItems.reduce(
      (accumulator, item) =>
        accumulator + item["product"]["price"] * item["quantity"],
      0
    );
  }
  get selectedCustomer() {
    return this.customers.find((cust) => cust.id == this.selectedCustomerId);
  }

  async submitInvoice(): Promise<void> {
    this.invoice = {
      customerId: this.selectedCustomerId,
      lineItems: this.lineItems,
      createdOn: new Date(),
      updatedOn: new Date(),
    };

    await invoiceService.makeNewInvoice(this.invoice);
    this.downloadPdf();
    await this.$router.push("/orders");
  }

  downloadPdf() {
    let pdf = new jsPDF("p", "pt", "a4", true);
    // ! at the end means: I am sure it will not return 'null' but an Element
    let invoice = document.getElementById("invoice")!;
    let width = (this.$refs["invoice"] as any).clientWidth;
    let height = (this.$refs["invoice"] as any).clientHeight;

    html2canvas(invoice).then((canvas) => {
      let image = canvas.toDataURL("image/png");
      pdf.addImage(image, "PNG", 0, 0, width * 0.55, height * 0.55);
      pdf.save("invoice");
    });
  }

  startOver(): void {
    this.invoice = this.generateNewInvoice();
    this.invoiceStep = 1;
  }

  generateNewLineItem() {
    return {
      product: {
        id: 0,
        createdOn: new Date(),
        updatedOn: new Date(),
        name: "",
        description: "",
        price: 0,
        isTaxable: true,
        isArchived: false,
      },
      quantity: 0,
    };
  }
  generateNewInvoice() {
    return {
      createdOn: new Date(),
      updatedOn: new Date(),
      customerId: 0,
      lineItems: [],
    };
  }

  addLineItem() {
    let newItem: ILineItem = {
      product: this.newItem.product,
      quantity: Number(this.newItem.quantity),
    };

    let existingItems = this.lineItems.map(function (item) {
      if (item.product.id === newItem.product.id) {
        return item.product.id;
      }
    });

    if (existingItems.includes(newItem.product.id)) {
      let lineItem = this.lineItems.find(
        (item) => item.product.id === newItem.product.id
      );

      // if() just to avoid compiler warnings (lineItem possibly 'undefined')
      if (lineItem) {
        let currentQuantity = Number(lineItem.quantity);
        let updatedQuantity = currentQuantity + newItem.quantity;
        lineItem.quantity = updatedQuantity;
      }
    } else {
      this.lineItems.push(this.newItem);
    }

    this.newItem = { product: this.generateNewLineItem().product, quantity: 0 };
  }

  finalizeOrder() {
    this.invoiceStep = 3;
  }

  prev(): void {
    if (this.invoiceStep === 1) {
      return;
    }
    this.invoiceStep -= 1;
  }
  next(): void {
    if (this.invoiceStep === 3) {
      return;
    }
    this.invoiceStep += 1;
  }

  async initialize(): Promise<void> {
    // another way to do it
    //customerService.getCustomers().then(resp => this.customers = resp).catch(() => {} //show error modal/msg)

    this.customers = await customerService.getCustomers();
    this.inventory = await inventoryService.getInventory();
  }

  async created() {
    await this.initialize();
  }
}
</script>

<style scoped lang="scss">
@import "@/scss/global.scss";

.invoice-steps-actions {
  display: flex;
  width: 100%;
}

.invoice-step {
}

.invoice-step-detail {
  margin: 1.2rem;
}

.invoice-order-list {
  margin-top: 1.2rem;
  padding: 0.8rem;
  .line-item {
    display: flex;
    border-bottom: 1px solid #ccc;
    padding: 0.8rem;
  }
  .item-col {
    flex-grow: 1;
  }
}

.invoice-item-actions {
  display: flex;
}

.price-pre-tax {
  font-weight: bold;
}

.price-final {
  font-weight: bold;
  color: $solar-green;
}

.due {
  font-weight: bold;
}

.invoice-header {
  width: 100%;
  margin-bottom: 1.2rem;
}

.invoice-logo {
  padding-top: 1.4rem;
  text-align: center;
  img {
    width: 280px;
  }
}
</style>