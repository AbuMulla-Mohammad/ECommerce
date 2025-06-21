using ECommerce.API.DTOs.Requests;
using Stripe;
using Stripe.BillingPortal;

namespace ECommerce.API.Services
{
    public interface ICheckOutService
    {
        Task<(Stripe.Checkout.Session?,int orderId)>PayAsync(string userId, string successUrl, string cancelUrl,PaymentRequest paymentRequest, CancellationToken cancellationToken);
        Task<Refund>RefundAsync(string sessionId, CancellationToken cancellationToken);
        Task<(bool isSuccess,string? message)> SuccessAsync(int orderId,CancellationToken cancellationToken);
    }
}
