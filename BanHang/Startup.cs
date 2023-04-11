using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BanHang.Startup))]
namespace BanHang
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
