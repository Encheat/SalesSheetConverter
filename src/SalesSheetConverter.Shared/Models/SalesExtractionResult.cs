namespace SalesSheetConverter.Shared.Models;

public class SalesExtractionResult
{
    public string? DocumentType { get; set; }
    public string? Confidence { get; set; }
    public string? Notes { get; set; } //May not need this, I like the above two fields more
    public List<Row> ColumnValueEntries { get; set; } = [];
}

public class Row
{
    public Dictionary<string, string?> ColumnValue { get; set; } = [];
}