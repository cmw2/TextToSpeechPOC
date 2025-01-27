using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Options;
using TextToSpeechPOC.Options;

namespace TextToSpeechPOC.Data;

public class AzureSpeechService
{
    private readonly AzureSpeechOptions _options;
    private readonly SpeechConfig _speechConfig;

    public AzureSpeechService(IOptions<AzureSpeechOptions> options)
    {
        _options = options.Value;
        _speechConfig = SpeechConfig.FromSubscription(_options.ApiKey, _options.Region);      

        // The neural multilingual voice can speak different languages based on the input text.
        _speechConfig.SpeechSynthesisVoiceName = _options.VoiceName; 

    }

    public async Task<string> SynthesizeSpeechToFileAsync(string text)
    {
        string tempFileName = $"speech_{DateTime.Now:yyyyMMdd_HHmmss}.wav";
        string tempFilePath = Path.Combine(Path.GetTempPath(), tempFileName);
        var audioConfig = AudioConfig.FromWavFileOutput(tempFilePath);
        using var synthesizer = new SpeechSynthesizer(_speechConfig, audioConfig);
        var result = await synthesizer.SpeakTextAsync(text);

        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
        {
            return tempFilePath;
        }
        else if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
            throw new Exception($"Speech synthesis canceled: {cancellation.Reason}, {cancellation.ErrorDetails}");
        }

        throw new Exception("Speech synthesis failed.");
    }
}