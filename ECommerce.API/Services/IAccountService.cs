using ECommerce.API.DTOs.Requests;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.API.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(RegisterRequest registerRequest, HttpRequest httpRequest);
        Task<(bool success,string? token,string? errorMessage)> LoginAsync(LoginRequest loginRequest, HttpRequest httpRequest);
        Task<(bool success, IEnumerable<IdentityError> errors, string? ErrorMessage)> ChangePasswordAsync(string userId, ChangePasswordRequest changePasswordRequest);
        Task<(bool Success, IEnumerable<string> Errors, string Message)> ConfirmEmail(string userId,string token);

    }
}
