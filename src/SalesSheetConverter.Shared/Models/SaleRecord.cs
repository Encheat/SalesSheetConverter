namespace SalesSheetConverter.Shared.Models;

public class SaleRecord
{
    public DateOnly? Date { get; set; }

    public string Item { get; set; } = "";

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }
}