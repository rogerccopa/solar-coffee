using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SolarCoffee.Data;
using SolarCoffee.Data.Models;
using SolarCoffee.Services.Customer;
using Xunit;

// libraries used to creat this testing:
// - Microsoft.EntityFrameworkCore
// - Microsoft.EntityFrameworkCore.InMemory
// - FluentAssertions

namespace SolarCoffee.Test
{
    public class TestCustomerService
    {
        [Fact]
        public void CustomerService_GetsAllCustomers_GivenTheyExist()
        {
            var options = new DbContextOptionsBuilder<SolarDbContext>()
                .UseInMemoryDatabase("gets_all")
                .Options;

            var context = new SolarDbContext(options);

            // sut => System Under Test
            ICustomerService sut = new CustomerService(context);

            sut.CreateCustomer(new Customer { Id = 123 });
            sut.CreateCustomer(new Customer { Id = 456 });

            var allCustomers = sut.GetAllCustomers();

            allCustomers.Count.Should().Be(2);
        }

        [Fact]
        public void CustomerService_CreateCustomer_GivenNewCustomerObject()
        {
            var options = new DbContextOptionsBuilder<SolarDbContext>()
                .UseInMemoryDatabase("Add_writes_to_database")
                .Options;

            var context = new SolarDbContext(options);
            ICustomerService service = new CustomerService(context);

            service.CreateCustomer(new Customer { Id = 123 });
            context.Customers.Single().Id.Should().Be(123);
        }

        [Fact]
        public void CustomerService_DeleteCustomer_GivenId()
        {
            var options = new DbContextOptionsBuilder<SolarDbContext>()
                .UseInMemoryDatabase("deletes_one")
                .Options;
            var context = new SolarDbContext(options);
            ICustomerService sut = new CustomerService(context);
            sut.CreateCustomer(new Customer { Id = 123 });

            sut.DeleteCustomer(123);
            var allCustomers = sut.GetAllCustomers();
            allCustomers.Count.Should().Be(0);
        }
    }
}
