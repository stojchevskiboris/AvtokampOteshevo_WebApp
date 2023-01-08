using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Project_IT.Startup))]
namespace Project_IT
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
