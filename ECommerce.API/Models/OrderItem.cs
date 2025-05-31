namespace ECommerce.API.Models
{
    public class OrderItem
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } 
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string? Note { get; set; }
        public decimal TotalPrice { get; set; }


    }
}
