using System;
using System.Collections.Generic;

namespace SolarCoffee.Services.Customer
{
    public interface ICustomerService
    {
        public List<Data.Models.Customer> GetAllCustomers();
        public ServiceResponse<Data.Models.Customer> CreateCustomer(Data.Models.Customer customer);
        public ServiceResponse<bool> DeleteCustomer(int customerId);
        public Data.Models.Customer GetById(int customerId);
    }
}
