using Cosmos.Chat.GPT.Models;
using Azure;
using Azure.AI.OpenAI;

namespace Cosmos.Chat.GPT.Services;

public class OpenAiService
{
    private readonly string _modelName = String.Empty;

    private readonly OpenAIClient _client;

    private readonly string _systemPrompt = @"You are a Franmer AI assistant that helps people find information " + Environment.NewLine;

    private readonly string _sumarizePromt = @"Summarize this prompt is one or two words to use as a label in a button. No punctuation " + Environment.NewLine;

    public OpenAiService(string endpoint, string key, string modelName)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(modelName);
        ArgumentNullException.ThrowIfNullOrEmpty(endpoint);
        ArgumentNullException.ThrowIfNullOrEmpty(key);

        Uri uri = new(endpoint);
        AzureKeyCredential credential = new(key);
        _client = new(
            endpoint: uri,
            keyCredential: credential 
        );
    }

    public async Task<(string completionText, int completionTokens)> GetChatCompletionAsync(string sessionId, string userPrompt)
    {
        
        ChatMessage systemMessage = new(ChatRole.System, _systemPrompt);
        ChatMessage userMessage = new(ChatRole.User, userPrompt);

        ChatCompletionsOptions options = new()
        {
            Messages = 
            {
                systemMessage,
                userMessage
            },
        User = sessionId,
        MaxTokens = 4000,
        Temperature = 0.3f,
        NucleusSamplingFactor = 0.5f,
        FrequencyPenalty = 0,
        PresencePenalty = 0
        };
        
        ChatCompletions completions = await _client.GetChatCompletionsAsync(_modelName, options);

        return (
            completionText: completions.Choices[0].Message.Content,
            completionsTokens: completions.Usage.CompletionTokens
        );
    }

    public async Task<string> SummarizeAsync(string sessionId, string conversationText)
    {
        
        ChatMessage systemMessage = new(ChatRole.System, _sumarizePromt);
        ChatMessage userMessage = new(ChatRole.User, conversationText);

        ChatCompletionsOptions options = new()
        {
            Messages = 
            {
                systemMessage,
                userMessage
            },
        User = sessionId,
        MaxTokens = 200,
        Temperature = 0.0f,
        NucleusSamplingFactor = 1.0f,
        FrequencyPenalty = 0,
        PresencePenalty = 0
        }; 
        
        
        ChatCompletions completions = await _client.GetChatCompletionsAsync(_modelName, options);
        string completionText = completions.Choices[0].Message.Content;

        return completionText;
  
    }
}
