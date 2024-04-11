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

        public MemoryStream ExportHeadlinesExcel(SearchHeadlineResult data)
        {
            var result = new MemoryStream();
            var sheetName = "HeadlineList";
            HSSFWorkbook workbook = new HSSFWorkbook();
            var worksheet = workbook.CreateSheet(sheetName);

            CreateBodyHeadlinesExcel(workbook, worksheet, data);
            CreateHeaderHeadlinesExcel(workbook, worksheet);

            workbook.Write(result);
            return result;
        }

        private void CreateHeaderHeadlinesExcel(HSSFWorkbook workbook, ISheet worksheet)
        {
            var currentRow = 0;

            currentRow++;
            var headerRow = worksheet.CreateRow(currentRow);
            var columnWidths = new [] { 8, 27, 27, 10 };
            for (var i = 0; i <= columnWidths.Length - 1; i++)
            {
                headerRow.Sheet.SetColumnWidth(i, columnWidths[i] * 256);
            }

            var headerStyle = HeaderStyle(workbook);
            var currentColumn = 0;
            var currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue("No.");
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue("Headline");
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue("UserName");
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue("Status");
            currentCell.CellStyle = headerStyle;
            currentColumn++;
        }


        private ICellStyle CellByStatus(HSSFWorkbook workbook, HeadlineEntity headline, HorizontalAlignment horizontalAlignment)
        {
            if (!headline.IsEnable)
            {
                var disableCellStyle = DisableCellStyle(workbook);
                disableCellStyle.Alignment = horizontalAlignment;
                return disableCellStyle;
            }

            if (headline.IsClosed)
            {
                var closeCellStyle = CloseCellStyle(workbook, headline.ClosedReason);
                closeCellStyle.Alignment = horizontalAlignment;
                return closeCellStyle;
            }

            if (headline.IsSuspend)
            {
                var suspendedCellStyle = SuspendedCellStyle(workbook);
                suspendedCellStyle.Alignment = horizontalAlignment;
                return suspendedCellStyle;
            }

            var defaultStyle = DefaultStyle(workbook);
            defaultStyle.Alignment = horizontalAlignment;
            
            return defaultStyle;
        }

        private void CreateBodyHeadlinesExcel(HSSFWorkbook workbook, ISheet worksheet, SearchHeadlineResult data)
        {
            var currentRow = 2;
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

        private void MergedRegion(ISheet worksheet, int firstCellIndex, int lastCellIndex)
        {
            if (lastCellIndex > firstCellIndex)
            {
                var mergedNumberCol = new CellRangeAddress(firstCellIndex, lastCellIndex, 0, 0);
                var mergedHeadlineCol = new CellRangeAddress(firstCellIndex, lastCellIndex, 1, 1);
                worksheet.AddMergedRegion(mergedNumberCol);
                worksheet.AddMergedRegion(mergedHeadlineCol);
            }
        }

        private void ExportEmptyReport(HSSFWorkbook workbook, ISheet worksheet, int currentRow)
        {
            var textStyle = workbook.CreateCellStyle();
            var font = workbook.CreateFont();
            font.FontName = "Tahoma";
            font.FontHeightInPoints = 9;
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
            var headerStyle = workbook.CreateCellStyle();

            var font = workbook.CreateFont();
            font.FontName = "Tahoma";
            font.FontHeightInPoints = 9;
            font.Color = HSSFColor.Black.Index;
            font.IsBold = true;

            var palette = workbook.GetCustomPalette();
            var colorIndex = palette.FindSimilarColor(131, 202, 255).Indexed;
           // var borderColorIndex = palette.FindSimilarColor(128, 100, 162).Indexed;
            var borderColorIndex = HSSFColor.Black.Index;
            headerStyle.SetFont(font);
            headerStyle.Alignment = HorizontalAlignment.Center;
            headerStyle.VerticalAlignment = VerticalAlignment.Center;

            headerStyle.BottomBorderColor = borderColorIndex;
            headerStyle.BorderBottom = BorderStyle.Thin;
            headerStyle.TopBorderColor = borderColorIndex;
            headerStyle.BorderTop = BorderStyle.Thin;
            headerStyle.LeftBorderColor = borderColorIndex;
            headerStyle.BorderLeft = BorderStyle.Thin;
            headerStyle.RightBorderColor = borderColorIndex;
            headerStyle.BorderRight = BorderStyle.Thin;
            headerStyle.FillForegroundColor = colorIndex;
            headerStyle.FillPattern = FillPattern.SolidForeground;

            return headerStyle;
        }

        private ICellStyle DefaultStyle(HSSFWorkbook workbook)
        {
            var reportStyle = workbook.CreateCellStyle();
            var palette = workbook.GetCustomPalette();
           // var borderColorIndex = palette.FindSimilarColor(128, 100, 162).Indexed;
            var borderColorIndex = HSSFColor.Black.Index;
            var mediumBlackNormal = workbook.CreateFont();
            mediumBlackNormal.FontName = "Tahoma";
            mediumBlackNormal.FontHeightInPoints = 9;
            mediumBlackNormal.Color = HSSFColor.Black.Index;
            reportStyle.SetFont(mediumBlackNormal);

            reportStyle.BottomBorderColor = borderColorIndex;
            reportStyle.VerticalAlignment = VerticalAlignment.Center;
            reportStyle.BorderBottom = BorderStyle.Thin;
            reportStyle.TopBorderColor = borderColorIndex;
            reportStyle.BorderTop = BorderStyle.Thin;
            reportStyle.LeftBorderColor = borderColorIndex;
            reportStyle.BorderLeft = BorderStyle.Thin;
            reportStyle.RightBorderColor = borderColorIndex;
            reportStyle.BorderRight = BorderStyle.Thin;
            reportStyle.WrapText = true;
            
            return reportStyle;
        }

        private ICellStyle DisableCellStyle(HSSFWorkbook workbook)
        {
            var palette = workbook.GetCustomPalette();
            var disableColor = palette.FindSimilarColor(148, 189, 94).Indexed;
            var leftDisableColorTextStyle = workbook.CreateCellStyle();
            leftDisableColorTextStyle.CloneStyleFrom(DefaultStyle(workbook));
            leftDisableColorTextStyle.FillBackgroundColor = disableColor;
            leftDisableColorTextStyle.FillPattern = FillPattern.SolidForeground;
            return leftDisableColorTextStyle;
        }

        private ICellStyle CloseCellStyle(HSSFWorkbook workbook, int closedReason)
        {
            var palette = workbook.GetCustomPalette();
            short closeColor;
            switch (closedReason)
            {
                case ClosedByAdmin:
                    closeColor = palette.FindSimilarColor(153, 153, 255).Indexed;
                    break;
                case ClosedByUpline:
                    closeColor = palette.FindSimilarColor(255, 102, 0).Indexed;
                    break;
                case ClosedByArchive:
                    closeColor = palette.FindSimilarColor(150, 150, 150).Indexed;
                    break;
                default:
                    closeColor = HSSFColor.White.Index;
                    break;
            }

            var leftDisableColorTextStyle = workbook.CreateCellStyle();
            leftDisableColorTextStyle.CloneStyleFrom(DefaultStyle(workbook));
            leftDisableColorTextStyle.FillForegroundColor = closeColor;
            leftDisableColorTextStyle.FillPattern = FillPattern.SolidForeground;
            return leftDisableColorTextStyle;
        }

        private ICellStyle SuspendedCellStyle(HSSFWorkbook workbook)
        {
            var palette = workbook.GetCustomPalette();
            var suspendedColor = palette.FindSimilarColor(255, 204, 153).Indexed;
            var leftDisableColorTextStyle = workbook.CreateCellStyle();
            leftDisableColorTextStyle.CloneStyleFrom(DefaultStyle(workbook));
            leftDisableColorTextStyle.FillForegroundColor = suspendedColor;
            leftDisableColorTextStyle.FillPattern = FillPattern.SolidForeground;
            return leftDisableColorTextStyle;
        }
    }
}