using Microsoft.Extensions.Options;
using TextToSpeechPOC.Options;

public class AzureOpenAIOptionsService
{
    public AzureOpenAIOptions Options { get; private set; }

    public AzureOpenAIOptionsService(IOptions<AzureOpenAIOptions> options)
    {
        Options = new AzureOpenAIOptions
        {
            SystemPrompt = options.Value.SystemPrompt,
            UserPrompt = options.Value.UserPrompt,
            Temperature = options.Value.Temperature,
            MaxTokens = options.Value.MaxTokens,
            ModelDeploymentName = options.Value.ModelDeploymentName,
            Endpoint = options.Value.Endpoint,
            ApiKey = options.Value.ApiKey
        };
    }

    public void UpdateOptions(string systemPrompt, string userPrompt, float temperature, int maxTokens)
    {
        Options.SystemPrompt = systemPrompt;
        Options.UserPrompt = userPrompt;
        Options.Temperature = temperature;
        Options.MaxTokens = maxTokens;
    }
}