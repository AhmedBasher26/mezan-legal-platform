namespace Mezan.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<(string storagePath, long size)> SaveFileAsync(string fileName, Stream stream, CancellationToken cancellationToken = default);
    Task<Stream?> GetFileStreamAsync(string storagePath, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string storagePath, CancellationToken cancellationToken = default);
}
