﻿using System.ComponentModel.DataAnnotations;

namespace stockyapi.Requests;

public class LoginRequest
{
   [Required]
   [EmailAddress]
   public string Email { get; set; }

   [Required]
   [MinLength(8)]
   public string Password { get; set; }
}