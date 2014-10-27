using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NetArgot.Startup))]
namespace NetArgot
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
