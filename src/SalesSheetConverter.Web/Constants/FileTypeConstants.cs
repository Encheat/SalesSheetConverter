namespace SalesSheetConverter.Web.Constants;

public static class FileTypeConstants
{
    public static IReadOnlyList<string> AllowedFileTypes { get; } =
    [
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/gif",
        "image/bmp",
        "image/webp",
        "image/tiff",
        "image/svg+xml"
    ];
}