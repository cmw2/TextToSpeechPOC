using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Tasks;
using TextToSpeechPOC.Data;
using TextToSpeechPOC.Options;


namespace TextToSpeechPOC.Pages;

public partial class Index
{
    private IBrowserFile selectedfile;
    private string systemPrompt;
    private string userPrompt;
    private string docIntelligenceOutput;
    private string cleanedDocIntelligenceOutput;
    private string llmOutput;
    private bool showSidebar = false;
    private int activeTab = 1;
    private int activeLogTab = 1;
    private string audioFileName;

    // Inject the options
    [Inject]
    private IOptions<AzureOpenAIOptions> AzureOpenAIOptions { get; set; }

    [Inject]
    private AzureDocumentIntelligenceService DocumentIntelligenceService { get; set; }

    [Inject]
    private AzureSpeechService SpeechService { get; set; }
    
    [Inject]
    private AzureOpenAIService AOAIService { get; set; }
    protected override void OnInitialized()
    {
        systemPrompt = AzureOpenAIOptions.Value.SystemPrompt;
        userPrompt = AzureOpenAIOptions.Value.UserPrompt;
        // Use other properties as needed
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
        }
    }

    private async Task ExtractText()
    {
        if (!string.IsNullOrEmpty(cleanedDocIntelligenceOutput))
        {
            try
            {
                llmOutput = await AOAIService.GetExtractedText(cleanedDocIntelligenceOutput);
            }
            catch (Exception ex)
            {
                llmOutput = $"Error: {ex.Message}";
            }
        }
    }

    private async Task GenerateAudio()
    {
        if (!string.IsNullOrEmpty(llmOutput))
        {
            var audioFile = await SpeechService.SynthesizeSpeechToFileAsync(llmOutput);
            audioFileName = Path.GetFileName(audioFile);
        }
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
