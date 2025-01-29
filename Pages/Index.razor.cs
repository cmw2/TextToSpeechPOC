using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using TextToSpeechPOC.Data;
using TextToSpeechPOC.Options;

namespace TextToSpeechPOC.Pages;

public partial class Index
{
    private IBrowserFile selectedfile;
    private bool isProcessing = false;
    private string statusMessage = string.Empty;
    private string systemPrompt;
    private string userPrompt;
    private float temperature;
    private int maxTokens;
    private string docIntelligenceOutput;
    private string cleanedDocIntelligenceOutput;
    private string llmOutput;
    private bool showSidebar = false;
    private int activeTab = 1;
    private int activeLogTab = 1;
    private string audioFileToken;

    // Inject the options
    [Inject]
    private IOptions<AzureOpenAIOptions> AzureOpenAIOptions { get; set; }

    [Inject]
    private AzureDocumentIntelligenceService DocumentIntelligenceService { get; set; }

    [Inject]
    private AzureSpeechService SpeechService { get; set; }
    
    [Inject]
    private AzureOpenAIService AOAIService { get; set; }
    
    [Inject]
    private AzureOpenAIOptionsService AOAIOptionsService { get; set; }

    protected override void OnInitialized()
    {
        var options = AOAIOptionsService.Options;
        systemPrompt = options.SystemPrompt;
        userPrompt = options.UserPrompt;
        temperature = options.Temperature;
        maxTokens = options.MaxTokens;
    }

    private void HandleFileSelected(InputFileChangeEventArgs e)
    {
        selectedfile = e.File;
    }

    private async Task AnalyzeFile()
    {
        if (selectedfile != null)
        {
            try
            {
                isProcessing = true;
                using var stream = selectedfile.OpenReadStream();
                var analyzeResult = await DocumentIntelligenceService.ExtractTextFromFileAsync(stream);
                docIntelligenceOutput = JsonSerializer.Serialize(analyzeResult, new JsonSerializerOptions { WriteIndented = true });
                var cleanedResult = DocumentIntelligenceService.CleanAnalyzeResult(analyzeResult);
                cleanedDocIntelligenceOutput = JsonSerializer.Serialize(cleanedResult, new JsonSerializerOptions { WriteIndented = true });                
            }
            catch (Exception ex)
            {
                cleanedDocIntelligenceOutput = $"Error: {ex.Message}";
            }
            finally
            {
                isProcessing = false;
            }
        }
        else {
            cleanedDocIntelligenceOutput = "Please select a file to analyze.";
        }
    }

    private async Task ExtractText()
    {
        if (!string.IsNullOrEmpty(cleanedDocIntelligenceOutput))
        {
            try
            {
                isProcessing = true;
                llmOutput = await AOAIService.GetExtractedText(cleanedDocIntelligenceOutput);
            }
            catch (Exception ex)
            {
                llmOutput = $"Error: {ex.Message}";
            }
            finally
            {
                isProcessing = false;
            }
        }  else {
            llmOutput = "Please analyze a file first.";
        }
    }

    private async Task GenerateAudio()
    {
        if (!string.IsNullOrEmpty(llmOutput))
        {
            isProcessing = true;
            statusMessage = string.Empty;
            try
            {
                audioFileToken = await SpeechService.SynthesizeSpeechToFileAsync(llmOutput);
            }
            catch (Exception ex)
            {
                statusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                isProcessing = false;
            }
        } else {
            statusMessage = "Please extract text first.";
        }   
    }

    private void UpdateOptions()
    {
        AOAIOptionsService.UpdateOptions(systemPrompt, userPrompt, temperature, maxTokens);
    }

    private void ToggleSidebar()
    {
        showSidebar = !showSidebar;
    }
    private void ShowSettings()
    {
        showSidebar = true;
        activeTab = 1;
    }

    private void ShowLog()
    {
        showSidebar = true;
        activeTab = 2;
    }
    private void ShowDocIntelligenceOutput()
    {
        activeLogTab = 1;
    }

    private void ShowCleanedDocIntelligenceOutput()
    {
        activeLogTab = 2;
    }

    private void ShowLLMOutput()
    {
        activeLogTab = 3;
    }    
}
