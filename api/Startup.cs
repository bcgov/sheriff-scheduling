using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using CAS.API.helpers;
using CAS.API.infrastructure;
using CAS.API.infrastructure.authorization;
using CAS.API.infrastructure.encryption;
using CAS.API.infrastructure.middleware;
using CAS.API.services.ef;
using CAS.DB.models;
using Microsoft.Extensions.Logging;
using Quartz;
using CAS.API.cronjobs;

namespace CAS.API
{
    public class Startup
    {
        private IWebHostEnvironment CurrentEnvironment { get; }
        private bool DevelopmentMode { get; }
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
            DevelopmentMode = CurrentEnvironment.IsDevelopment() &&
                              Configuration.GetNonEmptyValue("ByPassAuthAndUseImpersonatedUser").Equals("true");
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(options =>
            {
                options.AddConsole(c =>
                {
                    c.DisableColors = true;
                    c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss.fff] ";
                });
            });

            services.AddSingleton<MigrationAndSeedService>();
            services.AddScoped<IClaimsTransformation, ClaimsTransformer>();

            services.AddDbContext<CourtAdminDbContext>(options =>
                {
                    options.UseNpgsql(Configuration.GetNonEmptyValue("DatabaseConnectionString"), npg => npg.MigrationsAssembly("db"));
                    if (CurrentEnvironment.IsDevelopment())
                        options.EnableSensitiveDataLogging();
                }
            );

            services.AddSingleton(new AesGcmEncryptionOptions { Key = Configuration.GetNonEmptyValue("DataProtectionKeyEncryptionKey") });

            services.AddDataProtection()
                .PersistKeysToDbContext<CourtAdminDbContext>()
                .UseXmlEncryptor(s => new AesGcmXmlEncryptor(s))
                .SetApplicationName("SS");

            services.AddAuthorizationAndAuthentication(CurrentEnvironment, Configuration);

            services.AddMapster();
            services.AddMemoryCache();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(Configuration.GetValue<string>("CORS_DOMAIN"));
                });
            });

            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddSSServices(Configuration);

            services.AddControllers((opts) =>
            {
                //This fills in the claims, that AllowAnonymous wont trigger.
                if (DevelopmentMode)
                    opts.Filters.Add<DevelopmentEnvironmentClaimsFilter>();
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() };
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                options.CustomSchemaIds(o => o.FullName);

                options.AddSecurityDefinition("Bearer", //Name the security scheme
                new OpenApiSecurityScheme
                {
                        Description = "JWT Authorization header using the Bearer scheme.",
                        Type = SecuritySchemeType.Http, //We set the scheme type to http since we're using bearer authentication
                        Scheme = "bearer" //The name of the HTTP Authorization scheme to be used in the Authorization header. In this case "bearer".
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference{
                                Id = "Bearer", //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        }, new List<string>()
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            services.AddSwaggerGenNewtonsoftSupport();
            
            services.AddQuartz(q =>  
            {     
                q.AddJobAndTrigger<TrainingNotification>(Configuration);                           
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var baseUrl = Configuration.GetNonEmptyValue("WebBaseHref");
            app.Use((context, next) =>
            {
                context.Request.Scheme = "https";
                if (context.Request.Headers.ContainsKey("X-Forwarded-Host"))
                    context.Request.PathBase = new PathString(baseUrl.Remove(baseUrl.Length - 1));
                return next();
            });

            app.UseForwardedHeaders();

            app.UseCors();

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "api/swagger/{documentname}/swagger.json";
                options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    if (!httpReq.Headers.ContainsKey("X-Forwarded-Host"))
                        return;

                    var forwardedHost = httpReq.Headers["X-Forwarded-Host"];
                    var forwardedPort = httpReq.Headers["X-Forwarded-Port"];
                    swaggerDoc.Servers = new List<OpenApiServer>()
                    {
                        new OpenApiServer { Url = XForwardedForHelper.BuildUrlString(forwardedHost, forwardedPort, baseUrl) }
                    };
                });
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("swagger/v1/swagger.json", "CAS.API");
                options.RoutePrefix = "api";
            });

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //Note this will allow access everywhere for local development. 
                if (DevelopmentMode)
                    endpoints.MapControllers().WithMetadata(new AllowAnonymousAttribute());
                else
                    endpoints.MapControllers();
            });
        }
    }
}