using ECommerce.API.Data;
using ECommerce.API.Models;
using ECommerce.API.Services.IService;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerce.API.Services
{
    public class BrandService : Service<Brand>, IBrandService
    {
        private readonly ApplicationDbContext _context;

        public BrandService(ApplicationDbContext context):base(context) {
            this._context = context;
        }

        public async Task<bool> EditAsync(int id, Brand brand, CancellationToken cancellationToken = default)
        {
            Brand? brandInDb = _context.Brands.Find(id);
            if (brandInDb == null) return false;
            brandInDb.Name = brand.Name;
            brandInDb.Description = brand.Description;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> UpdateToggleAsync(int id, CancellationToken cancellationToken = default)
        {
            Brand? brandInDb = _context.Brands.Find(id);
            if (brandInDb == null) return false;
            brandInDb.Status = !brandInDb.Status;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
