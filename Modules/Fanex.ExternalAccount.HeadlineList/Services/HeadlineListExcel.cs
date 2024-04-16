namespace Fanex.ExternalAccount.HeadlineList.Services
{
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using System.IO;
    using Models;
    using Resources;
    using NPOI.SS.Util;

    public interface IHeadlineListExcel
    {
        MemoryStream ExportHeadlinesExcel(SearchHeadlineResult data);
    }

    public class HeadlineListExcel : IHeadlineListExcel
    {
        private const byte ClosedByAdmin = 1;
        private const byte ClosedByUpline = 2;
        private const byte ClosedByArchive = 4;
        private const string DefaultFontName = "Tahoma";
        private const byte DefaultFontHeight = 9;
        private short SuspendedColor;
        private short ClosedByAdminColor;
        private short ClosedByUplineColor;
        private short ClosedByArchiveColor;
        private short DisableColor;

        public MemoryStream ExportHeadlinesExcel(SearchHeadlineResult data)
        {
            var result = new MemoryStream();
            var sheetName = "HeadlineList";
            HSSFWorkbook workbook = new HSSFWorkbook();
            var worksheet = workbook.CreateSheet(sheetName);

            var palette = workbook.GetCustomPalette();
            SuspendedColor = palette.FindSimilarColor(255, 204, 153).Indexed;
            ClosedByAdminColor = palette.FindSimilarColor(153, 153, 255).Indexed;
            ClosedByUplineColor = palette.FindSimilarColor(255, 102, 0).Indexed;
            ClosedByArchiveColor = palette.FindSimilarColor(150, 150, 150).Indexed;
            DisableColor = palette.FindSimilarColor(148, 189, 94).Indexed;

            CreateBodyHeadlinesExcel(workbook, worksheet, data);
            CreateHeaderHeadlinesExcel(workbook, worksheet);

            workbook.Write(result);
            return result;
        }

        private void CreateHeaderHeadlinesExcel(HSSFWorkbook workbook, ISheet worksheet)
        {
            var currentRow = 0;

            var headerRow = worksheet.CreateRow(currentRow);
            var headerStyle = HeaderStyle(workbook);
            var headerContents = new[] { "No.", "Headline", "Username", "Status" };
            var columnWidths = new[] { 8, 27, 27, 10 };
            for (var columnIndex = 0; columnIndex <= headerContents.Length - 1; columnIndex++)
            {
                var currentCell = headerRow.CreateCell(columnIndex);
                currentCell.SetCellValue(headerContents[columnIndex]);
                currentCell.CellStyle = headerStyle;
                headerRow.Sheet.SetColumnWidth(columnIndex, columnWidths[columnIndex] * 256);
            }
        }


        private ICellStyle SuspendedCellStyle(HSSFWorkbook workbook, HorizontalAlignment horizontalAlignment)
        {
            var suspendedCellStyle = FillColor(workbook, SuspendedColor);
            suspendedCellStyle.Alignment = horizontalAlignment;
            return suspendedCellStyle;
        }

        private ICellStyle CellByStatus(HSSFWorkbook workbook, HeadlineEntity headline, HorizontalAlignment horizontalAlignment)
        {
            if (!headline.IsEnable)
            {
                return DisableCellStyle(workbook, horizontalAlignment);
            }

            if (headline.IsClosed)
            {
                return CloseCellStyle(workbook, horizontalAlignment, headline.ClosedReason);
            }

            if (headline.IsSuspend)
            {
                return SuspendedCellStyle(workbook, horizontalAlignment);
            }

            var defaultStyle = DefaultStyle(workbook);
            defaultStyle.Alignment = horizontalAlignment;

            return defaultStyle;
        }

        private void CreateBodyHeadlinesExcel(HSSFWorkbook workbook, ISheet worksheet, SearchHeadlineResult data)
        {
            var currentRow = 1;
            if (data.TotalHeadlines == 0)
            {
                ExportEmptyReport(workbook, worksheet, currentRow);
                return;
            }

            for (int i = 0; i < data.TotalGroups; i++)
            {
                var groupHeadlines = data.GroupHeadlines[i];
                var numberHeadlines = groupHeadlines.HeadlineEntities.Count;

                for (int j = 0; j < numberHeadlines; j++)
                {
                    var row = worksheet.CreateRow(currentRow);
                    var headline = data.GroupHeadlines[i].HeadlineEntities[j];

                    if (j == 0)
                    {
                        var firstCellIndex = currentRow;
                        var lastCellIndex = currentRow + numberHeadlines - 1;
                        MergedRegion(worksheet, firstCellIndex, lastCellIndex);
                    }

                    var currentColumn = 0;
                    var column = row.CreateCell(4);
                    column = row.CreateCell(currentColumn);
                    column.SetCellValue(i + 1);
                    column.CellStyle = CellByStatus(workbook, headline, HorizontalAlignment.Center);

                    currentColumn = 1;
                    column = row.CreateCell(currentColumn);
                    column.SetCellValue(headline.GroupName);
                    column.CellStyle = CellByStatus(workbook, headline, HorizontalAlignment.Left);

                    currentColumn = 2;
                    column = row.CreateCell(currentColumn);
                    column.SetCellValue(headline.Username);
                    column.CellStyle = CellByStatus(workbook, headline, HorizontalAlignment.Left);

                    currentColumn = 3;
                    column = row.CreateCell(currentColumn);
                    column.SetCellValue(headline.Status);
                    column.CellStyle = CellByStatus(workbook, headline, HorizontalAlignment.Center);
                    row.GetCell(4).SetBlank();
                    currentRow++;
                }
            }
        }

        private void ExportEmptyReport(HSSFWorkbook workbook, ISheet worksheet, int currentRow)
        {
            var textStyle = workbook.CreateCellStyle();
            var font = workbook.CreateFont();
            font.FontName = DefaultFontName;
            font.FontHeightInPoints = DefaultFontHeight;
            font.Color = HSSFColor.Red.Index;
            font.IsBold = true;
            textStyle.SetFont(font);

            var row = worksheet.CreateRow(currentRow);
            var column = row.CreateCell(0);
            column.SetCellValue(ViewLang.NoData);
            column.CellStyle = textStyle;
        }

        private ICellStyle HeaderStyle(HSSFWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();
            var headerFont = DefaultFontStyle(workbook);
            headerFont.IsBold = true;
            style.SetFont(headerFont);


            var palette = workbook.GetCustomPalette();
            var colorIndex = palette.FindSimilarColor(131, 202, 255).Indexed;
            
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;

            DefaultBorderStyle(ref style);

            style.FillForegroundColor = colorIndex;
            style.FillPattern = FillPattern.SolidForeground;

            return style;
        }

        private ICellStyle DefaultStyle(HSSFWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();

            style.SetFont(DefaultFontStyle(workbook));

            style.VerticalAlignment = VerticalAlignment.Center;

            DefaultBorderStyle(ref style);

            style.WrapText = true;

            return style;
        }

        private void DefaultBorderStyle(ref ICellStyle style)
        {
            var borderColorIndex = HSSFColor.Black.Index;
            style.BottomBorderColor = borderColorIndex;
            style.BorderBottom = BorderStyle.Thin;
            style.TopBorderColor = borderColorIndex;
            style.BorderTop = BorderStyle.Thin;
            style.LeftBorderColor = borderColorIndex;
            style.BorderLeft = BorderStyle.Thin;
            style.RightBorderColor = borderColorIndex;
            style.BorderRight = BorderStyle.Thin;
        }

        private IFont DefaultFontStyle(HSSFWorkbook workbook)
        {
            var font = workbook.CreateFont();
            font.FontName = DefaultFontName;
            font.FontHeightInPoints = DefaultFontHeight;
            font.Color = HSSFColor.Black.Index;
            return font;
        }


        private ICellStyle FillColor(HSSFWorkbook workbook, short color)
        {
            var cellStyle = workbook.CreateCellStyle();
            cellStyle.CloneStyleFrom(DefaultStyle(workbook));
            cellStyle.FillForegroundColor = color;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            return cellStyle;
        }

        private void MergedRegion(ISheet worksheet, int firstCellIndex, int lastCellIndex)
        {
            if (lastCellIndex <= firstCellIndex) return;
            var mergedNumberCol = new CellRangeAddress(firstCellIndex, lastCellIndex, 0, 0);
            var mergedHeadlineCol = new CellRangeAddress(firstCellIndex, lastCellIndex, 1, 1);
            worksheet.AddMergedRegion(mergedNumberCol);
            worksheet.AddMergedRegion(mergedHeadlineCol);
        }

        private ICellStyle DisableCellStyle(HSSFWorkbook workbook, HorizontalAlignment horizontalAlignment)
        {
            var disableCellStyle = FillColor(workbook, DisableColor);
            disableCellStyle.Alignment = horizontalAlignment;
            return disableCellStyle;
        }

        private ICellStyle CloseCellStyle(HSSFWorkbook workbook, HorizontalAlignment horizontalAlignment, byte closedReason)
        {
            short closeColor;
            switch (closedReason)
            {
                case ClosedByAdmin:
                    closeColor = ClosedByAdminColor;
                    break;
                case ClosedByUpline:
                    closeColor = ClosedByUplineColor;
                    break;
                case ClosedByArchive:
                    closeColor = ClosedByArchiveColor;
                    break;
                default:
                    closeColor = HSSFColor.White.Index;
                    break;
            }

            var closeCellStyle = FillColor(workbook, closeColor);
            closeCellStyle.Alignment = horizontalAlignment;
            return closeCellStyle;
        }
    }
}