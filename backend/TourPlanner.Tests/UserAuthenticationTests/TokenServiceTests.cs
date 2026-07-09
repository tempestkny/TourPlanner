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
    { // simulates configuration
        Issuer = "TourPlanner.Api.Tests", // who issues the token
        Audience = "TourPlanner.Frontend.Tests", // intended audience for the token
        Secret = "TourPlannerTestSecretKeyWithEnoughLength123", // key used to sign the token 
        ExpirationMinutes = 60 // token expiration time in minutes
    };

    [Test]
    public void GenerateToken_WithUser_ShouldReturnJwt()
    {
        var tokenService = CreateTokenService();
        var user = CreateUser();

        var token = tokenService.GenerateToken(user);

        Assert.That(token, Is.Not.Empty);
        Assert.That(token.Split('.'), Has.Length.EqualTo(3)); // header, payload, signature
    }

    [Test]
    public void GenerateToken_ShouldContainConfiguredIssuerAndAudience()
    {
        var token = ReadToken(CreateTokenService().GenerateToken(CreateUser()));

        Assert.That(token.Issuer, Is.EqualTo(jwtOptions.Issuer));
        Assert.That(token.Audiences.Single(), Is.EqualTo(jwtOptions.Audience));
    }

    [Test]
    public void GenerateToken_ShouldContainUserIdClaim()
    {
        var user = CreateUser();

        var token = ReadToken(CreateTokenService().GenerateToken(user));

        var userIdClaim = token.Claims.Single(claim => claim.Type == ClaimTypes.NameIdentifier);
        Assert.That(userIdClaim.Value, Is.EqualTo(user.Id));
    }

    [Test]
    public void GenerateToken_ShouldNotContainUsernameClaim()
    {
        var token = ReadToken(CreateTokenService().GenerateToken(CreateUser()));
        var claimTypes = token.Claims.Select(claim => claim.Type).ToList();

        Assert.That(claimTypes, Does.Not.Contain(JwtRegisteredClaimNames.UniqueName));
        Assert.That(claimTypes, Does.Not.Contain(ClaimTypes.Name));
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
