using System.Collections.Generic;
using Owin;
using Microsoft.Owin.Security.OpenIdConnect;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.Web.Helpers;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using IdentityModel;
using Goloc.Manager.Web.Helpers;
using Goloc.Model.TokenServer;
using Goloc.Manager.Web.Authorization;
using Microsoft.IdentityModel.Protocols;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(Goloc.Manager.Web.Startup))]

namespace Goloc.Manager.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
            AntiForgeryConfig.UniqueClaimTypeIdentifier = JwtClaimTypes.Subject;

            AntiForgeryConfig.UniqueClaimTypeIdentifier = "unique_user_key";

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseResourceAuthorization(new AuthorizationManager());

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = "golocmvc",
                Authority = TokenServerConstants.IdentityServerAuthority,
                RedirectUri = TokenServerConstants.GolocMvcClientUri,
                SignInAsAuthenticationType = "Cookies",
                ResponseType = "id_token token",
                //ResponseType = "code id_token token",
                Scope = "openid profile UserGroups",

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = async n =>
                    {
                        var userInfo = await EndpointAndTokenHelper.CallUserInfoEndpoint(n.ProtocolMessage.AccessToken);

                        var givenNameClaim = new Claim(
                            JwtClaimTypes.GivenName,
                            userInfo.Value<string>(JwtClaimTypes.GivenName), "");

                        var familyNameClaim = new Claim(
                            JwtClaimTypes.FamilyName,
                            userInfo.Value<string>(JwtClaimTypes.FamilyName), "");                        


                        var newIdentity = new ClaimsIdentity(
                           n.AuthenticationTicket.Identity.AuthenticationType,
                           JwtClaimTypes.GivenName,
                           JwtClaimTypes.Role);

                        var tokens = userInfo.SelectTokens("role");                        

                        foreach (var roleClaims in tokens)
                        {
                            foreach (var roleClaim in roleClaims)
                            {
                                newIdentity.AddClaim(
                                    new Claim(
                                    JwtClaimTypes.Role,
                                    roleClaim.ToString(), "")
                                );
                            }                            
                        }                        
                        
                        newIdentity.AddClaim(givenNameClaim);
                        newIdentity.AddClaim(familyNameClaim);

                        newIdentity.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                        var issuerClaim = n.AuthenticationTicket.Identity
                            .FindFirst(JwtClaimTypes.Issuer);
                        var subjectClaim = n.AuthenticationTicket.Identity
                            .FindFirst(JwtClaimTypes.Subject);

                        newIdentity.AddClaim(subjectClaim);

                        newIdentity.AddClaim(new Claim("unique_user_key",
                            issuerClaim.Value + "_" + subjectClaim.Value));

                        n.AuthenticationTicket = new AuthenticationTicket(
                            newIdentity,
                            n.AuthenticationTicket.Properties);

                    },

                    RedirectToIdentityProvider = n =>
                    {
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                            if (idTokenHint != null)
                            {
                                n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                            }
                        }

                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}