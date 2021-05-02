export interface IInventoryTimeline {
    productInventorySnapshots: ISnapshot[];
    timeLine: Date[];
}

export interface ISnapshot {
    productId: number;
    quantityOnHand: number[];
}