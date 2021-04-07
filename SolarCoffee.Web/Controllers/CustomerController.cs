using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Services.Customer;
using SolarCoffee.Web.Serialization;
using SolarCoffee.Web.ViewModels;

namespace SolarCoffee.Web.Controllers
{
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerService _customerService;

        public CustomerController(ILogger<CustomerController> logger, ICustomerService customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        [HttpPost("api/customer")]
        public ActionResult CreateCustomer([FromBody] CustomerModel customer)
        {
            _logger.LogInformation("Creating a new Customer");
            customer.CreatedOn = DateTime.UtcNow;
            customer.UpdatedOn = DateTime.UtcNow;
            var customerData = CustomerMapper.SerializeCustomer(customer);
            var response = _customerService.CreateCustomer(customerData);

            return Ok(response);
        }

        [HttpGet("api/customer")]
        public ActionResult GetCustomers()
        {
            var customers = CustomerMapper
                .SerializeCustomers(_customerService.GetAllCustomers());

            return Ok(customers);
        }

        [HttpDelete("api/customer/{customerId}")]
        public ActionResult DeleteCustomer(int customerId)
        {
            _logger.LogInformation("Deleting a customer");
            var response = _customerService
                .DeleteCustomer(customerId);

            return Ok(response);
        }
    }
}
