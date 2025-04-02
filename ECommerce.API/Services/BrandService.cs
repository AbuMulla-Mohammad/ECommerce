using ECommerce.API.Data;
using ECommerce.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerce.API.Services
{
    public class BrandService : IBrandService
    {
        private readonly ApplicationDbContext _context;

        public BrandService(ApplicationDbContext context) {
            this._context = context;
        }
        public Brand Add(Brand brand)
        {
            _context.Brands.Add(brand);
            _context.SaveChanges();
            return brand;
        }

        public bool Edit(int id, Brand brand)
        {
            //Brand? BrandInDb=_context.Brands.Find(id); cause track
            Brand? BrandInDb=_context.Brands.AsNoTracking().FirstOrDefault(b => b.Id == id);//no track
            if (BrandInDb == null) return false;
            brand.Id = id;
            _context.Brands.Update(brand);
            _context.SaveChanges();
            return true;

        }

        public Brand? Get(Expression<Func<Brand, bool>> expression)
        {
            return _context.Brands.FirstOrDefault(expression);

        }

        public IEnumerable<Brand> GetAll()
        {
            return _context.Brands.ToList();
        }

        public bool Remove(int id)
        {
            Brand? brandInDb=_context.Brands.Find(id);
            if (brandInDb == null) return false;
            _context.Brands.Remove(brandInDb);
            _context.SaveChanges();
            return true;
        }
    }
}
