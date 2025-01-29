using System.Collections.Concurrent;

namespace TextToSpeechPOC.Data;

public class FileService
{
    private readonly ConcurrentDictionary<string, string> _fileMappings = new ConcurrentDictionary<string, string>();

    public void AddFileMapping(string token, string filePath)
    {
        _fileMappings[token] = filePath;
    }

    public string GetFilePath(string token)
    {
        return _fileMappings.TryGetValue(token, out var filePath) ? filePath : null;
    }

    public string GetNewFilePath(string filename)
    {
        return Path.Combine(Path.GetTempPath(), filename);
    }

    public byte[] GetFileBytes(string token)
    {
        var filePath = GetFilePath(token);
        if (filePath == null || !System.IO.File.Exists(filePath))
        {
            return null;
        }

        return System.IO.File.ReadAllBytes(filePath);
    }
}