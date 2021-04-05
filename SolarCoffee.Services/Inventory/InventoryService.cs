using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolarCoffee.Data;
using SolarCoffee.Data.Models;

namespace SolarCoffee.Services.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly SolarDbContext _db;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(SolarDbContext dbContext, ILogger<InventoryService> logger)
        {
            _db = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Gets a ProductInventory instance by Product Id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        ProductInventory IInventoryService.GetByProductId(int productId)
        {
            return _db.ProductInventories
                .Include(pi => pi.Product)
                .FirstOrDefault(pi => pi.Product.Id == productId);
        }

        /// <summary>
        /// Returns all current inventory from the database
        /// </summary>
        /// <returns></returns>
        List<ProductInventory> IInventoryService.GetCurrentInventory()
        {
            return _db.ProductInventories
                .Include(pi => pi.Product)
                .Where(pi => !pi.Product.IsArchived)
                .ToList();
        }

        /// <summary>
        /// Return Snapshot history for the previous 6 hours
        /// </summary>
        /// <returns></returns>
        List<ProductInventorySnapshot> IInventoryService.GetSnapshotHistory()
        {
            var earliest = DateTime.UtcNow - TimeSpan.FromHours(6);

            return _db.ProductInventorySnapshots
                .Include(snap => snap.Product)
                .Where(snap => snap.SnapshotTime > earliest && !snap.Product.IsArchived)
                .ToList();
        }

        /// <summary>
        /// Updates number of units available of the provided Product id
        /// Adjusts QuantityOnHand by adjustment value
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <param name="adjustment">Number of units added/removed from inventory</param>
        /// <returns></returns>
        ServiceResponse<ProductInventory> IInventoryService.UpdateUnitsAvailable(int productId, int adjustment)
        {
            var now = DateTime.UtcNow;

            try
            {
                var inventory = _db.ProductInventories
                    .Include(pi => pi.Product)
                    .First(pi => pi.Product.Id == productId);

                inventory.QuantityOnHand += adjustment;

                // use try-catch so it doesn't interrupt the inventory update.
                try
                {
                    CreateSnapshot(inventory);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error creating inventory snapshot");
                    _logger.LogError(ex.StackTrace);
                }

                _db.SaveChanges();

                return new ServiceResponse<ProductInventory>
                {
                    Data = inventory,
                    IsSuccess = true,
                    Message = $"Product {productId} inventory adjusted",
                    Time = now
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<ProductInventory>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Error updating ProductInventory QuantityOnHand",
                    Time = now
                };
            }
        }

        /// <summary>
        /// Creates a Snapshot record using the provided ProductInventory instance
        /// </summary>
        /// <param name="inventory"></param>
        private void CreateSnapshot(ProductInventory inventory)
        {
            var now = DateTime.UtcNow;

            var snapshot = new ProductInventorySnapshot
            {
                Product = inventory.Product,
                QuantityOnHand = inventory.QuantityOnHand,
                SnapshotTime = now
            };

            // Entity framework will infer type of 'snapshot' & its respective table
            _db.Add(snapshot);
        }
    }
}
