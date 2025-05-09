using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Models
{
    //[PrimaryKey(nameof(ProductId),nameof(ApplicationUserId))]//primary composite key using data anotaion
    public class Cart
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int Count { get; set; }
    }
}
