using stockymodels.Models;
using stockymodels.Data;
using stockyapi.Requests;
using stockyapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using stockymodels.models;

namespace stockyapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  private readonly ApplicationDbContext _context;
  private readonly ITokenService _tokenService;
  private readonly ILogger<string> logger;

  public AuthController(ApplicationDbContext context, ITokenService tokenService, ILogger<string> _logger)
  {
    _context = context;
    _tokenService = tokenService;
    _logger = logger;
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginRequest request)
  {
    try
    {
      // Find user
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
      if (user == null)
        return Unauthorized(new { error = "Invalid credentials" });

      // Verify password
      if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        return Unauthorized(new { error = "Invalid credentials" });

      // JWT token generation here
      var token = _tokenService.CreateToken(user);

      return Ok(new { message = "Login successful", token = token });
    }
    catch (Exception ex)
    {
      // Log the error here
      return StatusCode(500, "An error occurred during login");
      
    }
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterRequest request)
  {
    try
    {
      // Check if user exists
      if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        return BadRequest(new { error ="Registration failed"});  // Vague message for security

      var user = new UserModel
      {
        FirstName = request.FirstName,
        Surname = request.Surname,
        Email = request.Email,
        Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
      };

      _context.Users.Add(user);
      await _context.SaveChangesAsync();
      
      // JWT token generation here
      var token = _tokenService.CreateToken(user);

      return Ok(new { message = "Registration successful", token = token });
    }
    catch (Exception ex)
    {
      // Log the error here
      return StatusCode(500, "An error occurred during registration");
    }
  }
}