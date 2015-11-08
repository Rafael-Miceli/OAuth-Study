using Goloc.Model.TokenServer;
using IdentityServer3.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Goloc.IdSrv.Config
{
    public static class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new[]
            {
                new Client
                {
                    Enabled = true,
                    ClientName = "Goloc MVC",
                    ClientId = "golocmvc",
                    Flow = Flows.Implicit,                    
                    RequireConsent = false,

                    RedirectUris = new List<string>
                    {
                        TokenServerConstants.GolocMvcClientUri
                    },                    
                    PostLogoutRedirectUris = new List<string>
                    {
                        TokenServerConstants.GolocMvcClientUri
                    },

                    AllowAccessToAllScopes = true
                }
            };
        }
    }
}