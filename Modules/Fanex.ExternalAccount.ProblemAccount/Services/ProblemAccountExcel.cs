namespace Fanex.ExternalAccount.ProblemAccount.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.Excel.Utility;
    using Fanex.ExternalAccount.ProblemAccount.Constants;
    using Fanex.ExternalAccount.ProblemAccount.Extensions;
    using Fanex.ExternalAccount.ProblemAccount.Models;
    using Fanex.ExternalAccount.ProblemAccount.Resources;
    using Fanex.ExternalAccount.ProblemAccount.ViewModels;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    public interface IProblemAccountExcel
    {
        MemoryStream ExportExcel(string excelTemplatePath, GetListViewModel searchFilter, IEnumerable<ProblemAccountEntity> problemAccounts);
    }

    public class ProblemAccountExcel : IProblemAccountExcel
    {
        private const string DataSheetName = "ProblemAccountList";
        private const string TemplateSheetName = "Template";

        private const int FirstColumnIndex = 0;
        private const int TitleRowIndex = 0;
        private const int DataRowIndex = 2;

        private const int TotalMemberColumns = 7;
        private const int TotalMasterAgentColumns = 8;

        public MemoryStream ExportExcel(string excelTemplatePath, GetListViewModel searchFilter, IEnumerable<ProblemAccountEntity> problemAccounts)
        {
            using (var file = new FileStream(excelTemplatePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new HSSFWorkbook(file);
                var dataSheet = workbook.GetSheet(DataSheetName);
                var templateSheet = workbook.GetSheet(TemplateSheetName);

                ExportData(dataSheet, templateSheet, searchFilter, problemAccounts);
                workbook.RemoveSheetAt(workbook.GetSheetIndex(templateSheet));

                return WriteToStream(workbook);
            }
        }

        private static void ExportData(
            ISheet dataSheet,
            ISheet templateSheet,
            GetListViewModel searchFilter,
            IEnumerable<ProblemAccountEntity> problemAccounts)
        {
            ExportSearchFilter(dataSheet, searchFilter);
            ExportBodyRows(dataSheet, templateSheet, problemAccounts, ((int)searchFilter.SearchRoleId).IsNotMember());
        }

        private static void ExportSearchFilter(ISheet dataSheet, GetListViewModel searchFilter)
        {
            const string formatPattern = "\t{0}: {1}\n";

            var title = new StringBuilder();
            title
                .Append($"{ViewLang.FilterOption}:\n")
                .AppendFormat(formatPattern, ((int)searchFilter.SearchRoleId).DirectUplineLevel().DisplayRoleName(), searchFilter.UplineUsername)
                .AppendFormat(formatPattern, ViewLang.DateNotifiedFrom, searchFilter.FromDate.ToDateWithSlash())
                .AppendFormat(formatPattern, ViewLang.To, searchFilter.ToDate.ToDateWithSlash())
                .AppendFormat(formatPattern, ViewLang.ReviewStatus, ReviewStatusConst.GetReviewStatusName(searchFilter.ReviewStatus))
                .AppendFormat(formatPattern, ViewLang.Username, searchFilter.SearchUsername)
                .AppendFormat("{0}: {1}", ViewLang.ExportDate, DateTime.Now.ToDateTimeInSecondString());

            dataSheet.GetCell(TitleRowIndex, FirstColumnIndex).SetCellValue(title.ToString());
        }

        private static void ExportBodyRows(
            ISheet dataSheet,
            ISheet templateSheet,
            IEnumerable<ProblemAccountEntity> problemAccounts,
            bool isNotMember)
        {
            var startRowIndex = DataRowIndex;
            var lastColumnIndex = isNotMember ? TotalMasterAgentColumns : TotalMemberColumns;
            var number = 0;

            foreach (var item in problemAccounts)
            {
                SetBackgroundRow(dataSheet, templateSheet, startRowIndex, lastColumnIndex, item);
                var values = new List<object>
                {
                    ++number,
                    item.Username,
                    item.FirstName,
                    item.LastName,
                    item.StatusName,
                    item.CreatedDateStr
                };

                if (isNotMember)
                {
                    values.Add(item.NumberOfDownline);
                }

                values.Add(item.ReviewStatus);
                dataSheet.GetRow(startRowIndex).WriteCells(FirstColumnIndex, values);

                startRowIndex++;
            }
        }

        private static void SetBackgroundRow(
            ISheet dataSheet,
            ISheet templateSheet,
            int rowId,
            int lastColumnIndex,
            ProblemAccountEntity problemAccount)
        {
            var fromRowId = MapAccountStatusToExcelRow(problemAccount.Status, problemAccount.ClosedReason, problemAccount.Locked);

            dataSheet.CopyRow(templateSheet, fromRowId, rowId, FirstColumnIndex, lastColumnIndex);
        }

        private static int MapAccountStatusToExcelRow(byte status, byte closedReason, byte locked)
        {
            switch (status)
            {
                case AccountStatusConst.Suspended:
                    return AccountStatusConst.SuspendedRowIndex;

                case AccountStatusConst.Closed:
                    return MapClosedReasonToExcelRow(closedReason);

                case AccountStatusConst.Disabled:
                    return AccountStatusConst.DisabledRowIndex;

                case AccountStatusConst.Open:
                default:
                    return MapLockedAccessToExcelRow(locked);
            }
        }

        private static int MapClosedReasonToExcelRow(byte closedReason)
        {
            switch (closedReason)
            {
                case ClosedReasonConst.ClosedByAdmin:
                    return ClosedReasonConst.ClosedByAdminRowIndex;

                case ClosedReasonConst.ClosedByUpline:
                    return ClosedReasonConst.ClosedByUplineRowIndex;

                case ClosedReasonConst.ClosedByArchive:
                    return ClosedReasonConst.ClosedByArchiveRowIndex;

                default:
                    return AccountStatusConst.OpenRowIndex;
            }
        }

        private static int MapLockedAccessToExcelRow(byte locked)
        {
            switch (locked)
            {
                case LockedAccessConst.LockedFalseInputOTPSC:
                    return LockedAccessConst.LockedFalseInputOTPSCRowIndex;

                case LockedAccessConst.LockedFalseInputPassword:
                    return LockedAccessConst.LockedFalseInputPasswordRowIndex;

                default:
                    return AccountStatusConst.OpenRowIndex;
            }
        }

        private static MemoryStream WriteToStream(IWorkbook workbook)
        {
            var memoryStream = new MemoryStream();
            workbook.Write(memoryStream);

            return memoryStream;
        }
    }
}
