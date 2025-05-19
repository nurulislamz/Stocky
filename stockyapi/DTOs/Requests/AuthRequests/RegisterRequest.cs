﻿using System.ComponentModel.DataAnnotations;
using MediatR;
using stockyapi.Responses;

namespace stockyapi.Requests;

public class RegisterRequest : IRequest<RegisterResponse>
{
  [Required]
  [StringLength(50)]
  public required string FirstName { get; set; }

  [Required]
  [StringLength(50)]
  public required string Surname { get; set; }

  [Required]
  [EmailAddress]
  [MaxLength(255)]
  public required string Email { get; set; }

  [Required]
  [MinLength(8)]
  public required string Password { get; set; }
}