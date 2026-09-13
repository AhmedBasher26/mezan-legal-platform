using Mezan.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace Mezan.Infrastructure.Storage;

public class FileStorageService : IFileStorageService
{
    private readonly string _uploadDirectory;

    public FileStorageService(IWebHostEnvironment environment)
    {
        _uploadDirectory = Path.Combine(environment.ContentRootPath, "uploads");
        if (!Directory.Exists(_uploadDirectory))
        {
            Directory.CreateDirectory(_uploadDirectory);
        }
    }

    public async Task<(string storagePath, long size)> SaveFileAsync(string fileName, Stream stream, CancellationToken cancellationToken = default)
    {
        var safeExt = Path.GetExtension(fileName);
        var uniqueName = $"{Guid.NewGuid():N}{safeExt}";
        var fullPath = Path.Combine(_uploadDirectory, uniqueName);

        await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fileStream, cancellationToken);
        var size = fileStream.Length;

        return (uniqueName, size);
    }

    public Task<Stream?> GetFileStreamAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_uploadDirectory, storagePath);
        if (!File.Exists(fullPath)) return Task.FromResult<Stream?>(null);

        Stream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult<Stream?>(fileStream);
    }

    public Task<bool> DeleteFileAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_uploadDirectory, storagePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}
