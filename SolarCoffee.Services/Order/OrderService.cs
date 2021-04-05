using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolarCoffee.Data;
using SolarCoffee.Data.Models;
using SolarCoffee.Services.Inventory;
using SolarCoffee.Services.Product;

namespace SolarCoffee.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly SolarDbContext _db;
        private readonly ILogger _logger;
        private readonly IProductService _productService;
        private readonly IInventoryService _inventoryService;

        public OrderService(
            SolarDbContext dbContext,
            ILogger logger,
            IProductService productService,
            IInventoryService inventoryService)
        {
            _db = dbContext;
            _logger = logger;
            _productService = productService;
            _inventoryService = inventoryService;
        }

        /// <summary>
        /// Creates an open SalesOrder
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        ServiceResponse<SalesOrder> IOrderService.GenerateOpenOrder(SalesOrder order)
        {
            _logger.LogInformation("Generating new open order");

            foreach (var item in order.SalesOrderItems)
            {
                item.Product = _productService.GetProductById(item.Product.Id);

                var inventoryId = _inventoryService.GetByProductId(item.Product.Id).Id;

                _inventoryService.UpdateUnitsAvailable(inventoryId, -item.Quantity);

            }

            var now = DateTime.UtcNow;

            try
            {
                _db.SalesOrders.Add(order);
                _db.SaveChanges();

                return new ServiceResponse<SalesOrder>
                {
                    IsSuccess = true,
                    Message = "Open Order created",
                    Data = order,
                    Time = now
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<SalesOrder>
                {
                    IsSuccess = false,
                    Message = ex.StackTrace,
                    Data = null,
                    Time = now
                };
            }
        }

        /// <summary>
        /// Return all orders
        /// </summary>
        /// <returns></returns>
        List<SalesOrder> IOrderService.GetOrders()
        {
            return _db.SalesOrders
                .Include(order => order.Customer)
                    .ThenInclude(customer => customer.PrimaryAddress)
                .Include(order => order.SalesOrderItems)
                    .ThenInclude(item => item.Product)
                .ToList();
        }

        /// <summary>
        /// Mark an open Order as paid
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        ServiceResponse<bool> IOrderService.MarkFulfilled(int orderId)
        {
            var now = DateTime.UtcNow;
            var order = _db.SalesOrders.Find(orderId);
            order.UpdatedOn = now;
            order.IsPaid = true;

            try
            {
                _db.SalesOrders.Update(order);
                _db.SaveChanges();

                return new ServiceResponse<bool>
                {
                    IsSuccess = true,
                    Message = $"Order {order.Id} closed: Invoice paid in full",
                    Time = now,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    IsSuccess = false,
                    Message = ex.StackTrace,
                    Time = now,
                    Data = false
                };
            }
        }
    }
}
