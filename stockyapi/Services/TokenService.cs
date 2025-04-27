using System;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using stockyapi.Models;
using stockyapi.Data;
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
      new Claim(ClaimTypes.Email, user.Email),
      new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
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