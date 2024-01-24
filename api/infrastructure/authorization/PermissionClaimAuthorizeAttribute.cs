using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CAS.API.helpers.extensions;
using CAS.DB.models.auth;

namespace CAS.API.infrastructure.authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionClaimAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _permissions;
        private readonly AuthorizeOperation _authorizationOperation;

        public PermissionClaimAuthorizeAttribute(AuthorizeOperation authorizeOperation = AuthorizeOperation.And, params string[] perm)
        {
            _permissions = perm.Append(Permission.Login).ToArray();
            _authorizationOperation = authorizeOperation;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated ||
                _authorizationOperation == AuthorizeOperation.And && !user.HasPermissions(_permissions) ||
                _authorizationOperation == AuthorizeOperation.Or && !user.HasAPermission(_permissions))
                context.Result = new ForbidResult();
        }
    }

    public enum AuthorizeOperation
    {
        And,
        Or
    }
}
