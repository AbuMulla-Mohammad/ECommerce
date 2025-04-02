using ECommerce.API.Models;

namespace ECommerce.API.DTOs.Responses
{
    public class BrandResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
    }
}
