using stockyapi.Requests;
using stockyapi.Responses;

namespace stockyapi.Repository.AI;

public interface IStockyAiRepository
{
    Task<ChatGptResponse> CreateAsync(ChatGptRequest request);

    Task<ChatGptResponse> CreateAsync(string prompt, string apiKey);
}
