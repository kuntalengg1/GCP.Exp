using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GcpProject1.Startup))]
namespace GcpProject1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
