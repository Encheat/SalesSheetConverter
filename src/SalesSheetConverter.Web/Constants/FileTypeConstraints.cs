namespace SalesSheetConverter.Web.Constants;

public static class FileTypeConstraints
{
    public static IReadOnlyList<string> AllowedMimeTypes { get; } =
    [
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/bmp",
        "image/webp",
        "image/tiff"
    ];

    public static async Task<bool> IsAllowedFileHeader(Stream memoryStream)
    {
        bool result = false;
        memoryStream.Position = 0;
        byte[] header = new byte[8];
        await memoryStream.ReadExactlyAsync(header);

        byte[][] acceptableHeaders =
        [
            [0xFF, 0xD8, 0xFF],         //jpg/jpeg
            [0x89, 0x50, 0x4E, 0x47],   //png
            [0x42, 0x4D],               //bmp/dib
            [0x52, 0x49, 0x46, 0x46],   //webp
            [0x49, 0x49, 0x2A, 0x00],   //tiff (little-endian)
            [0x4D, 0x4D, 0x00, 0x2A],   //tiff (big-endian)
        ];
        
        foreach(byte[] byteHeader in acceptableHeaders)
        {
            byte[] sizedHeader = header.Take(byteHeader.Length).ToArray(); //reduce to exact size
            if (byteHeader.SequenceEqual(sizedHeader))
            {
                result = true;
            }
        }

        memoryStream.Position = 0;
        return result;
    }
}