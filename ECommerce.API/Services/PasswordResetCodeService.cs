using ECommerce.API.Data;
using ECommerce.API.Models;
using ECommerce.API.Services.IService;

namespace ECommerce.API.Services
{
    public class PasswordResetCodeService : Service<PasswordResetCode>, IPasswordResetCodeService
    {
        private readonly ApplicationDbContext _context;

        public PasswordResetCodeService(ApplicationDbContext context) : base(context)
        {
            this._context = context;
        }
    }
}
