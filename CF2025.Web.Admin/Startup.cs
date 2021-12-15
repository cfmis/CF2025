using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CF2025.Web.Admin.Startup))]
namespace CF2025.Web.Admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
