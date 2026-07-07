using System.Text;
using SalesSheetConverter.Shared.Models;

namespace SalesSheetConverter.Functions.Services;

public class CsvExportService
{
    public byte[] CreateCsv(ExtractionResult rawCsv)
    {
        var csv = string.Join(Environment.NewLine, rawCsv.CsvLines);

        return Encoding.UTF8.GetBytes(csv);
    }
}