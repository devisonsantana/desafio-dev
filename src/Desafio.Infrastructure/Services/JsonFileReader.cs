using System.Text.Json;

namespace Desafio.Infrastructure.Services;

public sealed class JsonFileReader
{
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
    private readonly string _directory;

    public JsonFileReader(string directory)
    {
        if (string.IsNullOrWhiteSpace(directory))
            throw new ArgumentException(
                "Data directory cannot be empty.",
                nameof(directory));

        _directory = directory;
    }
    public async Task<T> ReadJsonAsync<T>(string filename, CancellationToken cancellation = default)
    {
        if (string.IsNullOrWhiteSpace(filename))
            throw new ArgumentException(
                "Filename cannot be empty.",
                nameof(filename));

        var path = Path.Combine(_directory, filename);

        await using var stream = File.OpenRead(path);

        var result = await JsonSerializer.DeserializeAsync<T>(stream, _options, cancellation) ?? throw new InvalidOperationException($"Could not deserialize {filename} file.");

        return result;
    }
}
