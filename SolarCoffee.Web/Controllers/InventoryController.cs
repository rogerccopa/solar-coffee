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
            _logger.LogInformation("Getting all inventory...");

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
            // check if .NET binded successfully client request fields to our ProductModel
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation(
                $"Updating inventory " +
                $"for {shipment.ProductId} " +
                $"adjustment {shipment.Adjustment}");

            var productId = shipment.ProductId;
            var adjustment = shipment.Adjustment;
            var inventory = _inventoryService.UpdateUnitsAvailable(productId, adjustment);

            return Ok(inventory);
        }

        [HttpGet("api/inventory/snapshot")]
        public ActionResult GetSnapshotHistory()
        {
            /**
             * {
             *  timeline: [1,2,3,...n],
             *  inventory: [{id:1,qty:[43,21,32,...n]}, {id:2,qty:[12,24,45,...n]}]
             * }
             */

            _logger.LogInformation("Getting snapshot history");

            try
            {
                var snapshotHistory = _inventoryService.GetSnapshotHistory();

                // get distinct points in time a snapshot was collected
                var timelineMarkers = snapshotHistory
                    .Select(tl => tl.SnapshotTime)
                    .Distinct()
                    .ToList();

                // get quantities grouped by id
                var snapshots = snapshotHistory
                    .GroupBy(
                        hist => hist.Product,
                        hist => hist.QuantityOnHand,
                        (key_product, grouping_qtyOnHand) => new ProductInventorySnapshotModel
                        {
                            ProductId = key_product.Id,
                            QuantityOnHand = grouping_qtyOnHand.ToList()
                        })
                    .OrderBy(snap_hs => snap_hs.ProductId)
                    .ToList();

                var viewModel = new SnapshotResponse
                {
                    Timeline = timelineMarkers,
                    ProductInventorySnapshots = snapshots
                };
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Error getting snapshot history.");
                _logger.LogError(ex.StackTrace);
                return BadRequest("Error retrieving snapshot history");
            }
        }
    }
}
