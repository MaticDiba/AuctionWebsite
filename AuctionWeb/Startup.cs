using Microsoft.Owin;
using Owin;


[assembly: OwinStartupAttribute(typeof(AuctionWeb.App_Start.Startup))]
namespace AuctionWeb.App_Start
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}