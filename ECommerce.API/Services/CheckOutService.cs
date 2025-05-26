using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;

namespace ECommerce.API.Services
{
    public class CheckOutService : ICheckOutService
    {
        private readonly ICartService _cartService;
        private readonly Stripe.Checkout.SessionService _sessionService;

        public CheckOutService(ICartService cartService, Stripe.Checkout.SessionService sessionService)
        {
            this._cartService = cartService;
            this._sessionService = sessionService;
        }
        public async Task<Stripe.Checkout.Session> PayAsync(string userId, string successUrl, string cancelUrl, CancellationToken cancellationToken)
        {
            var cart =await _cartService.GetAsync(expression: (e => e.ApplicationUserId == userId), includes: [e=> e.Product]);
            if (cart is not null)
            {
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl =successUrl,
                    CancelUrl = cancelUrl,
                };
                foreach(var item in cart)
                {
                    options.LineItems.Add(new SessionLineItemOptions
                    {
                        PriceData=new SessionLineItemPriceDataOptions
                        {
                            Currency="USD",
                            ProductData=new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,
                                Description = item.Product.Description,
                            },
                            UnitAmount = (long)item.Product.Price * 100,
                        },
                        Quantity = item.Count,
                    });
                }
                //var service = new Stripe.Checkout.SessionService();
                var session =await _sessionService.CreateAsync(options);
                return session;

            }
            else
            {
                throw new Exception("No products in cart");
            }
        }
        public async Task<Refund> RefundAsync(string sessionId, CancellationToken cancellationToken)
        {
            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = sessionId,
            };
            var refund=await new RefundService().CreateAsync(refundOptions);
            return refund;
        }
    }
}
