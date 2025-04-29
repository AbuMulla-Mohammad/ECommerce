using ECommerce.API.Data;
using ECommerce.API.DTOs.Requests;
using ECommerce.API.DTOs.Responses;
using ECommerce.API.Models;
using ECommerce.API.Services;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService=productService;

        [HttpGet("")]
        public IActionResult GetAll([FromQuery] string? query, [FromQuery] int page, [FromQuery] int limit = 10)
        {
            try
            {
                var products = _productService.GetAll(query, page,limit);
                if (products is null)
                {
                    return NotFound("No products found");
                }
                return Ok(products.Adapt<IEnumerable<ProductResponse>>());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                if (id <= 0) return BadRequest("Invalid product id");
                var product = await _productService.GetOneAsync(p=>p.Id==id);
                if (product == null) return NotFound("Product not found");
                return Ok(product.Adapt<ProductResponse>());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdateProductRequest productRequest, CancellationToken cancellationToken)
        {
            try
            {
                var result=await _productService.EditAsync(id, productRequest, cancellationToken);
                if(result)
                {
                    return NoContent();
                }
                return NotFound("Product not found");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("")]
        public async Task<IActionResult> Create([FromForm] ProductRequest productRequest, CancellationToken cancellationToken)
        {
            try
            {
                
                Product? createdProduct = await _productService.AddProductAsync(productRequest, cancellationToken);
                if (createdProduct is null)
                {
                    return BadRequest("Main image is required");
                }
                return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct.Adapt<ProductResponse>());

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            try
            {
                if(id <= 0) return BadRequest("Invalid product id");
                var result= await _productService.DeleteProductAsync(id,cancellationToken);
                if (!result)
                {
                    return NotFound("Product not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
