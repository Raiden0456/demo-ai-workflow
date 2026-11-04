using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Demo.Api.Auth;

public sealed class DemoHeaderAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "DemoHeader";
    public const string Header = "X-Demo-User";
    public const string TenantClaim = "tenant";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Header, out var value))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!DemoUsers.All.TryGetValue(value.ToString(), out var user))
        {
            return Task.FromResult(AuthenticateResult.Fail("Unknown user"));
        }

        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(TenantClaim, user.TenantId),
            ],
            SchemeName);

        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName)));
    }
}

public static class ClaimsPrincipalExtensions
{
    public static string TenantId(this ClaimsPrincipal user) =>
        user.FindFirstValue(DemoHeaderAuthHandler.TenantClaim)
        ?? throw new InvalidOperationException("Authenticated user has no tenant claim");
}
