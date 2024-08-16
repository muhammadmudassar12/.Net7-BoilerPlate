using System.Reflection;
using System.Security.Claims;
using System.Text;
using EMS20.WebApi.Application.Common.Exceptions;
using EMS20.WebApi.Infrastructure.Auth.Permissions;
using EMS20.WebApi.Shared.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EMS20.WebApi.Infrastructure.Auth.Jwt;

public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtSettings _jwtSettings;

    public ConfigureJwtBearerOptions(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(string.Empty, options);
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        byte[] key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateLifetime = true,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {

            //OnChallenge = context =>
            //{
            //    context.HandleResponse();
            //    if (!context.Response.HasStarted)
            //    {
            //        throw new UnauthorizedException("Authentication Failed.");
            //    }

            //    return Task.CompletedTask;
            //},
            //OnForbidden = _ => throw new ForbiddenException("You are not authorized to access this resource."),
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                if (!string.IsNullOrEmpty(accessToken) &&
                    context.HttpContext.Request.Path.StartsWithSegments("/notifications"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var claimsIdentity = context.Principal.Identity as ClaimsIdentity;

                if (claimsIdentity != null)
                {
                    var newRoles = claimsIdentity.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                    var newPermissions = claimsIdentity.FindAll(FSHClaims.Permission).Select(c => c.Value).ToList();
                    bool isAdminOrSuperAdmin = newRoles.Any(role => role == "Admin" || role == "SuperAdmin");

                    foreach (var role in newRoles)
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                    }

                    foreach (var permission in newPermissions)
                    {
                        claimsIdentity.AddClaim(new Claim(FSHClaims.Permission, permission));
                    }

                    if (!isAdminOrSuperAdmin)
                    {
                        var actionDescriptor = context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();
                        var mustHavePermissionAttribute = actionDescriptor?.MethodInfo.GetCustomAttribute<MustHavePermissionAttribute>();

                        if (mustHavePermissionAttribute != null)
                        {
                            string? requiredAction = mustHavePermissionAttribute.Policy;

                            if (!HasPermission(claimsIdentity, requiredAction))
                            {
                                throw new ForbiddenException("You are not authorized to access this resource.");
                            }
                        }
                    }
                }
                else
                {
                    throw new UnauthorizedException("Authentication Failed.");
                }

                return Task.CompletedTask;
            }
        };


    }
    private bool HasPermission(ClaimsIdentity claimsIdentity, string requiredPermission)
    {
        return claimsIdentity.HasClaim(c => c.Type == FSHClaims.Permission &&
                                            c.Value == requiredPermission);
    }
}