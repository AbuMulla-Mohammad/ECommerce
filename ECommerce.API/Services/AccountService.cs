using ECommerce.API.DTOs.Requests;
using ECommerce.API.Models;
using ECommerce.API.Utility;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerce.API.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountService(UserManager<ApplicationUser>userManager, IEmailSender emailSender,SignInManager<ApplicationUser>signInManager)
        {
            this._userManager = userManager;
            this._emailSender = emailSender;
            this._signInManager = signInManager;
        }

        public async Task<(bool success, IEnumerable<IdentityError> errors, string? ErrorMessage)> ChangePasswordAsync(string userId, ChangePasswordRequest changePasswordRequest)
        {
            var applicationUser=await _userManager.FindByIdAsync(userId);
            if (applicationUser == null)
            {
                return (false, Enumerable.Empty<IdentityError>(), "User not found.");
            }
            var result = await _userManager.ChangePasswordAsync(applicationUser, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);
            if (result.Succeeded)
            {
                return (true, Enumerable.Empty<IdentityError>(), null);
            }
            else
            {
                return (false, result.Errors, "Failed to change password.");
            }
        }

        public async Task<(bool Success, IEnumerable<string> Errors, string Message)> ConfirmEmail(string userId, string token)
        {
            var applicationUser=await _userManager.FindByIdAsync(userId);
            if (applicationUser == null)
            {
                return (false, new List<string> { "User not found." }, "User not found.");
            }
            var decodedToken = Uri.UnescapeDataString(token);
            var result = await _userManager.ConfirmEmailAsync(applicationUser, decodedToken);
            if(result.Succeeded)
            {
                return (true, Enumerable.Empty<string>(), "Email Confirmed Successfully");
            }
            else
            {
                return (false, result.Errors.Select(e => e.Description), "Failed to confirm email.");
            }
        }

        public async Task<(bool success, string? token, string? errorMessage)> LoginAsync(LoginRequest loginRequest, HttpRequest httpRequest)
        {
            ApplicationUser applicationUser =await _userManager.FindByEmailAsync(loginRequest.Email);
            if (applicationUser == null)
            {
                return (false, null, "Invalid email or password.");
            }
            else
            {
                var result = await _signInManager.PasswordSignInAsync(applicationUser, loginRequest.Password, loginRequest.RememberMe,false);
                if (result.Succeeded)
                {
                    // Create claims for the user
                    List<Claim> claims = new()
                    {
                        new Claim(ClaimTypes.NameIdentifier, applicationUser.Id),
                        new Claim(ClaimTypes.Email, applicationUser.Email),
                        new Claim(ClaimTypes.Name, applicationUser.UserName)
                    };
                    var userRoles=await _userManager.GetRolesAsync(applicationUser);
                    if (userRoles.Count() > 0)
                    {
                        foreach(var role in userRoles)
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
                    SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                    // this key will be used to sign the token and below code will create the signing credentials
                    SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    // Generate JWT token here
                    var jwtToken=new JwtSecurityToken(
                        claims:claims, // token payload
                        expires: DateTime.Now.AddMinutes(30), // token expiration time set to 30 minutes
                        signingCredentials: signingCredentials // signing credentials
                    );
                    // Create the token handler 
                    string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);// token string
                    return (true,token,null);

                }
                else if (result.IsLockedOut)
                {
                    return (false, null, "Your account is locked. Please try again later.");
                }
                else if (result.IsNotAllowed)
                {
                    return (false, null, "You must confirm your email before logging in.");
                }
                return (false, null, "Invalid email or password.");
            }
        }

        public async Task<IdentityResult> RegisterAsync(RegisterRequest registerRequest, HttpRequest httpRequest)
        {
            var applicationUser = registerRequest.Adapt<ApplicationUser>();
            var result=await _userManager.CreateAsync(applicationUser, registerRequest.Password);
            if (!result.Succeeded) return result;
            var confiramtionToken = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
            var encodedToken = Uri.EscapeDataString(confiramtionToken);
            var confirmEmailUrl = $"{httpRequest.Scheme}://{httpRequest.Host}/api/Account/ConfirmEmail?token={encodedToken}&userId={applicationUser.Id}";
            await _emailSender.SendEmailAsync(applicationUser.Email, "Welcome to E-Shopper",
                        $"<h1>Thank you {applicationUser.FirstName} for registering with us.</h1>" +
                        $"<a href='{confirmEmailUrl}'>Confirm your Email</a>");
            await _userManager.AddToRoleAsync(applicationUser, StaticData.Customer);
            return result;
        }

        
    }
}
