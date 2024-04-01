namespace Fanex.ExternalAccount.HeadlineList.Services
{
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using System.IO;
    using Fanex.ExternalAccount.HeadlineList.Models;
    using Fanex.ExternalAccount.HeadlineList.Resources;
    using NPOI.SS.Util;

    public interface IHeadlineListExcel
    {
        MemoryStream ExportHeadlinesExcel(SearchHeadlineResult data);
    }

    public class HeadlineListExcel : IHeadlineListExcel
    {
        public MemoryStream ExportHeadlinesExcel(SearchHeadlineResult data)
        {
            var result = new MemoryStream();
            var sheetName = "HeadlinesSheet";
            HSSFWorkbook workbook = new HSSFWorkbook();
            var worksheet = workbook.CreateSheet(sheetName);

            CreateHeaderHeadlinesExcel(workbook, worksheet);
            CreateBodyHeadlinesExcel(workbook, worksheet, data);

            workbook.Write(result);
            return result;
        }

        private void CreateHeaderHeadlinesExcel(HSSFWorkbook workbook, ISheet worksheet)
        {
            var currentRow = 0;
            // create title
            // var titleRow = worksheet.CreateRow(currentRow);
            // var titleCell = titleRow.CreateCell(0);
            // titleCell.SetCellValue("Headlines Report");
            // titleCell.CellStyle = TitleStyle(workbook);

            currentRow++;
            var headerRow = worksheet.CreateRow(currentRow);
            var columnWidths = new int[] { 10, 25, 25, 10 };
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


        private ICellStyle CellByStatus(HSSFWorkbook workbook, string status, HorizontalAlignment horizontalAlignment)
        {
            switch (status)
            {
                case "Closed":
                    var closeCellStyle = CloseCellStyle(workbook);
                    closeCellStyle.Alignment = horizontalAlignment;
                    return closeCellStyle;
                case "Suspended":
                    var suspendedCellStyle = SuspendedCellStyle(workbook);
                    suspendedCellStyle.Alignment = horizontalAlignment;
                    return suspendedCellStyle;
                case "Disabled":
                    var disableCellStyle = DisableCellStyle(workbook);
                    disableCellStyle.Alignment = horizontalAlignment;
                    return disableCellStyle;
                default:
                    var defaultStyle = DefaultStyle(workbook);
                    defaultStyle.Alignment = horizontalAlignment;
                    return defaultStyle;
            }
        }

        private void CreateBodyHeadlinesExcel(HSSFWorkbook workbook, ISheet worksheet, SearchHeadlineResult data)
        {
            // //tao màu
            // var palette = workbook.GetCustomPalette();
            // // var suspendedColor = palette.FindSimilarColor(255, 204, 153).Indexed;
            // //  var closeColor =  palette.FindSimilarColor(153, 153, 255).Indexed; 
            // var disableColor = palette.FindSimilarColor(148, 189, 94).Indexed;
            //
            // // tạo textstyle chuẩn
            // var defaultStyle = DefaultStyle(workbook);
            //
            // // centerTextStyle
            // // var centerTextStyle = workbook.CreateCellStyle();
            // // centerTextStyle.CloneStyleFrom(defaultStyle);
            // // centerTextStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            //
            // // leftTextStyle
            
            var currentRow = 2;
            // var totalHeadlines = data.TotalHeadlines;
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
                        // if (lastCellIndex > firstCellIndex)
                        // {
                        //     var mergedNumberCol = new CellRangeAddress(firstCellIndex, lastCellIndex, 0, 0);
                        //     var mergedHeadlineCol = new CellRangeAddress(firstCellIndex, lastCellIndex, 1, 1);
                        //     worksheet.AddMergedRegion(mergedNumberCol);
                        //     worksheet.AddMergedRegion(mergedHeadlineCol);
                        // }
                    }

                    var currentColumn = 0;
                    var column = row.CreateCell(4);
                    column = row.CreateCell(currentColumn);
                    column.SetCellValue(i + 1);
                    column.CellStyle = CellByStatus(workbook, "", HorizontalAlignment.Center);

                    currentColumn = 1;
                    column = row.CreateCell(currentColumn);
                    column.SetCellValue(headline.GroupName);
                    column.CellStyle = CellByStatus(workbook, "", HorizontalAlignment.Left);

                    currentColumn = 2;
                    column = row.CreateCell(currentColumn);
                    column.SetCellValue(headline.Username);
                    column.CellStyle = CellByStatus(workbook, headline.Status, HorizontalAlignment.Left);

                    currentColumn = 3;
                    column = row.CreateCell(currentColumn);
                    column.SetCellValue(headline.Status);
                    column.CellStyle = CellByStatus(workbook, headline.Status, HorizontalAlignment.Center);
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

        private void ExportEmptyReport(
            HSSFWorkbook workbook,
            ISheet worksheet,
            int currentRow)
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

        private ICellStyle TitleStyle(HSSFWorkbook workbook)
        {
            var titleStyle = workbook.CreateCellStyle();

            var font = workbook.CreateFont();
            font.FontName = "Tahoma";
            font.FontHeightInPoints = 9;
            font.Color = HSSFColor.Black.Index;
            font.IsBold = true;

            titleStyle.Indention = 2;
            titleStyle.SetFont(font);
            return titleStyle;
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
            var borderColorIndex = palette.FindSimilarColor(128, 100, 162).Indexed;

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
            //var borderColorIndex = palette.FindSimilarColor(128, 100, 162).Indexed;
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

            reportStyle.FillPattern = FillPattern.SolidForeground;

            return reportStyle;
        }

        private ICellStyle DisableCellStyle(HSSFWorkbook workbook)
        {
            var palette = workbook.GetCustomPalette();
            var disableColor = palette.FindSimilarColor(148, 189, 94).Indexed;
            var leftDisableColorTextStyle = workbook.CreateCellStyle();
            leftDisableColorTextStyle.CloneStyleFrom(DefaultStyle(workbook));
            leftDisableColorTextStyle.FillBackgroundColor = disableColor;

            return leftDisableColorTextStyle;
        }

        private ICellStyle CloseCellStyle(HSSFWorkbook workbook)
        {
            var palette = workbook.GetCustomPalette();
            var closeColor = palette.FindSimilarColor(153, 153, 255).Indexed;
            var leftDisableColorTextStyle = workbook.CreateCellStyle();
            leftDisableColorTextStyle.CloneStyleFrom(DefaultStyle(workbook));
            leftDisableColorTextStyle.FillForegroundColor = closeColor;

            return leftDisableColorTextStyle;
        }

        private ICellStyle SuspendedCellStyle(HSSFWorkbook workbook)
        {
            var palette = workbook.GetCustomPalette();
            var suspendedColor = palette.FindSimilarColor(255, 204, 153).Indexed;
            var leftDisableColorTextStyle = workbook.CreateCellStyle();
            leftDisableColorTextStyle.CloneStyleFrom(DefaultStyle(workbook));
            leftDisableColorTextStyle.FillForegroundColor = suspendedColor;


            return leftDisableColorTextStyle;
        }
    }
}