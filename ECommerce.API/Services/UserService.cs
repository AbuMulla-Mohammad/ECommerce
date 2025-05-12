using ECommerce.API.Data;
using ECommerce.API.Models;
using ECommerce.API.Services.IService;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ECommerce.API.Services
{
    public class UserService : Service<ApplicationUser>, IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(ApplicationDbContext context,UserManager<ApplicationUser>userManager) : base(context)
        {
            this._context = context;
            this._userManager = userManager;
        }
        public async Task<bool> ChangeUserRole(string userId,string newRole)
        {
            var user =await _userManager.FindByIdAsync(userId);
            if(user is not null)
            {
                //remove old roles
                var oldRoles =await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, oldRoles);
                //add new role
                var result = await _userManager.AddToRoleAsync(user, newRole);
                if (result.Succeeded)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            return false;
        }
    }
}
