using System.Text.Json;
using System.Text.Json.Nodes;
using Azure;
using Azure.AI.DocumentIntelligence;
using Azure.Identity;
using Microsoft.Extensions.Options;
using TextToSpeechPOC.Options;

namespace TextToSpeechPOC.Data;

public class AzureDocumentIntelligenceService
{
    private readonly DocumentIntelligenceClient _client;

    public AzureDocumentIntelligenceService(IOptions<AzureDocumentIntelligenceOptions> options)
    {
        var endpoint = options.Value.Endpoint;
        var apiKey = options.Value.ApiKey;

        if (!string.IsNullOrEmpty(apiKey))
        {
            var credential = new AzureKeyCredential(apiKey);
            _client = new DocumentIntelligenceClient(new Uri(endpoint), credential);
        }
        else
        {
            var credential = new DefaultAzureCredential();
            _client = new DocumentIntelligenceClient(new Uri(endpoint), credential);
        }
    }

    public async Task<AnalyzeResult> ExtractTextFromFileAsync(Stream fileStream)
    {
        BinaryData binaryData = await BinaryData.FromStreamAsync(fileStream);
        Operation<AnalyzeResult> operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-layout", binaryData);
        AnalyzeResult result = operation.Value;
        return result;
    }

    public Dictionary<string, object> CleanAnalyzeResult(AnalyzeResult analyzeResult)
    {
        var json = JsonSerializer.Serialize(analyzeResult);
        var jsonObject = JsonNode.Parse(json) as JsonObject;

        var keysToRemove = new HashSet<string> { "BoundingRegions", "Words", "Polygon" };
        RemoveElements(jsonObject, keysToRemove);

        var cleanedResult = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonObject.ToJsonString());
        return cleanedResult;
    }

    private void RemoveElements(JsonObject jsonObject, HashSet<string> keysToRemove)
    {
        foreach (var key in keysToRemove)
        {
            jsonObject.Remove(key);
        }

        foreach (var property in jsonObject)
        {
            if (property.Value is JsonObject nestedObject)
            {
                RemoveElements(nestedObject, keysToRemove);
            }
            else if (property.Value is JsonArray nestedArray)
            {
                foreach (var item in nestedArray)
                {
                    if (item is JsonObject arrayObject)
                    {
                        RemoveElements(arrayObject, keysToRemove);
                    }
                }
            }
        }
    }
}
