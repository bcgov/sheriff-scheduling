using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using CAS.API.helpers;
using CAS.API.helpers.extensions;
using CAS.API.infrastructure.exceptions;
using CAS.COMMON.authorization;

namespace CAS.API.infrastructure.authorization
{
    public class DevelopmentEnvironmentClaimsFilter : IAuthorizationFilter
    {
        private IConfiguration Configuration { get; }

        public DevelopmentEnvironmentClaimsFilter(IConfiguration configuration, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
                throw new ConfigurationException("This is not a development environment.");
            Configuration = configuration;
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            var identity = new ClaimsIdentity("Develop");
            context.HttpContext.User = new ClaimsPrincipal(identity);
            var claims = new List<Claim>();
            var roles = Configuration.GetNonEmptyValue("ImpersonateUser:Roles").Split(",").Select(s => s.Trim());
            var permissions = Configuration.GetNonEmptyValue("ImpersonateUser:Permissions").Split(",")
                .Select(s => s.Trim());
            var userId = Configuration.GetNonEmptyValue("ImpersonateUser:UserId");
            var homeLocationId = Configuration.GetNonEmptyValue("ImpersonateUser:HomeLocationId");
            claims.Add(new Claim(CustomClaimTypes.IdirUserName, "test"));
            claims.Add(new Claim(CustomClaimTypes.IdirId, Guid.NewGuid().ToString()));
            claims.AddRange(roles.SelectToList(r => new Claim(ClaimTypes.Role, r)));
            claims.AddRange(permissions.SelectToList(p => new Claim(CustomClaimTypes.Permission, p)));
            claims.Add(new Claim(CustomClaimTypes.UserId, userId));
            claims.Add(new Claim(CustomClaimTypes.HomeLocationId, homeLocationId));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));
            ((ClaimsIdentity) context.HttpContext.User.Identity).AddClaims(claims);

            //Had to put the section below, so when token is called, it returns a value back for the user. 
            var tokens = new List<AuthenticationToken>
            {
                new AuthenticationToken {Name = OpenIdConnectParameterNames.AccessToken, Value = "IMPERSONATED"},
                new AuthenticationToken {Name = "expires_at", Value = DateTime.UtcNow.AddDays(50).ToString()}
            };
            var authProperties = new AuthenticationProperties();
            authProperties.StoreTokens(tokens);

            await context.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                context.HttpContext.User,
                authProperties);
        }
    }
}
