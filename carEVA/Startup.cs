using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(carEVA.Startup))]
namespace carEVA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
