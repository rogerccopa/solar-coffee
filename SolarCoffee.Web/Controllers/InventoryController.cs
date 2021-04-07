using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Services.Inventory;
using SolarCoffee.Web.Serialization;
using SolarCoffee.Web.ViewModels;

namespace SolarCoffee.Web.Controllers
{
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly ILogger<InventoryController> _logger;
        private readonly IInventoryService _inventoryService;

        public InventoryController(
            ILogger<InventoryController> logger,
            IInventoryService inventoryService)
        {
            _logger = logger;
            _inventoryService = inventoryService;
        }

        [HttpGet("api/inventory")]
        public ActionResult GenCurrentInventory()
        {
            _logger.LogInformation("Getting all inventory");

            var inventory = _inventoryService.GetCurrentInventory();
            var inventoryModel = inventory
                .Select(inv => new ProductInventoryModel{
                    Id = inv.Id,
                    Product = ProductMapper.SerializeProductModel(inv.Product),
                    IdealQuantity = inv.IdealQuantity,
                    QuantityOnHand = inv.QuantityOnHand
                })
                .OrderBy(inv => inv.Product.Name)
                .ToList();

            return Ok(inventoryModel);
        }

        [HttpPatch("api/inventory")]
        public ActionResult UpdateInventory([FromBody] ShipmentModel shipment)
        {
            _logger.LogInformation(
                $"Updating inventory " +
                $"for {shipment.ProductId} " +
                $"adjustment {shipment.Adjustment}");

            var productId = shipment.ProductId;
            var adjustment = shipment.Adjustment;
            var inventory = _inventoryService.UpdateUnitsAvailable(productId, adjustment);

            return Ok(inventory);
        }
    }
}
