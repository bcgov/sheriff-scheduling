using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using CAS.API.helpers.extensions;
using CAS.COMMON.authorization;
using CAS.DB.models.auth;
using Xunit;

namespace tests.api.Helpers
{
    public class HttpResponseTest
    {
        public static T CheckForValid200HttpResponseAndReturnValue<T>(ActionResult<T> actionResult)
        {
            Assert.NotNull(actionResult);
            Assert.NotNull(actionResult.Result);
            var okObjectResult = actionResult.Result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            var result = (T)okObjectResult.Value;
            Assert.NotNull(result);
            return result;
        }

        public static void CheckForNoContentResponse<T>(ActionResult<T> actionResult)
        {
            Assert.NotNull(actionResult);
            Assert.NotNull(actionResult.Result);
            var noContentResult = actionResult.Result as NoContentResult;
        }

        public static void CheckForNoContentResponse(ActionResult actionResult)
        {
            Assert.NotNull(actionResult);
            var noContentResult = actionResult as NoContentResult;
            Assert.NotNull(noContentResult);
        }

        public static void CheckForForbid<T>(ActionResult<T> actionResult)
        {
            Assert.NotNull(actionResult);
            Assert.NotNull(actionResult.Result);
            var forbid = actionResult.Result as ForbidResult;
            Assert.NotNull(forbid);
        }

        public static void CheckForNotFound<T>(ActionResult<T> actionResult)
        {
            Assert.NotNull(actionResult);
            Assert.NotNull(actionResult.Result);
            var notFound = actionResult.Result as NotFoundResult;
            Assert.NotNull(notFound);
        }

        public static HttpContext SetupHttpContext()
        {
            var headerDictionary = new HeaderDictionary();
            var response = new Mock<HttpResponse>();
            response.SetupGet(r => r.Headers).Returns(headerDictionary);

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(a => a.Response).Returns(response.Object);

            var identity = new ClaimsIdentity("Develop");
            var user = new ClaimsPrincipal(identity);

            //Load in all permissions.
            var claims = new List<Claim>();
            var permissions = typeof(Permission).GetFields(BindingFlags.Public | BindingFlags.Static |
                                                           BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly).Select(p => p.Name);
            claims.AddRange(permissions.SelectToList(p => new Claim(CustomClaimTypes.Permission, p)));
            claims.Add(new Claim(CustomClaimTypes.UserId, User.SystemUser.ToString()));
            ((ClaimsIdentity)user.Identity)
                .AddClaims(claims);

            httpContext.SetupGet(a => a.User).Returns(user);

            return httpContext.Object;
        }

        public static ControllerContext SetupMockControllerContext()
        {
            return new ControllerContext
            {
                HttpContext = SetupHttpContext()
            };
        }
    }
}