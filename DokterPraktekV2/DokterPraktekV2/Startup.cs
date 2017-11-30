using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DokterPraktekV2.Startup))]
namespace DokterPraktekV2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
