using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using stockymodels.models;
using stockyapi.Options;

namespace stockyapi.Services;

public interface ITokenService
{
  string CreateToken(UserModel user);
  string CreateDevelopmentToken(Claim[] claims);
}

public class TokenService : ITokenService
{
  private readonly JwtSettings _jwtSettings;

  public TokenService(IOptions<JwtSettings> jwtSettings)
  {
    _jwtSettings = jwtSettings.Value;
  }

  public string CreateToken(UserModel user)
  {
    // Create claims (user info) using standard ClaimTypes
    var claims = new[]
    {
      new Claim(ClaimTypes.Email, user.Email),
      new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new Claim(ClaimTypes.GivenName, user.FirstName),
      new Claim(ClaimTypes.Surname, user.Surname),
      new Claim(ClaimTypes.Role, user.Role.ToString())
    };

    // Create token
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    // Generate token
    var token = new JwtSecurityToken(
      issuer: _jwtSettings.Issuer,
      audience: _jwtSettings.Audience,
      claims: claims,
      expires: DateTime.Now.AddMinutes(_jwtSettings.ExpirationInMinutes),
      signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  public string CreateDevelopmentToken(Claim[] claims)
  {
    // Create token
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    // Generate token
    var token = new JwtSecurityToken(
      issuer: _jwtSettings.Issuer,
      audience: _jwtSettings.Audience,
      claims: claims,
      expires: DateTime.Now.AddMinutes(_jwtSettings.ExpirationInMinutes),
      signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}