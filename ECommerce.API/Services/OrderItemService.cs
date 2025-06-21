using ECommerce.API.Data;
using ECommerce.API.Models;
using ECommerce.API.Services.IService;

namespace ECommerce.API.Services
{
    public class OrderItemService : Service<OrderItem>, IOrderItemService
    {
        private readonly ApplicationDbContext _context;

        public OrderItemService(ApplicationDbContext context) : base(context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<OrderItem>> AddRangeAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken = default)
        {
            await _context.AddRangeAsync(orderItems, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return orderItems;
        }
    }
}
