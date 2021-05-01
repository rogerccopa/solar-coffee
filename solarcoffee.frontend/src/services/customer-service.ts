import { ICustomer } from "@/types/Customer";
import { IServiceResponse } from "@/types/IServiceResponse";
import axios from "axios";

/*
* Customer Service
* Provides UI business logic assotiated with Customers in our system
*/
export class CustomerService {
    // this value comes from the 'environment variables' file (.env)
    API_URL = process.env.VUE_APP_API_URL;

    public async getCustomers(): Promise<ICustomer[]> {
        const result: any = await axios.get(`${this.API_URL}/customer`);
        return result.data;
    }

    public async addCustomer(newCustomer: ICustomer): Promise<IServiceResponse<ICustomer>> {
        const result: any = await axios.post(`${this.API_URL}/customer`, newCustomer);
        return result.data;
    }

    public async deleteCustomer(customerId: number): Promise<boolean> {
        const result: any = await axios.delete(`${this.API_URL}/customer/${customerId}`);
        return result.data;
    }
}