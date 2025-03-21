using ECommerce.API.Data;
using ECommerce.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("")]
        public IActionResult getAll()
        {
            try
            {
                var categories = _context.Categories.ToList();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}")]
        public IActionResult getById([FromRoute]int id)
        {
            
            try
            {
                if (id <= 0)
                {
                    return BadRequest("invalid ID");
                }
                var category = _context.Categories.Find(id);
                return category == null ? NotFound() : Ok(category);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("")]
        public IActionResult create([FromBody] Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            //return Created($"https://localhost:7262/api/Categories/{category.Id}",category);
            //return Created($"{Request.Scheme}://{Request.Host}/api/Categories/{category.Id}", category);
            //this better
            return CreatedAtAction(nameof(getById), new {category.Id}, category);
        }
        [HttpDelete("{id}")]
        public IActionResult deleteById([FromRoute]int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
