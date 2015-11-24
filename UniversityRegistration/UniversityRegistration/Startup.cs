using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UniversityRegistration.Startup))]
namespace UniversityRegistration
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
