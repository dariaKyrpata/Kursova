using System.Text.Json;

namespace Domain.Core.Services;

public class JsonFileService<T>
{
    private readonly string _filePath;

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonFileService(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be empty.", nameof(filePath));
        }

        _filePath = Path.GetFullPath(filePath);
    }

    public void Save(List<T> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        string? directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string json = JsonSerializer.Serialize(data, _options);
        File.WriteAllText(_filePath, json);
    }

    public List<T> Load()
    {
        if (!File.Exists(_filePath))
        {
            return new List<T>();
        }

        string json = File.ReadAllText(_filePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<T>();
        }

        return JsonSerializer.Deserialize<List<T>>(json, _options) ?? new List<T>();
    }
}