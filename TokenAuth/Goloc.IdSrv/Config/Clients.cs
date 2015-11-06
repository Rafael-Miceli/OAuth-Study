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
                    ClientId = "goloc-mvc",
                    Flow = Flows.Hybrid,
                    RequireConsent = false,

                    RedirectUris = new List<string>
                    {
                        "https://localhost:44306/"
                    }
                }
            };
        }
    }
}