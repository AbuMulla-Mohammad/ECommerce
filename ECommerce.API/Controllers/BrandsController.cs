using ECommerce.API.DTOs.Requests;
using ECommerce.API.DTOs.Responses;
using ECommerce.API.Models;
using ECommerce.API.Services;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetAll()
        {
            var brands=_brandService.GetAll();
            return Ok(brands);
        }
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            try
            {
                if (id <= 0) return BadRequest("Invalid ID");
                var brand=_brandService.Get(b=>b.Id == id);
                if (brand == null) return NotFound();
                return Ok(brand.Adapt<BrandResponse>());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            } 
        }
        [HttpPost("")]
        public IActionResult Create([FromBody]BrandRequest brand)
        {
            var brandToCreate=_brandService.Add(brand.Adapt<Brand>());
            return CreatedAtAction(nameof(GetById), new { brandToCreate.Id}, brandToCreate);
        }
        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] BrandRequest brand)
        {
            try
            {
                if (id == 0) return BadRequest("Invalid ID");
                var editedBrand = _brandService.Edit(id, brand.Adapt<Brand>());
                if (!editedBrand) return NotFound();
                return NoContent();
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
                if (id <= 0) return BadRequest("Invalid ID");
                var deletedBrand = _brandService.Remove(id);
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
