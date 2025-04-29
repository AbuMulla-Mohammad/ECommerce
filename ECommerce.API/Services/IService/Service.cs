using ECommerce.API.Data;
using ECommerce.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ECommerce.API.Services.IService
{
    public class Service<T> : IService<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbset;

        public Service(ApplicationDbContext context){
            this._context = context;
            _dbset = _context.Set<T>();
        }
        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbset.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T?> GetOneAsync(Expression<Func<T, bool>> expression, Expression<Func<T, object>>?[] includes = null, CancellationToken cancellationToken = default, bool isTracked = true)
        {
            var all=await GetAsync(expression, includes, isTracked);
            return all.FirstOrDefault();
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>?[] includes = null,bool isTracked=true)
        {
            IQueryable<T> entitis=_dbset;
            if (expression != null)
            {
                entitis= entitis.Where(expression);
            }
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entitis = entitis.Include(include);
                }
            }
            if (!isTracked)
            {
                entitis = entitis.AsNoTracking();
            }
            return await entitis.ToListAsync();
        }

        public async Task<bool> RemoveAsync(int id, CancellationToken cancellationToken = default)
        {
            T? entityInDb = await _dbset.FindAsync(id);
            if (entityInDb == null) return false;
            _dbset.Remove(entityInDb);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
