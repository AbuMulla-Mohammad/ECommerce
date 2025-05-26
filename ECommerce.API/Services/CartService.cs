using ECommerce.API.Data;
using ECommerce.API.Models;
using ECommerce.API.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Services
{
    public class CartService:Service<Cart>, ICartService
    {
        private readonly ApplicationDbContext _context;
        public CartService(ApplicationDbContext context):base(context)
        {
            this._context = context;
        }

        public async Task<Cart> AddToCartAsync(string UserId, int ProductId, CancellationToken cancellationToken)
        {
            var existingCartItem =await _context.Carts.FirstOrDefaultAsync(cart=> cart.ApplicationUserId== UserId&& cart.ProductId==ProductId);
            if(existingCartItem is not null)
            {
                existingCartItem.Count++;
            }
            else
            {
                existingCartItem = new Cart
                {
                    ProductId = ProductId,
                    ApplicationUserId = UserId,
                    Count = 1
                };
                await _context.Carts.AddAsync(existingCartItem, cancellationToken);
            }
            await _context.SaveChangesAsync(cancellationToken);
            return existingCartItem;
        }
        public async Task<IEnumerable<Cart>> GetCartProducts(string userId, CancellationToken cancellationToken)
        {
            return await GetAsync(expression: (e=>e.ApplicationUserId==userId),includes: [cart=>cart.Product]);
        }
    }
}
