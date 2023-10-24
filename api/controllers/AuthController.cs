using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using CAS.API.helpers;
using CAS.API.helpers.extensions;
using CAS.API.services;
using CAS.COMMON.authorization;
using CAS.DB.models;

namespace CAS.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private bool IsImpersonated { get; }
        private double OvertimeHoursPerDay { get; }
        private ChesEmailService ChesEmailService { get; }
        private IConfiguration Configuration { get; }
        private CourtAdminDbContext Db { get; }
        public AuthController(IWebHostEnvironment env, IConfiguration configuration, ChesEmailService chesEmailService, CourtAdminDbContext db)
        {
            Configuration = configuration;
            ChesEmailService = chesEmailService;
            IsImpersonated = env.IsDevelopment() &&
                              configuration.GetNonEmptyValue("ByPassAuthAndUseImpersonatedUser").Equals("true");
            OvertimeHoursPerDay = double.Parse(configuration.GetNonEmptyValue("OverTimeHoursPerDay"));
            Db = db;
        }
        /// <summary>
        /// This cannot be called from AJAX or SWAGGER. It must be loaded in the browser location, because it brings the user to the SSO page. 
        /// </summary>
        /// <param name="redirectUri">URL to go back to.</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
        [HttpGet("login")]
        public async Task<IActionResult> Login(string redirectUri = "/api")
        {
            if (IsImpersonated)
                return Redirect(redirectUri);

            //This was moved from claims, because it is only hit once (versus multiple times for GenerateClaims).
            var idirId = User.Claims.GetIdirId();
            var idirName = User.Claims.GetIdirUserName();
            var user = await Db.User.FirstOrDefaultAsync(u => u.IdirId == idirId || !u.IdirId.HasValue && u.IdirName == idirName);
            if (user == null) 
                return Redirect(redirectUri);
            
            user.IdirId ??= idirId;
            user.KeyCloakId = User.Claims.GetKeyCloakId();
            user.LastLogin = DateTimeOffset.UtcNow;
            await Db.SaveChangesAsync();
            return Redirect(redirectUri);
        }

        /// <summary>
        /// Logout function, should wipe out all cookies. 
        /// </summary>
        /// <returns></returns>
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            var logoutUrl = $"{Configuration.GetNonEmptyValue("Keycloak:Authority")}/protocol/openid-connect/logout";

            var forwardedHost = HttpContext.Request.Headers.ContainsKey("X-Forwarded-Host")
                ? HttpContext.Request.Headers["X-Forwarded-Host"].ToString()
                : Request.Host.ToString();
            var forwardedPort = HttpContext.Request.Headers["X-Forwarded-Port"];

            //We are always sending X-Forwarded-Port, only time we aren't is when we are hitting the API directly. 
            var baseUri = HttpContext.Request.Headers.ContainsKey("X-Forwarded-Host") ? $"{Configuration.GetNonEmptyValue("WebBaseHref")}logout" : "/api";

            var applicationUrl = $"{XForwardedForHelper.BuildUrlString(forwardedHost, forwardedPort, baseUri)}";
            var keycloakLogoutUrl = $"{logoutUrl}?redirect_uri={applicationUrl}";
            var siteminderLogoutUrl = $"{Configuration.GetNonEmptyValue("SiteMinderLogoutUrl")}?returl={keycloakLogoutUrl}&retnow=1";
            return Redirect(keycloakLogoutUrl);
        }

        [HttpGet("requestAccess")]
        [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RequestAccess(string currentEmailAddress)
        {
            var emailString =
                $"{User.FullName()} - {User.IdirUserName()} - {currentEmailAddress} - Has requested access to CourtAdmin Scheduling on {DateTime.Now}.";

            var requestAccessEmailAddress = Configuration.GetNonEmptyValue("RequestAccessEmailAddress");

            await ChesEmailService.SendEmail(emailString,
                "Access Request", requestAccessEmailAddress);
            return NoContent();
        }

        /// <summary>
        /// Should be logged in to call this. 
        /// </summary>
        /// <returns>access_token and refresh_token for API calls.</returns>
        [HttpGet("token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetToken()
        {
            var accessToken = await HttpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "access_token");
            var expiresAt = await HttpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "expires_at");
            return Ok(new { access_token = accessToken, expires_at = expiresAt });
        }

        /// <summary>
        /// Provides Permissions, Roles, UserId, HomeLocationId via from claims to JSON.
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("info")]
        public ActionResult UserInfo()
        {
            var roles = HttpContext.User.FindAll(ClaimTypes.Role).SelectToList(r => r.Value);
            var permissions = HttpContext.User.FindAll(CustomClaimTypes.Permission).SelectToList(r => r.Value);
            var userId = HttpContext.User.FindFirst(CustomClaimTypes.UserId)?.Value;
            var homeLocationId = HttpContext.User.FindFirst(CustomClaimTypes.HomeLocationId)?.Value;
            var firstName = HttpContext.User.FindFirst(CustomClaimTypes.FirstName)?.Value;
            var lastName = HttpContext.User.FindFirst(CustomClaimTypes.LastName)?.Value;

            var viewShiftRestrictionDays = Configuration.GetNonEmptyValue("ViewShiftRestrictionDays");
            var viewDutyRosterRestrictionHours = Configuration.GetNonEmptyValue("ViewDutyRosterRestrictionHours");
            return Ok(new
            {
                Permissions = permissions,
                Roles = roles,
                UserId = userId,
                FirstName = firstName,
                LastName = lastName,
                HomeLocationId = homeLocationId,
                IsImpersonated = IsImpersonated,
                OverTimeHoursPerDay = OvertimeHoursPerDay,
                ViewShiftRestrictionDays = viewShiftRestrictionDays,
                ViewDutyRosterRestrictionHours = viewDutyRosterRestrictionHours,
                DateTime.UtcNow
            });
        }
    }
}
    