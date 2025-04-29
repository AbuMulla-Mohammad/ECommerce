using ECommerce.API.DTOs.Requests;
using ECommerce.API.Models;
using ECommerce.API.Services.IService;
using System.Linq.Expressions;

namespace ECommerce.API.Services
{
    public interface IProductService:IService<Product>
    {
        IEnumerable<Product> GetAll(string? serachQuery, int page,int limit=10);
        Task<Product?> AddProductAsync(ProductRequest productRequest , CancellationToken cancellationToken = default);
        Task<bool> EditAsync(int id, UpdateProductRequest productRequest, CancellationToken cancellationToken);
        Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken);

    }
}
