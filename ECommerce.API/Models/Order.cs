namespace ECommerce.API.Models
{
    public enum OrderStatus{
        Pending,
        Approved,
        Shipped,
        Completed,
        Cancelled,
    }
    public enum PaymentMethodType
    {
        Cash,
        Visa,
    }
public class Order
    {
        //order
        public int Id { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public decimal TotalPrice { get; set; }
        //payment
        public string? SessionId { get; set; }
        public string? TransactionId { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        //carrier
        public string? Carrier { get; set; }
        public string? TrackingNumber { get; set; }
        //relation 
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public string? ApplicationUserId { get; set; }
    }
}
