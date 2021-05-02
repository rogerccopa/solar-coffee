import { IProductInventory } from "@/types/Product";
import { IShipment } from "@/types/IShipment";
import axios from "axios";
import { IInventoryTimeline } from "@/types/InventoryGraph";

/*
* Inventory Service
* Provides UI business logic associated with product inventory
*/
export class InventoryService {
    // this value comes from the 'environment variables' file (.env)
    API_URL = process.env.VUE_APP_API_URL;

    public async getInventory(): Promise<IProductInventory[]> {
        const apiReqUrl = `${this.API_URL}/inventory/`;
        // console.log("getInventory: ", apiReqUrl);
        const result = await axios.get(apiReqUrl);
        return result.data;
    }

    public async updateInventoryQuantity(shipment: IShipment) {
        const result = await axios.patch(`${this.API_URL}/inventory`, shipment);
        return result.data;
    }

    public async getSnapshotHistory(): Promise<IInventoryTimeline> {
        let result: any = await axios.get(`${this.API_URL}/inventory/snapshot`);
        return result.data;
    }
}
