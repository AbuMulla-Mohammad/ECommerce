using ECommerce.API.Data;
using ECommerce.API.DTOs.Requests;
using ECommerce.API.DTOs.Responses;
using ECommerce.API.Models;
using ECommerce.API.Services;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult GetAll()
        {
            try
            {
                var categories = _categoryService.GetAll();
                return Ok(categories.Adapt<IEnumerable<CategoryResponse>>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute]int id)
        {
            
            try
            {
                if (id <= 0)
                {
                    return BadRequest("invalid ID");
                }
                var category = _categoryService.Get(c=>c.Id==id);
                return category == null ? NotFound() : Ok(category.Adapt<CategoryResponse>());

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("")]
        public IActionResult Create([FromBody] CategoryRequest category)
        {
           var createdCategory = _categoryService.Add(category.Adapt<Category>());
            //return Created($"https://localhost:7262/api/Categories/{category.Id}",category);
            //return Created($"{Request.Scheme}://{Request.Host}/api/Categories/{category.Id}", category);
            //this better
            return CreatedAtAction(nameof(GetById), new { createdCategory.Id}, createdCategory);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteById([FromRoute]int id)
        {
            var deletedCategory = _categoryService.Remove(id);
            if (deletedCategory == false) return NotFound();
            return NoContent();
        }
        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] CategoryRequest category)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest("Invalid ID");
                }
                //var categoryInDb = _context.Categories.Find(id);//track the object
                var categoryInDb = _categoryService.Edit(id, category.Adapt<Category>());
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
