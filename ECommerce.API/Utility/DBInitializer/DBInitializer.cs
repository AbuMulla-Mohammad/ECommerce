using ECommerce.API.Data;
using ECommerce.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace ECommerce.API.Utility.DBInitializer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DBInitializer(ApplicationDbContext dbContext,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser>userManager)
        {
            this._dbContext = dbContext;
            this._roleManager = roleManager;
            this._userManager = userManager;
        }
        public async Task Initialize()
        {
            try
            {
                if (_dbContext.Database.GetPendingMigrations().Any()) _dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                throw new Exception("Database initialization failed", ex);
            }
            if (_roleManager.Roles.IsNullOrEmpty())//on first register of my application
            {
                await _roleManager.CreateAsync(new(StaticData.SuperAdmin));
                await _roleManager.CreateAsync(new(StaticData.Admin));
                await _roleManager.CreateAsync(new(StaticData.Customer));
                await _roleManager.CreateAsync(new(StaticData.Company));
            }
            //create the super admin user
            await _userManager.CreateAsync(new()
            {
                FirstName = "Super",
                LastName = "Admin",
                UserName = "Super_Admin",
                Gender=ApplicationUserGender.Male,
                DateOfBirth =new DateTime(1999,1,1),
                Email="admin@Eshopper.com",

            }, "SuperAdmin@1");
            var user = await _userManager.FindByEmailAsync("admin@Eshopper.com");
            await _userManager.AddToRoleAsync(user, "SuperAdmin");
        }
    }
}
