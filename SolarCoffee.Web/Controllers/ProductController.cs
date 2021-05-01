using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Services.Product;
using SolarCoffee.Web.Serialization;
using SolarCoffee.Web.ViewModels;

namespace SolarCoffee.Web.Controllers
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        // We use IProductService (Interface) because we want to
        // program to the Abstraction not to the Implementation
        private readonly IProductService _productService;

        public ProductController(
            ILogger<ProductController> logger,
            IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        [HttpGet("/api/product")]
        public ActionResult GetProduct()
        {
            _logger.LogInformation("Getting all products");
            var products = _productService.GetAllProducts();
            var productViewModels = products.Select(ProductMapper.SerializeProductModel);
            return Ok(productViewModels);
        }

        [HttpPatch("api/product/{productId}")]
        public ActionResult ArchiveProduct(int productId)
        {
            _logger.LogInformation($"Archiving product {productId}");
            var archiveResult = _productService.ArchiveProduct(productId);

            return Ok(archiveResult);
        }

        [HttpPost("api/product")]
        public ActionResult AddProduct([FromBody] ProductModel newProduct)
        {
            // check if .NET binded successfully client request fields to our ProductModel
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Saving new product");
            var newProductDataModel = ProductMapper.SerializeProductModel(newProduct);
            var newProductResponse = _productService.CreateProduct(newProductDataModel);
            return Ok(newProductResponse.Data);
        }
    }
}
