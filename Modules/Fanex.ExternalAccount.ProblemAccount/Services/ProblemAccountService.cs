namespace Fanex.ExternalAccount.ProblemAccount.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Excel.Utility;
    using Fanex.ExternalAccount.ProblemAccount.Constants;
    using Fanex.ExternalAccount.ProblemAccount.Criteria;
    using Fanex.ExternalAccount.ProblemAccount.DataAccesses;
    using Fanex.ExternalAccount.ProblemAccount.Extensions;
    using Fanex.ExternalAccount.ProblemAccount.Models;
    using Fanex.ExternalAccount.ProblemAccount.Resources;
    using Fanex.ExternalAccount.ProblemAccount.ViewModels;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    public interface IProblemAccountService
    {
        ProblemAccountReport GetProblemAccounts(GetListViewModel viewModel);

        MemoryStream ExportExcel(GetListViewModel searchFilter, string excelTemplatePath);

        Result SetToReviewed(SetToReviewedViewModel viewModel);
    }

    public class ProblemAccountService : IProblemAccountService
    {
        private const int FirstColumnIndex = 0;

        private readonly IProblemAccountDataAccess dataAccess;

        public ProblemAccountService(IProblemAccountDataAccess dataAccess = null)
        {
            this.dataAccess = dataAccess ?? new ProblemAccountDataAccess();
        }

        public ProblemAccountReport GetProblemAccounts(GetListViewModel viewModel)
        {
            viewModel.PageIndex++;
            var criteria = new GetProblemAccountCriteria(viewModel);
            var problemAccounts = dataAccess.GetProblemAccounts(criteria);

            return new ProblemAccountReport(criteria.TotalRows, problemAccounts);
        }

        public MemoryStream ExportExcel(GetListViewModel searchFilter, string excelTemplatePath)
        {
            const int allPageSize = int.MaxValue - 2;
            searchFilter.PageIndex = 0;
            searchFilter.PageSize = allPageSize;
            var report = GetProblemAccounts(searchFilter);

            using (var file = new FileStream(excelTemplatePath, FileMode.Open, FileAccess.Read))
            {
                var workbook = new HSSFWorkbook(file);
                var dataSheet = workbook.GetSheet("ProblemAccountList");
                var templateSheet = workbook.GetSheet("Template");

                ExportData(dataSheet, templateSheet, searchFilter, report.ProblemAccounts);
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
            var isMemberLevel = searchFilter.SearchRoleId == Role.Member;

            ExportSearchFilter(dataSheet, searchFilter);
            ExportBodyRows(dataSheet, templateSheet, problemAccounts, isMemberLevel);
        }

        private static void ExportSearchFilter(ISheet dataSheet, GetListViewModel searchFilter)
        {
            const int mainTitleRowIndex = 0;
            var uplineRoleId = searchFilter.SearchRoleId + 1;

            var mainTitle = new StringBuilder();
            mainTitle
                .Append($"{ViewLang.FilterOption}:\n")
                .AppendFormat("\t{0}: {1}\n", uplineRoleId.DisplayRoleName(), searchFilter.UplineUsername)
                .Append($"\t{ViewLang.DateNotifiedFrom}: {searchFilter.FromDate.ToDateWithSlash()}\n")
                .Append($"\t{ViewLang.To}: {searchFilter.ToDate.ToDateWithSlash()}\n")
                .Append($"\t{ViewLang.ReviewStatus}: {ReviewStatusConst.GetReviewStatusName(searchFilter.ReviewStatus)}\n")
                .Append($"\t{ViewLang.Username}: {searchFilter.SearchUsername}\n")
                .Append($"{ViewLang.ExportDate}: {DateTime.Now.ToDateTimeInSecondString()}");

            dataSheet.GetCell(mainTitleRowIndex, FirstColumnIndex).SetCellValue(mainTitle.ToString());
        }

        private static void ExportBodyRows(
            ISheet dataSheet,
            ISheet templateSheet,
            IEnumerable<ProblemAccountEntity> problemAccounts,
            bool isMemberLevel)
        {
            var dataRowIndex = 2;
            var lastColumnIndex = isMemberLevel ? 7 : 8;
            var number = 0;

            foreach (var item in problemAccounts)
            {
                SetBackgroundRow(dataSheet, templateSheet, dataRowIndex, lastColumnIndex, item);
                var values = new List<object>
                {
                    ++number,
                    item.Username,
                    item.FirstName,
                    item.LastName,
                    item.StatusName,
                    item.CreatedDateStr
                };

                if (!isMemberLevel)
                {
                    values.Add(item.NumberOfDownline);
                }

                values.Add(item.ReviewStatus);
                dataSheet.GetRow(dataRowIndex).WriteCells(FirstColumnIndex, values);

                dataRowIndex++;
            }
        }

        private static void SetBackgroundRow(
            ISheet dataSheet,
            ISheet templateSheet,
            int rowId,
            int lastColumnIndex,
            ProblemAccountEntity problemAccount)
        {
            var fromRowId = MapAccountStatusToExcelRow(problemAccount.Status, problemAccount.ClosedReason);

            dataSheet.CopyRow(templateSheet, fromRowId, rowId, FirstColumnIndex, lastColumnIndex);
        }

        private static int MapAccountStatusToExcelRow(byte status, byte closedReason)
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
                    return AccountStatusConst.OpenRowIndex;
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

                case ClosedReasonConst.ClosedWrongOTPSC:
                    return ClosedReasonConst.ClosedWrongOTPSCRowIndex;

                case ClosedReasonConst.ClosedWrongPwdOthers:
                default:
                    return ClosedReasonConst.ClosedWrongPwdOthersRowIndex;
            }
        }

        private static MemoryStream WriteToStream(HSSFWorkbook workbook)
        {
            var memoryStream = new MemoryStream();
            workbook.Write(memoryStream);

            return memoryStream;
        }

        public Result SetToReviewed(SetToReviewedViewModel viewModel)
            => dataAccess.SetToReviewed(new SetToReviewedCriteria(viewModel));
    }
}
