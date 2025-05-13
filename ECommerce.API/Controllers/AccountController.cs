using ECommerce.API.DTOs.Requests;
using ECommerce.API.Models;
using ECommerce.API.Utility;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager
            )
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._emailSender = emailSender;
            this._roleManager = roleManager;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                var applicationUser = registerRequest.Adapt<ApplicationUser>();
                // Here you would typically save the user to the database using the userManager
                var result = await _userManager.CreateAsync(applicationUser, registerRequest.Password);
                if (result.Succeeded)
                {
                    await _emailSender.SendEmailAsync(applicationUser.Email, "Welcome to E-Shopper", $"<h1>Thank you {applicationUser.FirstName} for registering with us.</h1>");
                    await _userManager.AddToRoleAsync(applicationUser, StaticData.Customer);
                    return NoContent();
                }
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
                ApplicationUser? applicationUser = await _userManager.FindByEmailAsync(loginRequest.Email);
                if (applicationUser != null)
                {
                    var result = await _userManager.CheckPasswordAsync(applicationUser, loginRequest.Password);
                    if (result)
                    {
                        // Create claims for the user
                        List<Claim> claims = new();
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, applicationUser.Id));
                        claims.Add(new Claim(ClaimTypes.Email, applicationUser.Email));
                        claims.Add(new Claim(ClaimTypes.Name, applicationUser.UserName));
                        var userRoles= await _userManager.GetRolesAsync(applicationUser);
                        if(userRoles.Count() > 0)
                        {
                            foreach (var role in userRoles)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, role));
                            }
                        }
                        // Create a security key for signing the token
                        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET");
                        if (string.IsNullOrWhiteSpace(secretKey))
                        {
                            throw new InvalidOperationException("JWT_SECRET environment variable is not set.");
                        }
                        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));// 
                        SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        // Generate JWT token here
                       var jwtToken= new JwtSecurityToken(
                            claims: claims,// token payload
                            expires: DateTime.Now.AddMinutes(30),// token expiration time set to 30 minutes
                            signingCredentials: signingCredentials// signing credentials
                            );
                        // Create the token handler 
                        string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);// token string
                        return Ok(new { token});
                    }
                }
                return Unauthorized("Invalid Email or Password");
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
        [HttpPost("cheangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            try
            {
                var applicaationUser=await _userManager.GetUserAsync(User);
                if (applicaationUser != null)
                {
                    var result=await _userManager.ChangePasswordAsync(applicaationUser, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);
                    if(result.Succeeded)
                    {
                        return NoContent();
                    }
                    else
                    {
                        return BadRequest(result.Errors);
                    }
                }
                return BadRequest(new { message = "invalid data" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
