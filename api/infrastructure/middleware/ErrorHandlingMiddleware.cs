using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using CAS.API.infrastructure.exceptions;

namespace CAS.API.infrastructure.middleware
{
    /// <summary>
    /// ErrorHandlingMiddleware class, provides a way to catch and handle unhandled errors in a generic way.
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        #region Variables

        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly JsonOptions _options;

        #endregion Variables

        #region Constructors

        /// <summary>
        /// Creates a new instance of an ErrorHandlingMiddleware class, and initializes it with the specified arguments.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        public ErrorHandlingMiddleware(RequestDelegate next, IWebHostEnvironment env, ILogger<ErrorHandlingMiddleware> logger, IOptions<JsonOptions> options)
        {
            _next = next;
            _env = env;
            _logger = logger;
            _options = options.Value;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Handle the exception if one occurs. Note this wont catch exceptions created from async void functions.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handle the exception by returning an appropriate error message depending on type and environment.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError;
            var message = "An unhandled error has occurred.";

            switch (ex)
            {
                case SecurityTokenException _:
                    code = HttpStatusCode.Unauthorized;
                    message = "The authentication token is invalid.";
                    break;

                case KeyNotFoundException _:
                    code = HttpStatusCode.BadRequest;
                    message = "Item does not exist.";
                    _logger.LogError(ex, "Middleware caught unhandled exception.");
                    break;

                case NotAuthorizedException _:
                    code = HttpStatusCode.Forbidden;
                    message = "User is not authorized to perform this action.";
                    _logger.LogWarning(ex, ex.Message);
                    break;

                case ConfigurationException _:
                    code = HttpStatusCode.InternalServerError;
                    message = "Application configuration details invalid or missing.";
                    _logger.LogError(ex, ex.Message);
                    break;

                case NotFoundException _:
                    code = HttpStatusCode.NotFound;
                    message = ex.Message;
                    _logger.LogError(ex, ex.Message);
                    break;

                case DbUpdateConcurrencyException _:
                    code = HttpStatusCode.BadRequest;
                    message = "Data may have been modified or deleted since item was loaded.";
                    _logger.LogDebug(ex, ex.Message);
                    break;
      
                case BusinessLayerException _:
                case BadRequestException _:
                case InvalidOperationException _:
                    code = HttpStatusCode.BadRequest;
                    message = ex.Message;
                    _logger.LogDebug(ex, ex.Message);
                    break;

                case JCCommon.Clients.LocationServices.ApiException exception:
                    code = (HttpStatusCode)exception.StatusCode;
                    message = exception.Message;
                    _logger.LogError(ex, ex.Message);
                    break;

                default:
                    _logger.LogError(ex, "Middleware caught unhandled exception.");
                    break;
            }

            if (!context.Response.HasStarted)
            {
                var result = JsonSerializer.Serialize(new Models.ErrorResponseModel(_env, ex, message), _options.JsonSerializerOptions);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)code;
                await context.Response.WriteAsync(result);
            }
            else
            {
                // Had to do this because odd errors were occurring when bearer tokens were failing.
                await context.Response.WriteAsync(string.Empty);
            }
        }

        #endregion Methods
    }
}