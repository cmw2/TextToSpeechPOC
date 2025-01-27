using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Text.Json;
using System.Threading.Tasks;
using TextToSpeechPOC.Data;
using System.Text.Json;


namespace TextToSpeechPOC.Pages
{
    public partial class Index
    {
        private IBrowserFile selectedfile;
        private string systemPrompt;
        private string userPrompt;
        private string parsedText;

        [Inject]
        private DocumentIntelligenceService DocumentIntelligenceService { get; set; }
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
    }
}