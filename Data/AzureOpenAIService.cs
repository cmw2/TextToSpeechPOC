using System.ClientModel;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using TextToSpeechPOC.Options;
using SharpToken;

namespace TextToSpeechPOC.Data;

public class AzureOpenAIService
{
    private readonly ChatClient  _chatClient;
    private readonly AzureOpenAIOptions _options;


    public AzureOpenAIService(AzureOpenAIOptionsService optionsService)
    {
        _options = optionsService.Options;
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
        var completionOptions = new ChatCompletionOptions
        {
            Temperature = _options.Temperature,
            MaxOutputTokenCount = _options.MaxTokens
        };         
        ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, completionOptions);
        
        return completion.Content[0].Text ?? string.Empty;
    }    

    public async Task<string> GetExtractedText(string inputText)
    {
        // use the default user prompt and format it with the input text
        // then call getchatcompletion
        string userPrompt = string.Format(_options.UserPrompt, inputText);
        //return await GetChatCompletionAsync(userPrompt);
        return await ExtractTextInChunksAsync(userPrompt);

    }

    public async Task<string> ExtractTextInChunksAsync(string inputText)
    {
        // Step 1: Create the GPT-4 tokenizer  
        var encoding = GptEncoding.GetEncoding("cl100k_base"); // for GPT-4, GPT-3.5-turbo, etc.  

        // Step 2: Set limits (adjust per your needs & model!)  
        int maxTokensPerRequest = 4096; // Most OpenAI GPT-4 models  
        int maxCompletionTokens = 2048; // How much output you want PER REQUEST  
        int reservedForPrompt = 256;    // Estimate tokens used by your prompt template  

        // Step 3: Figure out input chunk size in tokens  
        int inputChunkTokens = maxTokensPerRequest - maxCompletionTokens - reservedForPrompt;

        // Tokenize the full input  
        var inputTokens = encoding.Encode(inputText);

        // Split input into chunks of up to inputChunkTokens  
        var chunkedTokens = ChunkTokens(inputTokens, inputChunkTokens);

        var outputs = new List<string>();

        foreach (var chunk in chunkedTokens)
        {
            // Decode token chunk back into string  
            string chunkText = encoding.Decode(chunk.ToArray());

            // Plug into your prompt template  
            string userPrompt = string.Format(_options.UserPrompt, chunkText);

            // Call your completion method (e.g., OpenAI API)  
            string chunkOutput = await GetChatCompletionAsync(userPrompt);

            outputs.Add(chunkOutput.Trim());
        }

        // Combine all outputs  
        return string.Join(" ", outputs);
    }

    // Helper function to chunk tokens  
    public static IEnumerable<List<int>> ChunkTokens(IReadOnlyList<int> tokens, int chunkSize)
    {
        for (int i = 0; i < tokens.Count; i += chunkSize)
        {
            yield return tokens.Skip(i).Take(chunkSize).ToList();
        }
    }

}
