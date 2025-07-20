using System.ComponentModel.DataAnnotations;
using MediatR;
using stockyapi.Responses;

namespace stockyapi.Requests;

public class ChangePasswordRequest : IRequest<ChangePasswordResponse>
{
    [Required]
    public required string CurrentPassword { get; set; }

    [Required]
    public required string NewPassword { get; set; }

    [Required]
    public required string ConfirmNewPassword { get; set; }
}