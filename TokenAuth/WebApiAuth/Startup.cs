using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.OpenIdConnect;
using WebApiAuth.Models;
using System.IdentityModel.Tokens;
//using Thinktecture.IdentityModel.Client;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.Web.Helpers;

[assembly: OwinStartup(typeof(WebApiAuth.Startup))]

namespace WebApiAuth
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);

            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
            AntiForgeryConfig.UniqueClaimTypeIdentifier = "unique_user_key";

            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = "mvc",
                Authority = "https://golocidsrv.azurewebsites.net/identity",
                RedirectUri = "https://localhost:44306/",
                SignInAsAuthenticationType = "Cookies",
                ResponseType = "code id_token token",
                Scope = "openid profile",

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    MessageReceived = async n =>
                    {
                        //EndpointAndTokenHelper.DecodeAndWrite(n.ProtocolMessage.IdToken);
                        //EndpointAndTokenHelper.DecodeAndWrite(n.ProtocolMessage.AccessToken);

                        //var userInfo = await EndpointAndTokenHelper.CallUserInfoEndpoint(n.ProtocolMessage.AccessToken);
                    },

                    SecurityTokenValidated = async n =>
                    {
                        var userInfo = await EndpointAndTokenHelper.CallUserInfoEndpoint(n.ProtocolMessage.AccessToken);

                        var givenNameClaim = new Claim(
                            Thinktecture.IdentityModel.Client.JwtClaimTypes.GivenName,
                            userInfo.Value<string>("given_name"), "");

                        var familyNameClaim = new Claim(
                            Thinktecture.IdentityModel.Client.JwtClaimTypes.FamilyName,
                            userInfo.Value<string>("family_name"), "");

                        var newIdentity = new ClaimsIdentity(
                           n.AuthenticationTicket.Identity.AuthenticationType,
                           Thinktecture.IdentityModel.Client.JwtClaimTypes.GivenName,
                           Thinktecture.IdentityModel.Client.JwtClaimTypes.Role);

                        newIdentity.AddClaim(givenNameClaim);
                        newIdentity.AddClaim(familyNameClaim);

                        var issuerClaim = n.AuthenticationTicket.Identity
                            .FindFirst(Thinktecture.IdentityModel.Client.JwtClaimTypes.Issuer);
                        var subjectClaim = n.AuthenticationTicket.Identity
                            .FindFirst(Thinktecture.IdentityModel.Client.JwtClaimTypes.Subject);

                        newIdentity.AddClaim(new Claim("unique_user_key",
                            issuerClaim.Value + "_" + subjectClaim.Value));

                        n.AuthenticationTicket = new AuthenticationTicket(
                            newIdentity,
                            n.AuthenticationTicket.Properties);
                    }
                }
            });
        }
    }
}
