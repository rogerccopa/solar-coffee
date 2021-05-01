﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Services.Customer;
using SolarCoffee.Services.Order;
using SolarCoffee.Web.Serialization;
using SolarCoffee.Web.ViewModels;

namespace SolarCoffee.Web.Controllers
{
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;

        public OrderController(
            ILogger<OrderController> logger,
            IOrderService orderService,
            ICustomerService customerService)
        {
            _logger = logger;
            _orderService = orderService;
            _customerService = customerService;
        }

        [HttpPost("api/invoice")]
        public ActionResult GenerateNewOrder([FromBody] InvoiceModel invoice)
        {
            // check if .NET binded successfully client request fields to our OrderModel
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            _logger.LogInformation("Generating invoice");
            var order = OrderMapper.SerializeInvoiceToOrder(invoice);
            order.Customer = _customerService.GetById(invoice.CustomerId);
            _orderService.GenerateOpenOrder(order);
            return Ok();
        }

        [HttpGet("api/order")]
        public ActionResult GetOrders()
        {
            var orders = _orderService.GetOrders();
            var orderModels = OrderMapper.SerializeOrdersToViewModels(orders);

            return Ok(orderModels);
        }

        [HttpPatch("api/order/complete/{orderId}")]
        public ActionResult MarkOrderComplete(int orderId)
        {
            _logger.LogInformation($"Marking order {orderId} complete");
            _orderService.MarkFulfilled(orderId);

            return Ok();
        }
    }
}
