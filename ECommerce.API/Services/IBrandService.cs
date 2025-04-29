using ECommerce.API.Models;
using ECommerce.API.Services.IService;
using System.Linq.Expressions;

namespace ECommerce.API.Services
{
    public interface IBrandService:IService<Brand>
    {
        Task<bool> EditAsync(int id, Brand brand, CancellationToken cancellationToken = default);
        Task<bool> UpdateToggleAsync(int id, CancellationToken cancellationToken = default);
    }
}
