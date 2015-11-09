using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

namespace Goloc.Manager.Web.Authorization
{
    public class AuthorizationManager : ResourceAuthorizationManager
    {
        public override Task<bool> CheckAccessAsync(ResourceAuthorizationContext context)
        {
            switch (context.Resource.First().Value)
            {
                case "UserGroups":
                    return AuthorizeUserGroups(context);
                default:
                    return Nok();
            }
        }

        private Task<bool> AuthorizeUserGroups(ResourceAuthorizationContext context)
        {
            switch (context.Action.First().Value)
            {
                case "Read":
                    return Eval(context.Principal.HasClaim("role", "GestorGeral"));
                case "Write":
                    return Eval(context.Principal.HasClaim("role", "GestorGeral"));
                default:
                    return Nok();
            }
        }
    }
}