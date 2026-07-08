using System.Text;
using SalesSheetConverter.Shared.Models;

namespace SalesSheetConverter.Functions.Services;

public class CsvExportService
{
    public byte[] CreateCsv(ExtractionResult rawCsv)
    {
        //TODO: verify csv? Not sure how, but I don't trust the AI to work perfectly every time.
        var csv = string.Join(Environment.NewLine, rawCsv.CsvLines);

        return Encoding.UTF8.GetBytes(csv);
    }
}