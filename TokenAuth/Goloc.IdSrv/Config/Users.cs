using IdentityServer3.Core;
using IdentityServer3.Core.Services.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Goloc.IdSrv.Config
{
    public static class Users
    {
        public static List<InMemoryUser> Get()
        {
            return new List<InMemoryUser>()
            {

                new InMemoryUser
                {
                    Username = "rafael.miceli",
                    Password = "12345678",
                    Subject = "1",

                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.GivenName, "Rafael"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Miceli"),
                        new Claim(Constants.ClaimTypes.Role, "GestorGeral"),
                        new Claim(Constants.ClaimTypes.Role, "Usuario")
                   }
                }
                ,
                new InMemoryUser
                {
                    Username = "Sven",
                    Password = "secret",
                    Subject = "2",

                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.GivenName, "Sven"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Vercauteren"),
                   }
                },

                new InMemoryUser
                {
                    Username = "Nils",
                    Password = "secret",
                    Subject = "3",

                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.GivenName, "Nils"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Missorten"),
                   }
                }

           };
        }
    }
}