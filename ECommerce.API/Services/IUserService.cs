using ECommerce.API.Models;
using ECommerce.API.Services.IService;

namespace ECommerce.API.Services
{
    public interface IUserService:IService<ApplicationUser>
    {
        Task<bool> ChangeUserRole(string userId, string newRole);
        Task<bool?>LockUnlock(string userId);
    }
}
