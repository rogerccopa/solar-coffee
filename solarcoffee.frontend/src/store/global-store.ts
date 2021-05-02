import { commit, make } from "vuex-pathify";
import { IInventoryTimeline } from "@/types/InventoryGraph";
import { InventoryService } from "@/services/inventory-service";

class GlobalStore {
    snapshotTimeLine: IInventoryTimeline = {
        productInventorySnapshots: [],
        timeLine: []
    };

    isTimeLineBuilt: boolean = false;
}

const state = new GlobalStore();
const mutations = make.mutations(state);
const actions = {
    async assignSnapshots({ commit }) {
        const inventoryService = new InventoryService();
        let resp = await inventoryService.getSnapshotHistory();

        let timeLine: IInventoryTimeLine = {
            productInventorySnapshots: resp.productInventorySnapshots,
            timeLine: resp.timeLine
        };
    };

    commit("SET_SNAPSHOT_TIMELINE", timeLine);
    commit("SET_IS_TIMELINE_BUILT", true);
};
const getters = {};

export default {
    state,
    mutations,
    actions,
    getters
};
