using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(safetys4.Startup))]
namespace safetys4
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
