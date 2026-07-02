using System.Globalization;
using System.Text;
using SalesSheetConverter.Shared.Models;

namespace SalesSheetConverter.Functions.Services;

public class CsvExportService
{
    public byte[] CreateCsv(IEnumerable<SaleRecord> sales)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Date,Item,Quantity,Unit Price,Total Price");

        foreach (var sale in sales)
        {
            sb.AppendLine(
                string.Join(",",
                    sale.Date?.ToString("yyyy-MM-dd") ?? "",
                    Escape(sale.Item),
                    sale.Quantity,
                    sale.UnitPrice.ToString(CultureInfo.InvariantCulture),
                    sale.TotalPrice.ToString(CultureInfo.InvariantCulture)));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string Escape(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}