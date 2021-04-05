﻿using System;
using System.Collections.Generic;

namespace SolarCoffee.Services.Customer
{
    public interface ICustomerService
    {
        public List<Data.Models.Customer> GetAllCustomers();
        ServiceResponse<Data.Models.Customer> CreateCustomer(Data.Models.Customer customer);
        ServiceResponse<bool> DeleteCustomer(int customerId);
        Data.Models.Customer GetById(int customerId);
    }
}
