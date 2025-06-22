using System;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using stockymodels.models;
using stockyapi.Options;

namespace stockyapi.Services;

public interface ITokenService
{
  string CreateToken(UserModel user);
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
    // Create claims (user info)
    var claims = new[]
    {
      new Claim("email", user.Email),
      new Claim("userId", user.Id.ToString()),
      new Claim("firstName", user.FirstName),
      new Claim("surname", user.Surname),
      new Claim("role", user.Role.ToString())
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
}