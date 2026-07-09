using System.IdentityModel.Tokens.Jwt; // Creates writes tokens
using System.Security.Claims; // store information inside token
using System.Text; 
using Microsoft.Extensions.Options; // used to sign token
using Microsoft.IdentityModel.Tokens; // reads jwt options from configuration
using TourPlanner.Models;

namespace TourPlanner.Bll.Auth;

public class TokenService : ITokenService
{
    private readonly JwtOptions jwtOptions;

    public TokenService(IOptions<JwtOptions> jwtOptions)
    {
        this.jwtOptions = jwtOptions.Value; // gives access to the JwtOptions 
    }

    public string GenerateToken(User user) // creates token
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id) // id stored in token as a claim
        };

        var tokenDescriptor = new SecurityTokenDescriptor // describes the token
        {
            Subject = new ClaimsIdentity(claims), // contains claims
            Expires = DateTime.UtcNow.AddMinutes(jwtOptions.ExpirationMinutes), // token lifetime
            SigningCredentials = credentials, // how the token is signed
            Issuer = jwtOptions.Issuer, // who created token
            Audience = jwtOptions.Audience // who the token is meant for
        };

        var tokenHandler = new JwtSecurityTokenHandler(); 
        var token = tokenHandler.CreateToken(tokenDescriptor); // creates token based on the descriptor

        return tokenHandler.WriteToken(token);  // returns the token as a string
    }
}
