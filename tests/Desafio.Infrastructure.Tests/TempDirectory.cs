namespace Desafio.Infrastructure.Tests;

public sealed class TempDirectory : IDisposable
{
    public string BaseDirectory { get; }
    public TempDirectory()
    {
        var baseDir = AppContext.BaseDirectory;
        BaseDirectory = Path.Combine(baseDir, "Temp", Guid.NewGuid().ToString());
        Directory.CreateDirectory(BaseDirectory);
    }
    public void Dispose()
    {
        if (Directory.Exists(BaseDirectory))
            Directory.Delete(BaseDirectory, recursive: true);
    }
}
