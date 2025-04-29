using ECommerce.API.Data;
using ECommerce.API.DTOs.Requests;
using ECommerce.API.Models;
using ECommerce.API.Services.IService;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ECommerce.API.Services
{
    public class ProductService : Service<Product>, IProductService
    {
        private readonly ApplicationDbContext _context;
        public ProductService(ApplicationDbContext context):base(context)
        {
            this._context = context;
        }

        public async Task<Product?> AddProductAsync(ProductRequest productRequest, CancellationToken cancellationToken = default)
        {
            var file = productRequest.MainImage;
            var product = productRequest.Adapt<Product>();
            if (file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", fileName);
                using (var stream = File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
                product.MainImage = fileName;
                await _context.Products.AddAsync(product, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return product;
            }
            return null;
        }
                
        public async Task<bool> EditAsync(int id,UpdateProductRequest productRequest, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Product? productIndDb=await GetOneAsync(p => p.Id == id,isTracked:false);
            var existingProduct = productRequest.Adapt<Product>();
            var file = productRequest.MainImage;
            if (productIndDb != null)
            {
                if(file!=null&& file.Length > 0)
                {
                    var newFileName=Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
                    var newFilePath=Path.Combine(Directory.GetCurrentDirectory(), "Images", newFileName);
                    using(var stream=File.Create(newFilePath))
                    {
                        await file.CopyToAsync(stream,cancellationToken);
                    }
                    var oldFilePath= Path.Combine(Directory.GetCurrentDirectory(), "Images", productIndDb.MainImage);
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath); 
                    }
                    existingProduct.MainImage = newFileName;
                }
                else
                {
                    existingProduct.MainImage=productIndDb.MainImage; // Set the existing image if no new image is provided
                }
                existingProduct.Id = id;
                //here below i'm making updates in the product that adapted from productRequest which it dosent
                //have an image so it will update the image that is already in the database into null so i have to add its
                //value from the productInDb(the one that is already in the database) as i did above
                // existingProduct.MainImage = productInDb.MainImage;
                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }
        public IEnumerable<Product> GetAll(string? serachQuery, int page, int limit = 10)
        {
            IQueryable<Product> products = _context.Products;
            if (serachQuery != null)
            {
                products = products.Where(p => p.Name.ToLower().Contains(serachQuery.ToLower())||p.Description.ToLower().Contains(serachQuery.ToLower()));
            }
            if(page <= 0) page = 1;
            if (limit <= 0) limit = 10;
            products = products.Skip((page-1)*limit).Take(limit);
            return products;
        }
        public async Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken)
        {
            Product? productIndDb = await _context.Products.FindAsync(id);
            if (productIndDb != null)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", productIndDb.MainImage);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                _context.Products.Remove(productIndDb);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }
    }
}
