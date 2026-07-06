namespace SalesSheetConverter.Shared.Models;

public class SalesExtractionResult
{
    public string? DocumentType { get; set; }
    public string? Confidence { get; set; }
    public string? Notes { get; set; } //May not need this, I like the above two fields more
    public List<SaleRecord> Sales { get; set; } = [];

}

public class SaleRecord
{
    public Dictionary<string, string?> Fields { get; set; } = [];
}