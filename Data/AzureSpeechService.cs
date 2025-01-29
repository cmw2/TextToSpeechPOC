using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Options;
using TextToSpeechPOC.Options;

namespace TextToSpeechPOC.Data;

public class AzureSpeechService
{
    private readonly AzureSpeechOptions _options;
    private readonly SpeechConfig _speechConfig;
    private readonly FileService _fileService;

    public AzureSpeechService(IOptions<AzureSpeechOptions> options, FileService fileService)
    {
        _options = options.Value;
        _fileService = fileService;

        _speechConfig = SpeechConfig.FromSubscription(_options.ApiKey, _options.Region);      
        _speechConfig.SpeechSynthesisVoiceName = _options.VoiceName; 

    }

    public async Task<string> SynthesizeSpeechToFileAsync(string text)
    {
        string tempFileName = $"speech_{DateTime.Now:yyyyMMdd_HHmmss}.wav";
        string tempFilePath = _fileService.GetNewFilePath(tempFileName);
        var audioConfig = AudioConfig.FromWavFileOutput(tempFilePath);
        using var synthesizer = new SpeechSynthesizer(_speechConfig, audioConfig);
        var result = await synthesizer.SpeakTextAsync(text);

        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
        {
            string token = Guid.NewGuid().ToString();
            _fileService.AddFileMapping(token, tempFilePath);
            return token;
        }
        else if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
            throw new Exception($"Speech synthesis canceled: {cancellation.Reason}, {cancellation.ErrorDetails}");
        }

        throw new Exception("Speech synthesis failed.");
    }  
}