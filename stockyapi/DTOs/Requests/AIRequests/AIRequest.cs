using MediatR;
using stockyapi.Responses;

namespace stockyapi.Requests;

public class AiRequest : IRequest<AiResponse>
{
  public required string ApiKey { get; set; }
  public required string Prompt { get; set; }
}