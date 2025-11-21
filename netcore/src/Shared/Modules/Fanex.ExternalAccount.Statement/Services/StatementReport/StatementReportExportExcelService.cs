namespace Fanex.ExternalAccount.Statement.Services.StatementReport
{
    using Fanex.ExternalAccount.Statement.Configurations;
    using Fanex.ExternalAccount.Statement.Criteria.StatementReport;
    using Fanex.ExternalAccount.Statement.Models;
    using Fanex.ExternalAccount.Statement.Models.StatementReport;
    using Fanex.ExternalAccount.Statement.Resources;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    public class StatementReportExportExcelService
    {
        private readonly StatementSection _statementSection;

        public StatementReportExportExcelService(StatementSection? statementSection = null)
        {
            _statementSection = statementSection ?? new StatementSection(null);
        }

        public MemoryStream ExportMemberStatement(MemberStatementCriteria criteria, StatementReportModel data)
        {
            var result = new MemoryStream();
            var sheetName = Labels.Statement;
            HSSFWorkbook workbook = new HSSFWorkbook();
            var worksheet = workbook.CreateSheet(sheetName);

            ExportHeaderMemberStatement(workbook, worksheet, criteria);
            ExportBodyMemberStatement(workbook, worksheet, data);

            workbook.Write(result);
            return result;
        }

        public MemoryStream ExportSMAStatement(SMAStatementCriteria criteria, StatementReportModel data)
        {
            var result = new MemoryStream();
            var sheetName = Labels.Statement;
            HSSFWorkbook workbook = new HSSFWorkbook();
            var worksheet = workbook.CreateSheet(sheetName);

            ExportHeaderSMAStatement(workbook, worksheet, criteria);
            ExportBodySMAStatement(workbook, worksheet, data);

            workbook.Write(result);
            return result;
        }

        public MemoryStream ExportTransferDetails(TransferDetailsCriteria criteria, StatementReportModel data)
        {
            var result = new MemoryStream();
            var sheetName = Labels.Statement;
            HSSFWorkbook workbook = new HSSFWorkbook();
            var worksheet = workbook.CreateSheet(sheetName);

            ExportHeaderTransferDetails(workbook, worksheet, criteria);
            ExportBodyTransferDetails(workbook, worksheet, data);

            workbook.Write(result);
            return result;
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
            headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            headerStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

            headerStyle.BottomBorderColor = borderColorIndex;
            headerStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            headerStyle.TopBorderColor = borderColorIndex;
            headerStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            headerStyle.LeftBorderColor = borderColorIndex;
            headerStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            headerStyle.RightBorderColor = borderColorIndex;
            headerStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            headerStyle.FillForegroundColor = colorIndex;
            headerStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;

            return headerStyle;
        }

        private ICellStyle ReportStyle(HSSFWorkbook workbook)
        {
            var reportStyle = workbook.CreateCellStyle();
            var palette = workbook.GetCustomPalette();
            var borderColorIndex = palette.FindSimilarColor(128, 100, 162).Indexed;

            reportStyle.BottomBorderColor = borderColorIndex;
            reportStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            reportStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            reportStyle.TopBorderColor = borderColorIndex;
            reportStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            reportStyle.LeftBorderColor = borderColorIndex;
            reportStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            reportStyle.RightBorderColor = borderColorIndex;
            reportStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            reportStyle.WrapText = true;
            return reportStyle;
        }

        private void ExportHeaderMemberStatement(
            HSSFWorkbook workbook,
            ISheet worksheet,
            MemberStatementCriteria criteria)
        {
            var currentRow = 0;
            var titleRow = worksheet.CreateRow(currentRow);
            var titleCell = titleRow.CreateCell(0);
            titleCell.SetCellValue(Labels.Statement + " (" + criteria.Username + "): " + criteria.FromDate.ToString("MM/dd/yyyy") +
                "-->" + criteria.ToDate.ToString("MM/dd/yyyy"));
            titleCell.CellStyle = TitleStyle(workbook);
            currentRow++;

            var headerRow = worksheet.CreateRow(currentRow);
            var columnWidths = new int[] { 13, 20, 20, 20, 20, 20, 20, 20, 20, 20 };
            for (var i = 0; i <= columnWidths.Length - 1; i++)
            {
                headerRow.Sheet.SetColumnWidth(i, columnWidths[i] * 256);
            }

            var headerStyle = HeaderStyle(workbook);
            var currentColumn = 0;
            var currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.Date);
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.Opening_Balance);
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.Turnover);
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.Void + "/" + Labels.Rejected);
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.Outstanding);
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.Win_Loss);
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.Commission);
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.GrandPrize);
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.Transfer_Amt);
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.Accumulated_Total);
            currentCell.CellStyle = headerStyle;
        }

        private void ExportBodyMemberStatement(
            HSSFWorkbook workbook,
            ISheet worksheet,
            StatementReportModel data)
        {
            var currentRow = 2;
            var statementListLength = data.StatementList.Count;
            if (statementListLength == 0)
            {
                ExportEmptyReport(workbook, worksheet, currentRow);
                return;
            }

            var palette = workbook.GetCustomPalette();          
            var noteOpeningBalanceColorIndex = palette.FindSimilarColor(255, 102, 255).Indexed;

            var reportStyle = ReportStyle(workbook);
            var textStyle = workbook.CreateCellStyle();

            var mediumBlackNormal = workbook.CreateFont();
            mediumBlackNormal.FontName = "Tahoma";
            mediumBlackNormal.FontHeightInPoints = 9;
            mediumBlackNormal.Color = HSSFColor.Black.Index;

            var mediumRedNormal = workbook.CreateFont();
            mediumRedNormal.FontName = "Tahoma";
            mediumRedNormal.FontHeightInPoints = 9;
            mediumRedNormal.Color = HSSFColor.Red.Index;

            textStyle.CloneStyleFrom(reportStyle);
            textStyle.SetFont(mediumBlackNormal);

            var centerTextStyle = workbook.CreateCellStyle();
            centerTextStyle.CloneStyleFrom(textStyle);
            centerTextStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

            var twoDecimalNumberStyle = workbook.CreateCellStyle();
            twoDecimalNumberStyle.CloneStyleFrom(reportStyle);
            twoDecimalNumberStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
            twoDecimalNumberStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
            twoDecimalNumberStyle.SetFont(mediumBlackNormal);

            var twoDecimalRedNumberStyle = workbook.CreateCellStyle();
            twoDecimalRedNumberStyle.CloneStyleFrom(twoDecimalNumberStyle);
            twoDecimalRedNumberStyle.SetFont(mediumRedNormal);

            StatementModel previousStatement = null;
            for (int i = 0; i < statementListLength; i++)
            {
                var row = worksheet.CreateRow(currentRow);
                var statement = data.StatementList[i];
                var currentColumn = 0;
                var column = row.CreateCell(currentColumn);
                column.SetCellValue(statement.StatementDateString);
                column.CellStyle = centerTextStyle;
                currentColumn++;

                column = row.CreateCell(currentColumn);
                ExportDataColumnModel(column, statement.OpeningBalance);
                var cellStyle = workbook.CreateCellStyle();
                cellStyle.CloneStyleFrom(IsNegative(statement.OpeningBalance) ?
                    twoDecimalRedNumberStyle :
                    twoDecimalNumberStyle);
                if (previousStatement != null &&
                    (double)statement.OpeningBalance.Value != (double)previousStatement.AccumulatedTotal.Value)
                {
                    cellStyle.FillForegroundColor = noteOpeningBalanceColorIndex;
                    cellStyle.FillPattern = FillPattern.SolidForeground;
                }

                column.CellStyle = cellStyle;
                currentColumn++;

                column = row.CreateCell(currentColumn);
                ExportDataColumnModel(column, statement.Turnover);
                column.CellStyle = twoDecimalNumberStyle;
                currentColumn++;

                column = row.CreateCell(currentColumn);
                ExportDataColumnModel(column, statement.VoidCancelled);
                column.CellStyle = IsNegative(statement.VoidCancelled) ?
                    twoDecimalRedNumberStyle :
                    twoDecimalNumberStyle;
                currentColumn++;

                column = row.CreateCell(currentColumn);
                ExportDataColumnModel(column, statement.Outstanding);
                column.CellStyle = IsNegative(statement.Outstanding) ?
                    twoDecimalRedNumberStyle :
                    twoDecimalNumberStyle;
                currentColumn++;

                column = row.CreateCell(currentColumn);
                ExportDataColumnModel(column, statement.WinLoss);
                column.CellStyle = IsNegative(statement.WinLoss) ?
                    twoDecimalRedNumberStyle :
                    twoDecimalNumberStyle;
                currentColumn++;

                column = row.CreateCell(currentColumn);
                ExportDataColumnModel(column, statement.Commission);
                column.CellStyle = IsNegative(statement.Commission) ?
                    twoDecimalRedNumberStyle :
                    twoDecimalNumberStyle;
                currentColumn++;

                column = row.CreateCell(currentColumn);
                ExportDataColumnModel(column, statement.GrandPrize);
                column.CellStyle = IsNegative(statement.GrandPrize) ?
                    twoDecimalRedNumberStyle :
                    twoDecimalNumberStyle;
                currentColumn++;

                column = row.CreateCell(currentColumn);
                ExportDataColumnModel(column, statement.TransferAmount);
                column.CellStyle = IsNegative(statement.TransferAmount) ?
                    twoDecimalRedNumberStyle :
                    twoDecimalNumberStyle;
                currentColumn++;

                column = row.CreateCell(currentColumn);
                ExportDataColumnModel(column, statement.AccumulatedTotal);
                column.CellStyle = IsNegative(statement.AccumulatedTotal) ?
                    twoDecimalRedNumberStyle :
                    twoDecimalNumberStyle;

                previousStatement = statement;
                currentRow++;
            }
        }

        private void ExportHeaderSMAStatement(
            HSSFWorkbook workbook,
            ISheet worksheet,
            SMAStatementCriteria criteria)
        {
            var currentRow = 0;
            var titleRow = worksheet.CreateRow(currentRow);
            var titleCell = titleRow.CreateCell(0);
            titleCell.SetCellValue(Labels.Statement + " (" + criteria.Username + "): " + criteria.FromDate.ToString("MM/dd/yyyy") +
                "-->" + criteria.ToDate.ToString("MM/dd/yyyy"));
            titleCell.CellStyle = TitleStyle(workbook);
            currentRow++;

            var headerRow = worksheet.CreateRow(currentRow);
            var columnWidths = new int[] { 13, 20, 20, 20, 20, 20, 20 };
            var maxColumnIndex = 6;
            for (var i = 0; i <= maxColumnIndex; i++)
            {
                headerRow.Sheet.SetColumnWidth(i, columnWidths[i] * 256);
            }

            var headerStyle = HeaderStyle(workbook);
            var currentColumn = 0;
            var currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.Date);
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.Opening_Balance);
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.Turnover);
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.Win_Loss);
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.Commission);
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.Transfer_Amt);
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.Accumulated_Total);
            currentCell.CellStyle = headerStyle;
        }

        private void ExportBodySMAStatement(
            HSSFWorkbook workbook,
            ISheet worksheet,
            StatementReportModel data)
        {
            var statementListLength = data.StatementList.Count;
            var currentRow = 2;
            if (statementListLength == 0)
            {
                ExportEmptyReport(workbook, worksheet, currentRow);
                return;
            }

            var palette = workbook.GetCustomPalette();        
            var noteOpeningBalanceColorIndex = palette.FindSimilarColor(255, 102, 255).Indexed;

            var reportStyle = ReportStyle(workbook);
            var textStyle = workbook.CreateCellStyle();

            var mediumBlackNormal = workbook.CreateFont();
            mediumBlackNormal.FontName = "Tahoma";
            mediumBlackNormal.FontHeightInPoints = 9;
            mediumBlackNormal.Color = HSSFColor.Black.Index;

            var mediumRedNormal = workbook.CreateFont();
            mediumRedNormal.FontName = "Tahoma";
            mediumRedNormal.FontHeightInPoints = 9;
            mediumRedNormal.Color = HSSFColor.Red.Index;

            textStyle.CloneStyleFrom(reportStyle);
            textStyle.SetFont(mediumBlackNormal);

            var centerTextStyle = workbook.CreateCellStyle();
            centerTextStyle.CloneStyleFrom(textStyle);
            centerTextStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

            var twoDecimalNumberStyle = workbook.CreateCellStyle();
            twoDecimalNumberStyle.CloneStyleFrom(reportStyle);
            twoDecimalNumberStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
            twoDecimalNumberStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
            twoDecimalNumberStyle.SetFont(mediumBlackNormal);

            var twoDecimalRedNumberStyle = workbook.CreateCellStyle();
            twoDecimalRedNumberStyle.CloneStyleFrom(twoDecimalNumberStyle);
            twoDecimalRedNumberStyle.SetFont(mediumRedNormal);

            StatementModel previousStatement = null;
            for (int i = 0; i < statementListLength; i++)
            {
                var row = worksheet.CreateRow(currentRow);             

                var statement = data.StatementList[i];
                var currentColumn = 0;
                var column = row.CreateCell(currentColumn);
                column.SetCellValue(statement.StatementDateString);
                column.CellStyle = centerTextStyle;
                currentColumn++;

                column = row.CreateCell(currentColumn);
                ExportDataColumnModel(column, statement.OpeningBalance);
                var cellStyle = workbook.CreateCellStyle();
                cellStyle.CloneStyleFrom(IsNegative(statement.OpeningBalance) ?
                    twoDecimalRedNumberStyle :
                    twoDecimalNumberStyle);
                if (previousStatement != null &&
                   (double)statement.OpeningBalance.Value != (double)previousStatement.AccumulatedTotal.Value)
                {
                    cellStyle.FillForegroundColor = noteOpeningBalanceColorIndex;
                    cellStyle.FillPattern = FillPattern.SolidForeground;
                }

                column.CellStyle = cellStyle;
                currentColumn++;

                column = row.CreateCell(currentColumn);
                ExportDataColumnModel(column, statement.Turnover);
                column.CellStyle = twoDecimalNumberStyle;
                currentColumn++;

                column = row.CreateCell(currentColumn);
                ExportDataColumnModel(column, statement.WinLoss);
                column.CellStyle = IsNegative(statement.WinLoss) ?
                    twoDecimalRedNumberStyle :
                    twoDecimalNumberStyle;
                currentColumn++;

                column = row.CreateCell(currentColumn);
                ExportDataColumnModel(column, statement.Commission);
                column.CellStyle = IsNegative(statement.Commission) ?
                    twoDecimalRedNumberStyle :
                    twoDecimalNumberStyle;
                currentColumn++;

                column = row.CreateCell(currentColumn);
                ExportDataColumnModel(column, statement.TransferAmount);
                column.CellStyle = IsNegative(statement.TransferAmount) ?
                    twoDecimalRedNumberStyle :
                    twoDecimalNumberStyle;
                currentColumn++;

                column = row.CreateCell(currentColumn);
                ExportDataColumnModel(column, statement.AccumulatedTotal);
                column.CellStyle = IsNegative(statement.AccumulatedTotal) ?
                    twoDecimalRedNumberStyle :
                    twoDecimalNumberStyle;

                previousStatement = statement;
                currentRow++;
            }
        }

        private void ExportDataColumnModel(ICell column, DataColumnModel cellValue)
        {
            if (cellValue.HasData)
            {
                column.SetCellValue((double)cellValue.Value);
            }
            else
            {
                column.SetCellValue(Labels.Symbol_NoData);
            }
        }

        private void ExportDataColumnText(ICell column, string cellValue)
        {
            column.SetCellValue(cellValue);
        }

        private void ExportHeaderTransferDetails(
            HSSFWorkbook workbook,
            ISheet worksheet,
            TransferDetailsCriteria criteria)
        {
            var isShowCreator = _statementSection.ShowCreatorOnDetail;
            var currentRow = 0;
            var titleRow = worksheet.CreateRow(currentRow);
            var titleCell = titleRow.CreateCell(0);
            titleCell.SetCellValue(Labels.Transfer_Details + " ( " + criteria.Username + "): " + criteria.TransferDate);
            titleCell.CellStyle = TitleStyle(workbook);
            currentRow++;

            var headerRow = worksheet.CreateRow(currentRow);
            var columnWidths = new List<int> { 13, 50, 20 };
            if (isShowCreator)
            {
                columnWidths.Add(13);
            }

            var maxColumnIndex = 2;
            for (var i = 0; i <= maxColumnIndex; i++)
            {
                headerRow.Sheet.SetColumnWidth(i, columnWidths[i] * 256);
            }

            var headerStyle = HeaderStyle(workbook);
            var currentColumn = 0;
            var currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.Date);
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.Remark);
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            currentCell = headerRow.CreateCell(currentColumn);
            currentCell.SetCellValue(Labels.Amount);
            currentCell.CellStyle = headerStyle;
            currentColumn++;

            if (isShowCreator)
            {
                currentCell = headerRow.CreateCell(currentColumn);
                currentCell.SetCellValue(Labels.Creator);
                currentCell.CellStyle = headerStyle;
            }
        }

        private void ExportBodyTransferDetails(
            HSSFWorkbook workbook,
            ISheet worksheet,
            StatementReportModel data)
        {
            var statementListLength = data.StatementList.Count;
            var currentRow = 2;

            if (statementListLength == 0)
            {
                ExportEmptyReport(workbook, worksheet, currentRow);
                return;
            }

            var palette = workbook.GetCustomPalette();        

            var reportStyle = ReportStyle(workbook);
            var textStyle = workbook.CreateCellStyle();

            var mediumBlackNormal = workbook.CreateFont();
            mediumBlackNormal.FontName = "Tahoma";
            mediumBlackNormal.FontHeightInPoints = 9;
            mediumBlackNormal.Color = HSSFColor.Black.Index;

            var mediumRedNormal = workbook.CreateFont();
            mediumRedNormal.FontName = "Tahoma";
            mediumRedNormal.FontHeightInPoints = 9;
            mediumRedNormal.Color = HSSFColor.Red.Index;

            textStyle.CloneStyleFrom(reportStyle);
            textStyle.SetFont(mediumBlackNormal);

            var centerTextStyle = workbook.CreateCellStyle();
            centerTextStyle.CloneStyleFrom(textStyle);
            centerTextStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

            var twoDecimalNumberStyle = workbook.CreateCellStyle();
            twoDecimalNumberStyle.CloneStyleFrom(reportStyle);
            twoDecimalNumberStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
            twoDecimalNumberStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
            twoDecimalNumberStyle.SetFont(mediumBlackNormal);

            var twoDecimalRedNumberStyle = workbook.CreateCellStyle();
            twoDecimalRedNumberStyle.CloneStyleFrom(twoDecimalNumberStyle);
            twoDecimalRedNumberStyle.SetFont(mediumRedNormal);

            StatementModel statement;
            var currentColumn = 0;
            ICell column;
            IRow row;
            var isShowExchangeRate = _statementSection.ShowExchangeRateOnDetail;
            var isShowCreator = _statementSection.ShowCreatorOnDetail;
            for (int i = 0; i < statementListLength; i++)
            {
                row = worksheet.CreateRow(currentRow);   
                statement = data.StatementList[i];
                currentColumn = 0;
                column = row.CreateCell(currentColumn);
                column.SetCellValue(statement.StatementDateString.Replace("<br>", "\n"));
                column.CellStyle = centerTextStyle;
                currentColumn++;

                column = row.CreateCell(currentColumn);

                var remark = statement.TransactionType + "\n" + statement.Description +
                   (isShowExchangeRate && !string.IsNullOrEmpty(statement.ExchangeRate) ?
                       " - " + Labels.Symbol_ExchangeRate + ":" + statement.ExchangeRate :
                       string.Empty);
                column.SetCellValue(remark);
                column.CellStyle = textStyle;
                currentColumn++;

                column = row.CreateCell(currentColumn);
                ExportDataColumnModel(column, statement.TransferAmount);
                column.CellStyle = IsNegative(statement.TransferAmount) ?
                    twoDecimalRedNumberStyle :
                    twoDecimalNumberStyle;
                currentColumn++;

                if (isShowCreator)
                {
                    column = row.CreateCell(currentColumn);
                    ExportDataColumnText(column, statement.Creator);
                    column.CellStyle = textStyle;                  
                }

                currentRow++;
            }

            statement = data.Total;
            currentColumn = 0;

            var grandTotalColorIndex = palette.FindSimilarColor(219, 225, 187).Indexed;
            var grandTotalTextStyle = workbook.CreateCellStyle();

            var boldBlackNormal = workbook.CreateFont();
            boldBlackNormal.FontName = "Tahoma";
            boldBlackNormal.FontHeightInPoints = 9;
            boldBlackNormal.Color = HSSFColor.Black.Index;
            boldBlackNormal.IsBold = true;

            var boldRedNormal = workbook.CreateFont();
            boldRedNormal.FontName = "Tahoma";
            boldRedNormal.FontHeightInPoints = 9;
            boldRedNormal.Color = HSSFColor.Red.Index;
            boldRedNormal.IsBold = true;

            grandTotalTextStyle.CloneStyleFrom(reportStyle);
            grandTotalTextStyle.SetFont(boldBlackNormal);
            grandTotalTextStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
            grandTotalTextStyle.FillForegroundColor = grandTotalColorIndex;
            grandTotalTextStyle.FillPattern = FillPattern.SolidForeground;

            var twoDecimalRedNumberStyleGrandTotal = workbook.CreateCellStyle();
            twoDecimalRedNumberStyleGrandTotal.CloneStyleFrom(twoDecimalRedNumberStyle);
            twoDecimalRedNumberStyleGrandTotal.SetFont(boldRedNormal);

            var twoDecimalNumberStyleGrandTotal = workbook.CreateCellStyle();
            twoDecimalNumberStyleGrandTotal.CloneStyleFrom(twoDecimalNumberStyle);
            twoDecimalNumberStyleGrandTotal.SetFont(boldBlackNormal);

            row = worksheet.CreateRow(currentRow);
            column = row.CreateCell(currentColumn);
            column.CellStyle = grandTotalTextStyle;
            currentColumn++;

            column = row.CreateCell(currentColumn);
            column.SetCellValue(Labels.Total);
            column.CellStyle = grandTotalTextStyle;
            currentColumn++;

            column = row.CreateCell(currentColumn);
            ExportDataColumnModel(column, statement.TransferAmount);
            var cellStyle = workbook.CreateCellStyle();
            cellStyle.CloneStyleFrom(IsNegative(statement.TransferAmount) ?
                twoDecimalRedNumberStyleGrandTotal :
                twoDecimalNumberStyleGrandTotal);
            cellStyle.FillForegroundColor = grandTotalColorIndex;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            column.CellStyle = cellStyle;
            currentColumn++;

            if (isShowCreator)
            {
                column = row.CreateCell(currentColumn);
                column.SetCellValue(string.Empty);
                column.CellStyle = grandTotalTextStyle;              
            }          
        }

        private bool IsNegative(DataColumnModel cellValue)
        {
            return cellValue.HasData && (double)cellValue.Value < 0;
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
            column.SetCellValue(Labels.Message_NoDataReport);
            column.CellStyle = textStyle;
        }
    }
}
