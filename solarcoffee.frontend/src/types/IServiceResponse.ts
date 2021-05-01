export interface IServiceResponse<T> {
    isSuccessful: boolean;
    message: string;
    time: Date;
    data: T;
}