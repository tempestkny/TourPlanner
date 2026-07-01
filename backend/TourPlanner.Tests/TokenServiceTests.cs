using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using TourPlanner.Bll.Auth;
using TourPlanner.Models;

namespace TourPlanner.Tests;

[TestFixture]
public class TokenServiceTests
{
    private readonly JwtOptions jwtOptions = new()
    {
        Issuer = "TourPlanner.Api.Tests",
        Audience = "TourPlanner.Frontend.Tests",
        Secret = "TourPlannerTestSecretKeyWithEnoughLength123",
        ExpirationMinutes = 60
    };

    [Test]
    public void GenerateToken_WithUser_ShouldReturnJwt()
    {
        var tokenService = CreateTokenService();
        var user = CreateUser();

        var token = tokenService.GenerateToken(user);

        Assert.That(token, Is.Not.Empty);
        Assert.That(token.Split('.'), Has.Length.EqualTo(3));
    }

    [Test]
    public void GenerateToken_ShouldContainConfiguredIssuerAndAudience()
    {
        var token = ReadToken(CreateTokenService().GenerateToken(CreateUser()));

        Assert.That(token.Issuer, Is.EqualTo(jwtOptions.Issuer));
        Assert.That(token.Audiences.Single(), Is.EqualTo(jwtOptions.Audience));
    }

    [Test]
    public void GenerateToken_ShouldContainUsernameClaim()
    {
        var token = ReadToken(CreateTokenService().GenerateToken(CreateUser()));

        var usernameClaim = token.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.UniqueName);
        Assert.That(usernameClaim.Value, Is.EqualTo("TourUser"));
    }

    [Test]
    public void GenerateToken_ShouldNotContainPasswordData()
    {
        var token = ReadToken(CreateTokenService().GenerateToken(CreateUser()));
        var claimTypes = token.Claims.Select(claim => claim.Type).ToList();

        Assert.That(claimTypes, Does.Not.Contain("Password"));
        Assert.That(claimTypes, Does.Not.Contain("HashedPassword"));
    }

    [Test]
    public void GenerateToken_ShouldSetExpiration()
    {
        var token = ReadToken(CreateTokenService().GenerateToken(CreateUser()));

        Assert.That(token.ValidTo, Is.GreaterThan(DateTime.UtcNow));
        Assert.That(token.ValidTo, Is.LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(jwtOptions.ExpirationMinutes + 1)));
    }

    private TokenService CreateTokenService()
    {
        return new TokenService(Options.Create(jwtOptions));
    }

    private static JwtSecurityToken ReadToken(string token)
    {
        return new JwtSecurityTokenHandler().ReadJwtToken(token);
    }

    private static User CreateUser()
    {
        return new User
        {
            Email = "user@example.com",
            Username = "TourUser",
            HashedPassword = "hashed-password"
        };
    }
}
