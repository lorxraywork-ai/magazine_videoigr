using ClosedXML.Excel;

namespace VideoGameStoreSystem.Web.Infrastructure;

public static class ExcelExportHelper
{
    public static byte[] BuildWorkbook(
        string title,
        IReadOnlyList<string> headers,
        IEnumerable<IReadOnlyList<object?>> rows)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Данные");

        worksheet.Cell(1, 1).Value = title;
        worksheet.Range(1, 1, 1, headers.Count).Merge();
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 14;

        for (var i = 0; i < headers.Count; i++)
        {
            var headerCell = worksheet.Cell(3, i + 1);
            headerCell.Value = headers[i];
            headerCell.Style.Font.Bold = true;
            headerCell.Style.Fill.BackgroundColor = XLColor.LightBlue;
            headerCell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }

        var rowIndex = 4;
        foreach (var row in rows)
        {
            for (var col = 0; col < row.Count; col++)
            {
                worksheet.Cell(rowIndex, col + 1).Value = row[col]?.ToString() ?? string.Empty;
            }

            rowIndex++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
