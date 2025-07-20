using MediatR;
using stockyapi.Responses;

namespace stockyapi.Requests;

public class ChatGptRequest
{
    public required string Message { get; set; }
    public required string Model { get; set; }
    public string? ApiKey { get; set; }

    public ChatGptRequest(Prompt prompt, string model)
    {
        Message = prompt.ToString();
        Model = model;
    }

    public ChatGptRequest(string prompt, string apiKey)
    {
        Message = prompt;
        Model = "gpt-4o-mini";
        ApiKey = apiKey;
    }

    public ChatGptRequest() { }
}

public class Prompt
{
    public string Role { get; set; }
    public string Task { get; set; }
    public string ResponseFormat { get; set; }
    public string? Rules { get; set; }
    public string? Content { get; set; }
    public string? Example { get; set; }
    public string? ExampleResponse { get; set; }

    public Prompt(string role, string task, string responseFormat, string? rules, string? content, string? example, string? exampleResponse)
    {
        Role = role;
        Task = task;
        ResponseFormat = responseFormat;
        Rules = rules;
        Content = content;
        Example = example;
        ExampleResponse = exampleResponse;
    }

    public override string ToString()
    {
        return $"Role: {Role}\nTask: {Task}\nResponseFormat: {ResponseFormat}\nRules: {Rules}\nContent: {Content}\nExample: {Example}\nExampleResponse: {ExampleResponse}";
    }
}