import { commit, make } from "vuex-pathify";
import { IInventoryTimeline } from "@/types/InventoryGraph";
import { InventoryService } from "@/services/inventory-service";

class GlobalStore {
    snapshotTimeline: IInventoryTimeline = {
        productInventorySnapshots: [],
        timeline: []
    };

    isTimelineBuilt = false;
}

const state = new GlobalStore();
const mutations = make.mutations(state);
const actions = {
    async assignSnapshots({ commit }) {
        const inventoryService = new InventoryService();
        const resp = await inventoryService.getSnapshotHistory();

        const timeline: IInventoryTimeline = {
            productInventorySnapshots: resp.productInventorySnapshots,
            timeline: resp.timeline
        };

        commit("SET_SNAPSHOT_TIMELINE", timeline);
        commit("SET_IS_TIMELINE_BUILT", true);
    },
};
const getters = {};

export default {
    state,
    mutations,
    actions,
    getters
};
