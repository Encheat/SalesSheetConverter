using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SalesSheetConverter.Web.Models;

namespace SalesSheetConverter.Web.Services;

public interface IFileTransferService
{
    Task StoreAsync(FileTransferRequest request);
    Task<FileTransferRequest?> TryGetAsync(Guid sessionId);
    Task RemoveAsync(Guid sessionId);
}

public class FileTransferService : IFileTransferService
{
    private const string StoragePrefix = "file-transfer";
    private readonly ProtectedSessionStorage _sessionStorage;

    public FileTransferService(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public async Task StoreAsync(FileTransferRequest request)
    {
        await _sessionStorage.SetAsync($"{StoragePrefix}:{request.SessionId}", request);
    }

    public async Task<FileTransferRequest?> TryGetAsync(Guid id)
    {
        var result = await _sessionStorage.GetAsync<FileTransferRequest>($"{StoragePrefix}:{id}");

        return result.Success && result.Value is not null ? result.Value : null;
    }

    public async Task RemoveAsync(Guid sessionId)
    {
        await _sessionStorage.DeleteAsync($"{StoragePrefix}:{sessionId}");
    }
}