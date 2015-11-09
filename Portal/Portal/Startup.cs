using Microsoft.Owin;
using Owin;
using Portal;

[assembly: OwinStartup(typeof (Startup))]

namespace Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}