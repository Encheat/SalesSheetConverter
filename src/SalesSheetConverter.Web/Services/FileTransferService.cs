namespace SalesSheetConverter.Web.Services;

public class UploadedFile
{
    public string Name { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[] Bytes { get; set; } = Array.Empty<byte>();
}

public class FileTransferService
{
    public List<UploadedFile> Files { get; set; } = [];
}