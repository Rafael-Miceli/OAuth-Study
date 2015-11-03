using Microsoft.Owin;
using Owin;
using Thinktecture.IdentityServer.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using WebAppIdSrv.Config;

[assembly: OwinStartup(typeof(WebAppIdSrv.Startup))]

namespace WebAppIdSrv
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/identity", idsrvApp =>
            {
                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "Embedded IdentityServer",
                    //IssuerUri = "Identinty Uri",
                    SigningCertificate = LoadCertificate(),

                    Factory = InMemoryFactory.Create(
                        users: Users.Get(),
                        clients: Clients.Get(),
                        scopes: Scopes.Get())                    
                });
            });
        }

        X509Certificate2 LoadCertificate()
        {

            string file = string.Format(@"{0}\Certs\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory);

            return new X509Certificate2(file, "idsrv3test");
        }
    }
}