using ECommerce.API.DTOs.Responses;
using ECommerce.API.Models;
using ECommerce.API.Services;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            this._cartService = cartService;
        }
        [HttpPost("{ProductId}")]
        public async Task<IActionResult> AddToCart([FromRoute]int ProductId, CancellationToken cancellationToken)
        {
            try
            {
                var appUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;//return the value of the claim NameIdentifier which is the user id// this will help me get any value from the token
                //var appUser=_userManager.GetUserId(User);//return string with the user id // the User object is from the cookies
                var cart = await _cartService.AddToCartAsync(appUser, ProductId, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("")]
        public async Task<IActionResult> GetCartProducts(CancellationToken cancellationToken)
        {
            try
            {
                var appUser=User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var cartProducts =await _cartService.GetCartProducts(appUser, cancellationToken);
                var cartResponse = cartProducts.Select(c=>c.Product).Adapt<IEnumerable<CartResponse>>();//return only the products in the cart
                var totalPrice = cartProducts.Sum(c => c.Product.Price * c.Count);
                return Ok(new { cartProducts , totalPrice });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
