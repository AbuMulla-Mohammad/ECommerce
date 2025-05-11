using ECommerce.API.Models;
using ECommerce.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartsController(ICartService cartService,UserManager<ApplicationUser> userManager)
        {
            this._cartService = cartService;
            this._userManager = userManager;
        }
        [HttpPost("{ProductId}")]
        public async Task<IActionResult> AddToCart([FromRoute]int ProductId,[FromQuery] int count)
        {
            try
            {
                var appUser=_userManager.GetUserId(User);//return string with the user id
                var cart = new Cart()
                {
                    ProductId = ProductId,
                    ApplicationUserId = appUser,
                    Count = count
                };
                await _cartService.AddAsync(cart);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
