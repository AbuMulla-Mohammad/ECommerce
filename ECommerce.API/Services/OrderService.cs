using ECommerce.API.Data;
using ECommerce.API.Models;
using ECommerce.API.Services.IService;

namespace ECommerce.API.Services
{
    public class OrderService : Service<Order>, IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context) : base(context)
        {
            this._context = context;
        }
    }
}
