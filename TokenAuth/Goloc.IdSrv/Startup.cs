using Microsoft.Owin;
using Owin;
using System;
using System.Security.Cryptography.X509Certificates;
using Goloc.IdSrv.Config;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services.Default;

[assembly: OwinStartup(typeof(Goloc.IdSrv.Startup))]

namespace Goloc.IdSrv
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/identity", idsrvApp =>
            {
                var factory = new IdentityServerServiceFactory()
                    .UseInMemoryUsers(Users.Get())
                    .UseInMemoryClients(Clients.Get())
                    .UseInMemoryScopes(Scopes.Get());

                var viewOptions = new DefaultViewServiceOptions();
                viewOptions.Stylesheets.Add("/Content/Site.css");
                viewOptions.CacheViews = false;
                factory.ConfigureDefaultViewService(viewOptions);

                var options = new IdentityServerOptions
                {
                    SiteName = "IdentityServer3 - Configuring DefaultViewService",

                    SigningCertificate = LoadCertificate(),
                    Factory = factory,

                    //AuthenticationOptions = new AuthenticationOptions
                    //{
                    //    IdentityProviders = ConfigureAdditionalIdentityProviders,
                    //}
                };

                idsrvApp.UseIdentityServer(options);             
                
            });
        }

        X509Certificate2 LoadCertificate()
        {

            string file = string.Format(@"{0}\Certs\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory);

            return new X509Certificate2(file, "idsrv3test");
        }
    }
}