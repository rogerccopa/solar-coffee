using System;
using System.Collections.Generic;
using SolarCoffee.Data.Models;

namespace SolarCoffee.Services.Order
{
    public interface IOrderService
    {
        public List<SalesOrder> GetOrders();
        public ServiceResponse<SalesOrder> GenerateOpenOrder(SalesOrder order);
        public ServiceResponse<bool> MarkFulfilled(int orderId);
    }
}
