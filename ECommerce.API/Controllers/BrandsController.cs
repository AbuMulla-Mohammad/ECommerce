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
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            this._brandService = brandService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            
            
            try
            {
                var brands = await _brandService.GetAsync();
                return Ok(brands.Adapt<IEnumerable<BrandResponse>>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                if (id <= 0) return BadRequest("Invalid ID");
                var brand= await _brandService.GetOneAsync(b=>b.Id == id);
                if (brand == null) return NotFound();
                return Ok(brand.Adapt<BrandResponse>());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            } 
        }
        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody] BrandRequest brand)
        {
            try
            {
                var brandToCreate = await _brandService.AddAsync(brand.Adapt<Brand>());
                return CreatedAtAction(nameof(GetById), new { brandToCreate.Id }, brandToCreate);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] BrandRequest brand)
        {
            try
            {
                if (id == 0) return BadRequest("Invalid ID");
                var editedBrand = await _brandService.EditAsync(id, brand.Adapt<Brand>());
                if (!editedBrand) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                if (id <= 0) return BadRequest("Invalid ID");
                var deletedBrand = await _brandService.RemoveAsync(id);
                if (!deletedBrand) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
