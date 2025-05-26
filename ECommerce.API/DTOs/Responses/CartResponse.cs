namespace ECommerce.API.DTOs.Responses
{
    public class CartResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string MainImage { get; set; }
    }
}
