using ECommerce.API.DTOs.Requests;
using ECommerce.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CheckOutsController : ControllerBase
    {
        private readonly ICheckOutService _checkOutService;

        public CheckOutsController(ICheckOutService checkOutService)
        {
            this._checkOutService = checkOutService;
        }
        [HttpGet("")]
        public async Task<IActionResult> Pay(CancellationToken cancellationToken, [FromBody] PaymentRequest paymentRequest)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "User is not authorized." });
                var successUrl = $"{Request.Scheme}://{Request.Host}/api/CheckOuts/Success";
                var cancelUrl = $"{Request.Scheme}://{Request.Host}/api/CheckOuts/Cancel";
                try
                {
                    var session = await _checkOutService.PayAsync(userId, successUrl, cancelUrl, paymentRequest, cancellationToken);
                    if (session == null)
                    {
                        return Ok(new { message = "Order placed with Cash payment method." });
                    }
                    return Ok(new { session.Url });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("{sessionId}")]
        public async Task<IActionResult> Refund([FromRoute] string sessionId, CancellationToken cancellationToken)
        {
            try
            {
                var refund=await _checkOutService.RefundAsync(sessionId, cancellationToken);
                if(refund.Status== "succeeded")
                {
                    return Ok(new { message = "Refund successful" });
                }
                else
                {
                    return BadRequest(new
                    {
                        message = "Refund failed",
                        status = refund.Status
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("Success")]
        [AllowAnonymous]
        public IActionResult Success()
        {
            return Ok(new { message = "Payment successful" });
        }
    }
}
