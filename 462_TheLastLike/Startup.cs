using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(_462_TheLastLike.Startup))]
namespace _462_TheLastLike
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
