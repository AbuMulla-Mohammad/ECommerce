using ECommerce.API.Data;
using ECommerce.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerce.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext context;
        public CategoryService(ApplicationDbContext context) 
        {
            this.context = context;
        }
        public Category Add(Category category)
        {
            context.Categories.Add(category);
            context.SaveChanges();
            return category;
        }

        public bool Edit(int id, Category category)
        {
            //Category? categoryInDb=context.Categories.Find(id);//track
            Category? categoryInDb = context.Categories.AsNoTracking().FirstOrDefault(c => c.Id == id);//no track
            if (categoryInDb == null) return false;
            category.Id = id;
            context.Categories.Update(category);
            context.SaveChanges();
            return true;
        }

        public Category? Get(Expression<Func<Category, bool>> expression)
        {
            return context.Categories.FirstOrDefault(expression);
        }

        public IEnumerable<Category> GetAll()
        {
            return context.Categories.ToList();
        }

        public bool Remove(int id)
        {
            Category? categoryInDb = context.Categories.Find(id);
            if (categoryInDb == null) return false;
            context.Categories.Remove(categoryInDb);
            context.SaveChanges();
            return true;
        }
    }
}
