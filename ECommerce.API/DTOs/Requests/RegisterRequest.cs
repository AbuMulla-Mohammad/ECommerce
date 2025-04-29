using ECommerce.API.Models;
using System.ComponentModel.DataAnnotations;
using ECommerce.API.Validations;

namespace ECommerce.API.DTOs.Requests
{
    public class RegisterRequest
    {
        [MinLength(2)]
        public string FirstName { get; set; }
        [MinLength(2)]
        public string LastName { get; set; }
        [MinLength(6)]
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public ApplicationUserGender Gender { get; set; }
        [MinimumAge(18)]
        public DateTime DateOfBirth { get; set; }
    }
}
