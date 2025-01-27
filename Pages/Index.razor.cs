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
    private string parsedText;
    private bool showSidebar = false;
    private int activeTab = 1;

    // Inject the options
    [Inject]
    private IOptions<AzureOpenAIOptions> AzureOpenAIOptions { get; set; }

    [Inject]
    private AzureDocumentIntelligenceService DocumentIntelligenceService { get; set; }
    
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
            using var stream = selectedfile.OpenReadStream();
            var analyzeResult = await DocumentIntelligenceService.ExtractTextFromFileAsync(stream);
            var cleanedResult = DocumentIntelligenceService.CleanAnalyzeResult(analyzeResult);
            parsedText = JsonSerializer.Serialize(cleanedResult, new JsonSerializerOptions { WriteIndented = true });
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
}
