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
            var product=await _context.Products.FindAsync(ProductId, cancellationToken);
            if (product == null)
                throw new Exception("Product not found.");

            if (product.Quantity <= 0)
                throw new Exception("Product is out of stock.");
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
        public async Task<bool> RemoveRangeAsync(IEnumerable<Cart>items, CancellationToken cancellationToken = default)
        {
           _context.RemoveRange(items);
            await _context.SaveChangesAsync(cancellationToken);
            return true;

        }
    }
}
