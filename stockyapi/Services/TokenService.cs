using System;
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

// TODO: Change name to public static class CustomClaimTypes
public static class CustomClaimTypes
{
    public const string UserId = "userId";
    public const string FirstName = "firstName";
    public const string Surname = "surname";
    public const string Role = "role";
    public const string Email = "email";
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
      new Claim(CustomClaimTypes.Email, user.Email),
      new Claim(CustomClaimTypes.UserId, user.Id.ToString()),
      new Claim(CustomClaimTypes.FirstName, user.FirstName),
      new Claim(CustomClaimTypes.Surname, user.Surname),
      new Claim(CustomClaimTypes.Role, user.Role.ToString())
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