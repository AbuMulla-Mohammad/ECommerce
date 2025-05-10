using ECommerce.API.Data;
using ECommerce.API.Models;
using ECommerce.API.Services.IService;

namespace ECommerce.API.Services
{
    public class CartService:Service<Cart>, ICartService
    {
        private readonly ApplicationDbContext _context;
        public CartService(ApplicationDbContext context):base(context)
        {
            this._context = context;
        }
    }
}
