using ECommerce.API.DTOs.Responses;
using ECommerce.API.Models;
using ECommerce.API.Services;
using ECommerce.API.Utility;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles =StaticData.SuperAdmin)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            this._userService = userService;
        }
        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userService.GetAsync();
                return Ok(users.Adapt<IEnumerable<UserResponse>>());
            }catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var user =await _userService.GetOneAsync(u=>u.Id==id);
                return Ok(user.Adapt<UserResponse>());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("{userId}")]
        public async Task<IActionResult> ChangeRole([FromRoute] string userId,[FromQuery] string newRoleName)
        {
            try
            {
                var result =await _userService.ChangeUserRole(userId, newRoleName);
                if (result)
                {
                    return NoContent();
                }
                return BadRequest(new { message = "User not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
