using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Requirement_Management.Startup))]
namespace Requirement_Management
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
