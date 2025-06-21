using ECommerce.API.DTOs.Requests;
using ECommerce.API.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;

namespace ECommerce.API.Services
{
    public class CheckOutService : ICheckOutService
    {
        private readonly ICartService _cartService;
        private readonly Stripe.Checkout.SessionService _sessionService;
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;
        private readonly IEmailSender _emailSender;

        public CheckOutService(ICartService cartService, Stripe.Checkout.SessionService sessionService,IOrderService orderService,IOrderItemService orderItemService, IEmailSender emailSender)
        {
            this._cartService = cartService;
            this._sessionService = sessionService;
            this._orderService = orderService;
            this._orderItemService = orderItemService;
            this._emailSender = emailSender;
        }
        public async Task<(Stripe.Checkout.Session?, int orderId)> PayAsync(string userId, string successUrl, string cancelUrl, PaymentRequest paymentRequest, CancellationToken cancellationToken)
        {
            var cart =await _cartService.GetAsync(expression: (e => e.ApplicationUserId == userId), includes: [e=> e.Product]);
            if (cart is not null)
            {
                Order order = new()
                {
                    OrderStatus = OrderStatus.Pending,
                    OrderDate = DateTime.Now,
                    TotalPrice = cart.Sum(item => item.Product.Price * item.Count),
                    ApplicationUserId = userId,

                };
                if (paymentRequest.PaymentMethod == "Cash")
                {
                    order.PaymentMethod = PaymentMethodType.Cash;
                    await _orderService.AddAsync(order, cancellationToken);
                    return (null,order.Id);
                }
                else if (paymentRequest.PaymentMethod == "Visa") {
                    order.PaymentMethod = PaymentMethodType.Visa;
                    await _orderService.AddAsync(order, cancellationToken);
                    var options = new Stripe.Checkout.SessionCreateOptions
                    {
                        PaymentMethodTypes = new List<string> { "card" },
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment",
                        SuccessUrl = successUrl+ $"/{order.Id}",
                        CancelUrl = cancelUrl,
                    };
                    foreach (var item in cart)
                    {
                        if (item.Product.Quantity < item.Count)
                        {
                            throw new Exception($"Product {item.Product.Name} is out of stock");
                        }
                        options.LineItems.Add(new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "USD",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
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
                    var session = await _sessionService.CreateAsync(options);
                    order.SessionId = session.Id;

                    await _orderService.CommitAsync(cancellationToken);
                    return (session,order.Id);
                }
                else
                {
                    throw new Exception("Invalid payment method");
                }
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

        public async Task<(bool isSuccess, string? message)> SuccessAsync(int orderId, CancellationToken cancellationToken)
        {
            var order = await _orderService.GetOneAsync(
                expression: e => e.Id == orderId,
                includes: [e => e.ApplicationUser],
                cancellationToken);

            if (order is null)
                return (false, "Order not found");

            var applicationUser = order.ApplicationUser;
            if (applicationUser == null)
                return (false, "User information missing from order");

            var subject = "";
            var body = "";

            var cartItems = await _cartService.GetAsync(
                expression: e => e.ApplicationUserId == applicationUser.Id,
                includes: [e => e.Product]);

            if (cartItems == null || !cartItems.Any())
                return (false, "No items found in cart");

            List<OrderItem> orderItems = new();
            foreach (var item in cartItems)
            {
                orderItems.Add(new OrderItem
                {
                    OrderId = orderId,
                    ProductId = item.ProductId,
                    TotalPrice = item.Product.Price * item.Count,
                });

                item.Product.Quantity -= item.Count;
            }

            await _orderItemService.AddRangeAsync(orderItems, cancellationToken);
            await _cartService.RemoveRangeAsync(cartItems, cancellationToken);

            if (order.PaymentMethod == PaymentMethodType.Cash)
            {
                subject = "Order Received - E-Shopper - Cash Payment";
                body = $"<h1>Thank you {applicationUser.FirstName} for your order.</h1>" +
                       $"<p>Your order with order Id: {orderId} will be processed and shipped soon.</p>";
            }
            else
            {
                order.OrderStatus = OrderStatus.Approved;

                var service = new Stripe.Checkout.SessionService();
                var session = await service.GetAsync(order.SessionId);
                order.TransactionId = session.PaymentIntentId;

                await _orderService.CommitAsync(cancellationToken);

                subject = "Order Payment Success";
                body = $"<h1>Thank you {applicationUser.FirstName} for your payment.</h1>" +
                       $"<p>Your order with order Id: {orderId} is being processed and will be shipped soon.</p>";
            }

            await _emailSender.SendEmailAsync(applicationUser.Email, subject, body);
            return (true, "Order processed successfully");
        }

    }
}
