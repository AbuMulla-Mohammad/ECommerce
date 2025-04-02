using ECommerce.API.Data;
using ECommerce.API.DTOs.Requests;
using ECommerce.API.DTOs.Responses;
using ECommerce.API.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context=context;
        [HttpGet("")]
        public IActionResult GetAll()
        {
            try
            {
                var products = _context.Products.ToList();
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
        public IActionResult GetById([FromRoute] int id)
        {
            try
            {
                if (id <= 0) return BadRequest("Invalid product id");
                var product = _context.Products.Find(id);
                if (product == null) return NotFound("Product not found");
                return Ok(product.Adapt<ProductResponse>());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("")]
        public IActionResult Create([FromForm] ProductRequest productRequest)
        {
            try
            {
                var file = productRequest.MainImage;
                var product=productRequest.Adapt<Product>();
                if (file!= null&&file.Length>0)
                {
                    //create a new name for the file
                    var newFileName=Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
                    var filePath=Path.Combine(Directory.GetCurrentDirectory(),"Images", newFileName);
                    using(var stream=System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }
                    product.MainImage = newFileName;
                    _context.Products.Add(product);
                    _context.SaveChanges();
                    return CreatedAtAction(nameof(GetById), new {id=product.Id},product.Adapt<ProductResponse>());
                }
                return BadRequest("Main image is required");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            
        }
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                if(id <= 0) return BadRequest("Invalid product id");
                var product=_context.Products.Find(id);
                if (product is null) return NotFound("Product not found");
                var filePath=Path.Combine(Directory.GetCurrentDirectory(),"Images",product.MainImage);
                if(System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                _context.Products.Remove(product);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
