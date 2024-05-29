namespace Fanex.ExternalAccount.HeadlineList.Services
{
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using System.IO;
    using Models;
    using Resources;
    using NPOI.SS.Util;
    using System.Linq;
    public interface IHeadlineListExcel
    {
        MemoryStream ExportHeadlinesExcel(SearchHeadlineResult data);
    }

    public class HeadlineListExcel : IHeadlineListExcel
    {
        private const byte ClosedByAdmin = 1;
        private const byte ClosedByUpline = 2;
        private const byte ClosedByArchive = 4;
      
        private ConfigCell configCell;

        public MemoryStream ExportHeadlinesExcel(SearchHeadlineResult data)
        {
            var result = new MemoryStream();
            var sheetName = "HeadlineList";
            HSSFWorkbook workbook = new HSSFWorkbook();
            configCell = new ConfigCell(workbook);
            var worksheet = workbook.CreateSheet(sheetName);
            
            
            CreateHeaderHeadlinesExcel(workbook, worksheet);
            CreateBodyHeadlinesExcel(workbook, worksheet, data);
            
            workbook.Write(result);
            return result;
        }

        private void CreateHeaderHeadlinesExcel(HSSFWorkbook workbook, ISheet worksheet)
        {
            var currentRow = 0;

            var headerRow = worksheet.CreateRow(currentRow);
            var headerStyle = configCell.HeaderStyle(workbook);
            var headerContents = new[] { "No.", "Headline", "Username", "Status" };
            var columnWidths = new[] { 8, 27, 27, 10 };

            for (var columnIndex = 0; columnIndex < headerContents.Length; columnIndex++)
            {
                var currentCell = headerRow.CreateCell(columnIndex);
                currentCell.SetCellValue(headerContents[columnIndex]);
                currentCell.CellStyle = headerStyle;
                headerRow.Sheet.SetColumnWidth(columnIndex, columnWidths[columnIndex] * 256);
            }
        }
        
        private void CreateBodyHeadlinesExcel(HSSFWorkbook workbook, ISheet worksheet, SearchHeadlineResult data)
        {
            var currentRow = 1;
            if (data.TotalHeadlines == 0)
            {
                ExportEmptyReport(workbook, worksheet, currentRow);
                return;
            }

            var groups = data.GroupHeadlines.ToList();

            for (int i = 0; i < data.TotalGroups; i++)
            {
                var groupHeadlines = groups[i];
                var numberHeadlines = groupHeadlines.HeadlineEntities.Count;

                for (int j = 0; j < numberHeadlines; j++)
                {
                    var row = worksheet.CreateRow(currentRow);
                    var headline = groups[i].HeadlineEntities[j];

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
                    column.CellStyle = configCell.NumberCellStyle;
                    
                    currentColumn = 1;
                    column = row.CreateCell(currentColumn);
                    column.SetCellValue(headline.GroupName);
                    column.CellStyle = configCell.HeadlineCellStyle;
                    
                    currentColumn = 2;
                    column = row.CreateCell(currentColumn);
                    column.SetCellValue(headline.Username);
                    column.CellStyle = UsernameCellStyle(headline);
                    
                    currentColumn = 3;
                    column = row.CreateCell(currentColumn);
                    column.SetCellValue(headline.Status);
                    column.CellStyle = StatusCellStyle(headline);
                    row.GetCell(4).SetBlank();
                    currentRow++;
                }
            }
        }

        private void ExportEmptyReport(HSSFWorkbook workbook, ISheet worksheet, int currentRow)
        {
            var textStyle = workbook.CreateCellStyle();
            var font = workbook.CreateFont();
            font.FontName = configCell.DefaultFontName;
            font.FontHeightInPoints = configCell.DefaultFontHeight;
            font.Color = HSSFColor.Red.Index;
            font.IsBold = true;
            textStyle.SetFont(font);

            var row = worksheet.CreateRow(currentRow);
            var column = row.CreateCell(0);
            column.SetCellValue(ViewLang.NoData);
            column.CellStyle = textStyle;
        }
        
        private void MergedRegion(ISheet worksheet, int firstCellIndex, int lastCellIndex)
        {
            if (lastCellIndex <= firstCellIndex) return;
            var mergedNumberCol = new CellRangeAddress(firstCellIndex, lastCellIndex, 0, 0);
            var mergedHeadlineCol = new CellRangeAddress(firstCellIndex, lastCellIndex, 1, 1);
            worksheet.AddMergedRegion(mergedNumberCol);
            worksheet.AddMergedRegion(mergedHeadlineCol);
        }
        
        private ICellStyle UsernameCellStyle(HeadlineEntity headline)
        {
            if (!headline.IsEnable)
            {
                return configCell.DisableCellStyle;
            }

            if (headline.IsClosed)
            {
                switch (headline.ClosedReason)
                {
                    case ClosedByAdmin:
                        return configCell.ClosedByAdminCellStyle;
                    case ClosedByUpline:
                        return configCell.ClosedByUplineCellStyle;
                    case ClosedByArchive:
                        return configCell.ClosedByArchiveCellStyle;
                    default:
                        return configCell.DefaultCellStyle;
                }
            }

            if (headline.IsSuspend)
            {
                return configCell.SuspendedCellStyle;
            }
            
            return configCell.DefaultCellStyle;
        }
        
        private ICellStyle StatusCellStyle(HeadlineEntity headline)
        {
            if (!headline.IsEnable)
            {
                return configCell.DisableCellStyle_Center;
            }

            if (headline.IsClosed)
            {
                switch (headline.ClosedReason)
                {
                    case ClosedByAdmin:
                        return configCell.ClosedByAdminCellStyle_Center;
                    case ClosedByUpline:
                        return configCell.ClosedByUplineCellStyle_Center;
                    case ClosedByArchive:
                        return configCell.ClosedByArchiveCellStyle_Center;
                    default:
                        return configCell.DefaultCellStyle_Center;
                }
            }

            if (headline.IsSuspend)
            {
                return configCell.SuspendedCellStyle_Center;
            }
            
            return configCell.DefaultCellStyle_Center;
        }
        
    }
}