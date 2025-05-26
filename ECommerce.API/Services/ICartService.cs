using ECommerce.API.Models;
using ECommerce.API.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Services
{
    public interface ICartService:IService<Cart>
    {
        Task<Cart> AddToCartAsync(string userId, int ProductId,CancellationToken cancellationToken);
        Task<IEnumerable<Cart>>GetCartProducts(string userId, CancellationToken cancellationToken);
    }
}
