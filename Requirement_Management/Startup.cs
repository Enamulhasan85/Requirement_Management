using Microsoft.Owin;
using Owin;
using Requirement_Management.DataAccess;
using Requirement_Management.Models;
using System.Linq;

[assembly: OwinStartupAttribute(typeof(Requirement_Management.Startup))]
namespace Requirement_Management
{
    public partial class Startup
    {
        private RequirementManagementContext dbcontext = new RequirementManagementContext();

        public void Configuration(IAppBuilder app)
        {
            var role1 = dbcontext.Role.FirstOrDefault(r => r.RoleName == "systemadmin");
            if (role1 == null)
            {
                Role role = new Role()
                {
                    RoleName = "systemadmin"
                };
                dbcontext.Role.Add(role);
                dbcontext.SaveChanges();
            }

            var role2 = dbcontext.Role.FirstOrDefault(r => r.RoleName == "admin");
            if (role2 == null)
            {
                Role role = new Role()
                {
                    RoleName = "admin"
                };
                dbcontext.Role.Add(role);
                dbcontext.SaveChanges();
            }

            var role3 = dbcontext.Role.FirstOrDefault(r => r.RoleName == "normal user");
            if (role3 == null)
            {
                Role role = new Role()
                {
                    RoleName = "normal user"
                };
                dbcontext.Role.Add(role);
                dbcontext.SaveChanges();
            }

            var user = dbcontext.User.FirstOrDefault(r => r.Username == "systemadmin");
            if (user == null)
            {
                var user1 = new User() {
                    Username = "systemadmin",
                    FirstName = "Enamul",
                    LastName = "Hasan",
                    Email = "syl.enamul@gmail.com",
                    Password = "123",
                    IsActive = true,
                    Roles = dbcontext.Role.Where(r => r.RoleName == "systemadmin").ToList()
                };
                dbcontext.User.Add(user1);
                dbcontext.SaveChanges();
            }

            ConfigureAuth(app);
        }
    }
}
