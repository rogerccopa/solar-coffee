using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SolarCoffee.Data;

namespace SolarCoffee.Services.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly SolarDbContext _db;

        public CustomerService(SolarDbContext dbContext)
        {
            _db = dbContext;
        }

        /// <summary>
        /// Adds a new Customer record
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <returns>ServiceResponse<Customer></returns>
        ServiceResponse<Data.Models.Customer> ICustomerService.CreateCustomer(Data.Models.Customer customer)
        {
            try
            {
                _db.Customers.Add(customer);
                _db.SaveChanges();

                return new ServiceResponse<Data.Models.Customer>{
                    IsSuccess = true,
                    Data = customer,
                    Message = "Customer was added",
                    Time = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Data.Models.Customer>
                {
                    IsSuccess = false,
                    Data = customer,
                    Message = ex.StackTrace,
                    Time = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Deletes a Customer record
        /// </summary>
        /// <param name="customerId">Customer primary key</param>
        /// <returns>ServiceResponse<bool></returns>
        ServiceResponse<bool> ICustomerService.DeleteCustomer(int customerId)
        {
            var customer = _db.Customers.Find(customerId);
            var now = DateTime.UtcNow;

            if (customer == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    Message = "Customer to delete not found!",
                    Time = now
                };
            }

            try
            {
                _db.Customers.Remove(customer);
                _db.SaveChanges();

                return new ServiceResponse<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Customer removed",
                    Time = now
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    Message = ex.StackTrace,
                    Time = now
                };
            }
        }

        /// <summary>
        /// Returns a list of customers from the database
        /// </summary>
        /// <returns>List<Customer></returns>
        List<Data.Models.Customer> ICustomerService.GetAllCustomers()
        {
            return _db.Customers
                .Include(customer => customer.PrimaryAddress)
                .OrderBy(customer => customer.LastName)
                .ToList();
        }

        /// <summary>
        /// Gets a Customer record by primary key
        /// </summary>
        /// <param name="customerId">Customer primary key</param>
        /// <returns>Customer</returns>
        Data.Models.Customer ICustomerService.GetById(int customerId)
        {
            return _db.Customers.Find(customerId);
        }
    }
}
