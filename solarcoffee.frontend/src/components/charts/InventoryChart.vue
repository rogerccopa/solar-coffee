<template>
  <div>
    <apexchart
      type="area"
      width="100%"
      height="300"
      :options="options"
      :series="series"
    >
    </apexchart>
  </div>
</template>
<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { IInventoryTimeline } from "@/types/InventoryGraph";
import { Sync, Get } from "vuex-pathify";

@Component({
  name: "InventoryChart",
  components: {},
})
export default class InventoryChart extends Vue {
  @Sync("snapshotTimeLine")
  snapshotTimeline: IInventoryTimeLine;

  @Get("isTimelineBuilt")
  isTimelineBuild?: boolean;

  get options() {
    return {
      dataLabels: { enabled: false },
      fill: { type: "gradient" },
      stroke: { curve: "smooth" },
      xaxis: {
        categories: this.snapshotTimeLine.timeline,
        type: "datetime",
      },
    };
  }

  get series() {
    return this.snapshotTimeline.productInventorySnapshots.map((snapshot) => ({
      name: snapshot.productId,
      data: snapshot.quantityOnHand,
    }));
  }

  async created() {
    await this.$store.dispatch("assignSnapshots");
  }
}
</script>
<style scooped lang="scss">
</style>