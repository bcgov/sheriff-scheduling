using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using CAS.API.helpers;
using CAS.COMMON.authorization;

namespace CAS.API.infrastructure.authorization
{
    public class ClaimsTransformer : IClaimsTransformation
    {
        private IMemoryCache Cache { get; }
        private ClaimsService ClaimsService { get; }
        private TimeSpan ClaimCachePeriod { get; }
        private bool _isTransformed;

        public ClaimsTransformer(IMemoryCache cache, ClaimsService claimsService, IConfiguration configuration)
        {
            Cache = cache;
            ClaimsService = claimsService;
            ClaimCachePeriod = TimeSpan.Parse(configuration.GetNonEmptyValue("ClaimsCachePeriod"));
        }

        /// <summary>
        /// Note these claims don't get saved in the cookie.
        /// </summary>
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated || _isTransformed) return await Task.FromResult(principal);
            var currentPrincipal = (ClaimsIdentity)principal.Identity;
            var currentClaims = currentPrincipal.Claims.ToList();
                        
            var nameIdentifier = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier).Replace("@idir", ""));
            if (!Cache.TryGetValue(nameIdentifier, out List<Claim> claims))
            {
                claims = await ClaimsService.GenerateClaims(currentClaims);
                Cache.Set(nameIdentifier, claims, DateTimeOffset.UtcNow.Add(ClaimCachePeriod));
            }
            currentPrincipal.AddClaims(claims);
            _isTransformed = true;
            return await Task.FromResult(principal);
        }

        /// <summary>
        /// This is so we can filter out the keycloak claims, as they're saved in the cookies and can be quite long.
        /// </summary>
        public static List<string> UsedProviderClaimTypes =>
            new List<string> { ClaimTypes.NameIdentifier, CustomClaimTypes.IdirUserName, CustomClaimTypes.FullName, CustomClaimTypes.IdirId };
    }
}