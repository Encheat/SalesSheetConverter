namespace SalesSheetConverter.Web.Models;

public sealed class FileTransferRequest
{
    public Guid SessionId { get; init; }
    public List<UploadedFile> Files { get; init; } = [];

    public FileTransferRequest(List<UploadedFile> files)
    {
        SessionId = Guid.NewGuid();
        Files = files;
    }
}