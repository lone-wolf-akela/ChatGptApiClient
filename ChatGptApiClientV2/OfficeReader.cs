using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ChatGptApiClientV2;

public static partial class OfficeReader
{
    // Excel
    private static string? XlsxGetCellValue(WorkbookPart? workbookPart, CellType cell)
    {
        var cellValue = cell.CellValue?.InnerText;
        if (cellValue is not null && cell.DataType is not null && cell.DataType.Value == CellValues.SharedString)
        {
            return workbookPart?.SharedStringTablePart?.SharedStringTable
                .Elements<SharedStringItem>().ElementAt(int.Parse(cellValue)).InnerText;
        }

        return cellValue;
    }

    [GeneratedRegex("[A-Za-z]+")]
    private static partial Regex XlsxColumnNameMyRegex();

    private static string XlsxGetColumnName(string cellReference)
    {
        var regex = XlsxColumnNameMyRegex();
        var match = regex.Match(cellReference);
        return match.Value.ToUpperInvariant();
    }

    private static int XlsxGetColumnIndexFromName(string columnName)
    {
        var columnIndex = 0;
        var factor = 1;
        for (var i = columnName.Length - 1; i >= 0; i--)
        {
            columnIndex += (columnName[i] - 'A' + 1) * factor;
            factor *= 26;
        }

        return columnIndex - 1;
    }

    public static async Task<string> XlsxToText(string filename, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            var sb = new StringBuilder();

            using var document = SpreadsheetDocument.Open(filename, false);
            var workbookPart = document.WorkbookPart;
            var sheets = workbookPart?.Workbook.Descendants<Sheet>() ?? [];

            foreach (var sheet in sheets)
            {
                sb.AppendLine($"Sheet {sheet.Name}:");
                sb.AppendLine();

                var sheetId = sheet.Id;
                if (sheetId is null)
                {
                    continue;
                }

                var worksheetPart = workbookPart?.GetPartById(sheetId!) as WorksheetPart;
                foreach (var row in worksheetPart?.Worksheet.Descendants<Row>() ?? [])
                {
                    var cells = row.Descendants<Cell>().ToList();
                    var currentColumnIndex = 0;
                    foreach (var cell in cells)
                    {
                        var cellRef = cell.CellReference;
                        if (cellRef is null)
                        {
                            continue;
                        }

                        var columnIndex = XlsxGetColumnIndexFromName(XlsxGetColumnName(cellRef!));
                        for (; currentColumnIndex < columnIndex; currentColumnIndex++)
                        {
                            sb.Append(',');
                        }

                        var cellValue = XlsxGetCellValue(workbookPart, cell);
                        sb.Append($"\"{cellValue}\"");
                        sb.Append(',');
                        currentColumnIndex++;
                    }

                    sb.Length--; // remove the last comma
                    sb.AppendLine();
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }, cancellationToken);
    }

    // word
    public static async Task<string> DocxToText(string filename, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            using var doc = WordprocessingDocument.Open(filename, false);
            var sb = new StringBuilder();
            var body = doc.MainDocumentPart?.Document.Body;
            if (body is null)
            {
                return "";
            }

            foreach (var para in body.Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
            {
                foreach (var run in para.Elements<DocumentFormat.OpenXml.Wordprocessing.Run>())
                {
                    sb.Append(run.InnerText);
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }, cancellationToken);
    }

    // ppt
    public static async Task<string> PptxToText(string filename, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            using var ppt = PresentationDocument.Open(filename, false);
            var sb = new StringBuilder();
            var presentationPart = ppt.PresentationPart;
            var slideIdList = presentationPart?.Presentation.SlideIdList;
            var slideIndex = 1;
            foreach (var slideId in slideIdList?.ChildElements.OfType<SlideId>() ?? [])
            {
                var relationshipId = slideId.RelationshipId;
                if (relationshipId is null)
                {
                    continue;
                }

                var slidePart = presentationPart?.GetPartById(relationshipId!) as SlidePart;
                sb.AppendLine($"Slide {slideIndex++}:");
                var paragraphs = slidePart?.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>() ?? [];
                foreach (var paragraph in paragraphs)
                {
                    var texts = paragraph.Descendants<DocumentFormat.OpenXml.Drawing.Text>().Select(x => x.Text);
                    sb.Append(string.Join(" ", texts));
                    sb.AppendLine();
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }, cancellationToken);
    }
}