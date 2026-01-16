using System.ComponentModel.DataAnnotations;
using stockyapi.Responses;

namespace stockyapi.Requests;

public class CurrentPriceDataRequest
{
    [Required]
    public required string Ticker { get; set; }
}
