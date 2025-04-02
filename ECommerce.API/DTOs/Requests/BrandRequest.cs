using ECommerce.API.Models;

namespace ECommerce.API.DTOs.Requests
{
    public class BrandRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
    }
}
