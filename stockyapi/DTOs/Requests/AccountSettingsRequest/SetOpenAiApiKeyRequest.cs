using MediatR;
using stockyapi.Responses;

namespace stockyapi.Requests;

public class SetOpenAiApiKeyRequest : IRequest<SetOpenAiApiKeyResponse>
{
    public required string ApiKey { get; set; }
}