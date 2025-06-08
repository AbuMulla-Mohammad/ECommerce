using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs.Requests
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }    
        public string Password { get; set; }
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
