namespace SalesSheetConverter.Shared.Models;

public sealed class ImageDetails
{
    public required Stream Stream { get; init; }

    public required string ContentType { get; init; }
}