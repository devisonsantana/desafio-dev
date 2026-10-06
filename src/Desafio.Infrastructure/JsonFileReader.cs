using System.Text.Json;
using Desafio.Core;

namespace Desafio.Infrastructure;

public sealed class JsonFileReader
{
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
    private readonly string _directoty;

    public JsonFileReader(string directoty)
    {
        if (string.IsNullOrWhiteSpace(directoty))
            throw new ArgumentException(
                "Data directory cannot be empty.",
                nameof(directoty));

        _directoty = directoty;
    }
    public async Task<T> ReadJsonAsync<T>(string filename, CancellationToken cancellation = default)
    {
        var path = Path.Combine(_directoty, filename);

        await using var stream = File.OpenRead(path);

        var result = await JsonSerializer.DeserializeAsync<T>(stream, _options, cancellation) ?? throw new InvalidOperationException($"Could not deserialize {filename} file.");

        return result;
    }
}
