using ECommerce.API.Models;
using ECommerce.API.Services.IService;

namespace ECommerce.API.Services
{
    public interface IOrderItemService:IService<OrderItem>
    {
        Task<IEnumerable<OrderItem>>AddRangeAsync(IEnumerable<OrderItem>orderItems, CancellationToken cancellationToken = default);
    }
}
