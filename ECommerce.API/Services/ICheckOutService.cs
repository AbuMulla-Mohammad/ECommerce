using Stripe;
using Stripe.BillingPortal;

namespace ECommerce.API.Services
{
    public interface ICheckOutService
    {
        Task<Stripe.Checkout.Session>PayAsync(string userId, string successUrl, string cancelUrl, CancellationToken cancellationToken);
        Task<Refund>RefundAsync(string sessionId, CancellationToken cancellationToken);
    }
}
