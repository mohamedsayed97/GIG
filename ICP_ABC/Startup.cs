using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ICP_ABC.Startup))]
namespace ICP_ABC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
