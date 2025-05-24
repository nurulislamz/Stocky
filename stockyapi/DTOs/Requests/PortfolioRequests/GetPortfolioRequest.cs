using MediatR;
using stockyapi.Responses;

namespace stockyapi.Requests;

public class UserPortfolioRequest : IRequest<UserPortfolioResponse>
{
    public string? Symbol { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool? IncludeInactive { get; set; }
}