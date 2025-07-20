using MediatR;
using stockyapi.Requests;
using stockyapi.Responses;
using stockyapi.Repository.AI;

namespace stockyapi.RequestHandlers.Ai;

public class AiRequestHandler : IRequestHandler<AiRequest, AiResponse>
{
    private readonly IStockyAiRepository _stockyAiRepository;

    public AiRequestHandler(IStockyAiRepository stockyAiRepository)
    {
        _stockyAiRepository = stockyAiRepository;
    }

    public async Task<AiResponse> Handle(AiRequest request, CancellationToken cancellationToken)
    {
        var chatGptResponse = await _stockyAiRepository.CreateAsync(request.Prompt, request.ApiKey);

        return new AiResponse
        {
            Success = true,
            StatusCode = 200,
            Message = chatGptResponse.Message,
            Data = new AiData { Response = chatGptResponse.Message }
        };
    }
}