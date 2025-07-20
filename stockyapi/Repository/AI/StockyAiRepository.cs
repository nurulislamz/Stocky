using stockyapi.Requests;
using stockyapi.Responses;
using stockymodels.Data;
using OpenAI.Chat;

namespace stockyapi.Repository.AI;

public class StockyAiRepository : IStockyAiRepository
{
    private readonly ApplicationDbContext _context;

    public StockyAiRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ChatGptResponse> CreateAsync(ChatGptRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<ChatGptResponse> CreateAsync(string prompt, string apiKey)
    {
        ChatClient client = new ChatClient(model: "gpt-4o-mini", apiKey: apiKey);

        ChatCompletion completion = await client.CompleteChatAsync(prompt);

        return new ChatGptResponse
        {
            Message = completion.Content[0].Text
        };
    }
}