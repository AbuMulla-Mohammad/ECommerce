using ECommerce.API.DTOs.Requests;
using ECommerce.API.Models;
using ECommerce.API.Services;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAccountService _accountService;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            IAccountService accountService
            )
        {
            this._signInManager = signInManager;
            this._accountService = accountService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                var result = await _accountService.RegisterAsync(registerRequest, Request);
                if (result.Succeeded) return NoContent();
                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var (success,token,errorMessage) = await _accountService.LoginAsync(loginRequest, Request);
                if (success)
                {
                    return Ok(new { token });
                }
                return BadRequest(new { message = errorMessage ?? "Invalid login attempt." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated." });
                }

                var (success, errors, errorMessage) = await _accountService.ChangePasswordAsync(userId, changePasswordRequest);

                if (success)
                {
                    return NoContent();
                }

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return BadRequest(new { message = errorMessage });
                }

                return BadRequest(errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string userId)
        {
            /*
             Note: the token here is the email confirmation token generated during registration and it is URL encoded, so we need to decode it before using it.
             */
            try
            {
                var (Success, Errors, Message) = await _accountService.ConfirmEmail(userId, token);
                if (Success)
                {
                    return Ok(new { message = Message });
                }
                if (!string.IsNullOrEmpty(Message))
                {
                    return BadRequest(new { message = Message });
                }
                return BadRequest(new { errors = Errors });
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("SendResetPasswordCode")]
        public async Task<IActionResult> SendResetPasswordCode([FromBody] SendResetPasswordCodeRequest sendResetPasswordCodeRequest)
        {
            try
            {
                var (success, errorMessage) = await _accountService.SendResetPasswordCode(sendResetPasswordCodeRequest.Email);
                if(success)
                {
                    return Ok(new { message="Reset Code sent to your Email successfully "});
                }
                else
                {
                    return BadRequest(new { message = errorMessage ?? "Failed to send password reset email." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("ResetPassowrd")]
        public async Task<IActionResult> ResetPassowrd([FromBody] ForgotPasswordRequest forgotPasswordRequest )
        {
            try
            {
                var(success, errorMessage)= await _accountService.ResetPassword(forgotPasswordRequest.Email, forgotPasswordRequest.Code, forgotPasswordRequest.Password);
                if (success)
                {
                    return Ok("Your Password Reset Successfully");
                }
                else
                {
                    return BadRequest(new { message = errorMessage });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
