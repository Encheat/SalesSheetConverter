namespace SalesSheetConverter.Shared.Models;

public class ExtractionResult
{
    public string? DocumentType { get; set; }
    public double? Confidence { get; set; }
    public string? Notes { get; set; }
    public string? FileName { get; set; }
    public List<string> CsvLines { get; set; } = [];
}
