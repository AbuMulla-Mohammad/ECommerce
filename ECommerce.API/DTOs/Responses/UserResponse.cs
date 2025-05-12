using ECommerce.API.Models;

namespace ECommerce.API.DTOs.Responses
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }

        public string Email { get; set; }
        public ApplicationUserGender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
