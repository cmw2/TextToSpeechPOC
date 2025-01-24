
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure;
using Azure.Identity;
using Azure.AI.DocumentIntelligence;

namespace TextToSpeechPOC.Data;

public class DocumentIntelligenceService
{
    private readonly DocumentIntelligenceClient _client;

    public DocumentIntelligenceService(string endpoint, string? apiKey = null)
    {
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
        var analyzeResultDict = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(analyzeResult));
        var keysToRemove = new HashSet<string> { "boundingRegions", "words", "polygon" };
        RemoveElements(analyzeResultDict, keysToRemove);
        return analyzeResultDict;
    }

    private void RemoveElements(object element, HashSet<string> keysToRemove)
    {
        if (element is Dictionary<string, object> dict)
        {
            foreach (var key in keysToRemove)
            {
                dict.Remove(key);
            }
            foreach (var key in dict.Keys.ToList())
            {
                RemoveElements(dict[key], keysToRemove);
            }
        }
        else if (element is List<object> list)
        {
            foreach (var item in list)
            {
                RemoveElements(item, keysToRemove);
            }
        }
    }
}
