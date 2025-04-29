using ECommerce.API.Data;
using ECommerce.API.DTOs.Requests;
using ECommerce.API.Models;
using ECommerce.API.Services.IService;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ECommerce.API.Services
{
    public class CategoryService : Service<Category>,ICategoryService
    {
        private readonly ApplicationDbContext _context;
        public CategoryService(ApplicationDbContext context) :base(context)
        {
            this._context = context;
        }
        public async Task<bool> EditAsync(int id, Category category,CancellationToken cancellationToken=default)
        {
            //Category? categoryInDb=context.Categories.Find(id);//track
            Category? categoryInDb = _context.Categories.Find(id);
            if (categoryInDb == null) return false;
            categoryInDb.Name= category.Name;
            categoryInDb.Description= category.Description;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<bool> UpdateToggleAsync(int id,CancellationToken cancellationToken= default)
        {
            Category? cateogryInDb = _context.Categories.Find(id);
            if (cateogryInDb == null) return false;
            cateogryInDb.Status = !cateogryInDb.Status;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
            
        }
    }
}
