using ECommerce.API.Data;
using ECommerce.API.DTOs.Requests;
using ECommerce.API.DTOs.Responses;
using ECommerce.API.Models;
using ECommerce.API.Services;
using ECommerce.API.Utility;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class CategoriesController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService= categoryService;
        //this without using the primary constructer
        //private readonly ICategoryService categoryService;

        //public CategoriesController(ICategoryService categoryService)
        //{
        //    this._categoryService = categoryService;
        //}
        //using the primary constructer
        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var categories = await _categoryService.GetAsync();
                return Ok(categories.Adapt<IEnumerable<CategoryResponse>>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            
            try
            {
                if (id <= 0)
                {
                    return BadRequest("invalid ID");
                }
                var category = await _categoryService.GetOneAsync(c=>c.Id==id);
                return category == null ? NotFound() : Ok(category.Adapt<CategoryResponse>());

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("")]
        [Authorize(Roles = $"{StaticData.SuperAdmin},{StaticData.Admin},{StaticData.Company}")]//this will make the controller to be protected and only authenticated users can access it
        public async Task<IActionResult> Create([FromBody] CategoryRequest category, CancellationToken cancellationToken)
        {
            try
            {
                var createdCategory = await _categoryService.AddAsync(category.Adapt<Category>(), cancellationToken);
                //return Created($"https://localhost:7262/api/Categories/{category.Id}",category);
                //return Created($"{Request.Scheme}://{Request.Host}/api/Categories/{category.Id}", category);
                //this better
                return CreatedAtAction(nameof(GetById), new { createdCategory.Id }, createdCategory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{StaticData.SuperAdmin},{StaticData.Admin},{StaticData.Company}")]
        public async Task<IActionResult> DeleteById([FromRoute] int id, CancellationToken cancellationToken)
        {
            try
            {
                var deletedCategory = await _categoryService.RemoveAsync(id, cancellationToken);
                if (deletedCategory == false) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{id}")]
        [Authorize(Roles = $"{StaticData.SuperAdmin},{StaticData.Admin},{StaticData.Company}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CategoryRequest category,CancellationToken cancellationToken)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest("Invalid ID");
                }
                //var categoryInDb = _context.Categories.Find(id);//track the object
                var categoryInDb = await _categoryService.EditAsync(id, category.Adapt<Category>(), cancellationToken);
                if (!categoryInDb)
                {
                    return NotFound();
                }
                //two track of the object and response will cause an error so we have to make the find asNoTracking or use async and await 

                return NoContent();


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
