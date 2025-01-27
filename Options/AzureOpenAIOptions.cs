namespace TextToSpeechPOC.Options;

public class AzureOpenAIOptions
{
    public string Endpoint { get; set; }
    public string ApiKey { get; set; }
    public string ModelDeploymentName { get; set; }
    public string SystemPrompt { get; set; }
    public string UserPrompt { get; set; }
    public double Temperature { get; set; }
    public int MaxTokens { get; set; }
    public int TopP { get; set; }
}