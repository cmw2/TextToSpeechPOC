using System.ClientModel;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using TextToSpeechPOC.Options;

namespace TextToSpeechPOC.Data;

public class AzureOpenAIService
{
    private readonly ChatClient  _chatClient;
    private readonly AzureOpenAIOptions _options;


    public AzureOpenAIService(IOptions<AzureOpenAIOptions> options)
    {
        _options = options.Value;
        AzureOpenAIClient azureClient;

        if (!string.IsNullOrEmpty(_options.ApiKey))
        {
            var credential = new ApiKeyCredential(_options.ApiKey);
            azureClient = new AzureOpenAIClient(new Uri(_options.Endpoint), credential);
        }
        else
        {
            var credential = new DefaultAzureCredential();
            azureClient = new AzureOpenAIClient(new Uri(_options.Endpoint), credential);
        }        

        _chatClient = azureClient.GetChatClient(_options.ModelDeploymentName);
    }

    public async Task<string> GetChatCompletionAsync(string userInput)
    {
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(_options.SystemPrompt),
            new UserChatMessage(userInput)
        };        
        ChatCompletion completion = await _chatClient.CompleteChatAsync(messages);
        
        return completion.Content[0].Text ?? string.Empty;
    }    

    public async Task<string> GetExtractedText(string inputText)
    {
        // use the default user prompt and format it with the input text
        // then call getchatcompletion
        string userPrompt = string.Format(_options.UserPrompt, inputText);
        return await GetChatCompletionAsync(userPrompt);
    }
}
