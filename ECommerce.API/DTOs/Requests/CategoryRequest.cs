using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs.Requests
{
    public class CategoryRequest
    {
        [Required(ErrorMessage = "Name is Required")]
        [MinLength(3,ErrorMessage ="Name min length is 3 chars")]
        [MaxLength(50, ErrorMessage = "Name max length is 50 chars")]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
    }
}
