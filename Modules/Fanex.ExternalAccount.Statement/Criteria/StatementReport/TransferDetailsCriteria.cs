namespace Fanex.ExternalAccount.Statement.Criteria.StatementReport
{
    using System;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.Statement.Services;
    using Fanex.Logging;

    public class TransferDetailsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "WinLostDate")]
        public string TransferDate { get; set; }

        [SpParam(Ignored = true)]
        public DateTime BalanceUpDate { get; set; }

        public int CustId { get; set; }

        [SpParam(Ignored = true)]
        public string Username { get; set; }

        [SpParam(Ignored = true)]
        public int RoleId { get; set; }

        [SpParam(Ignored = true)]
        public string SiteName { get; set; }

        public override string GetSettingKey()
        {
            var storeProcedureSuffix = Converter.ToDateTime(TransferDate) >= BalanceUpDate ?
                string.Empty :
                ReportService.WarehouseSPSuffix;

            return "Statement_Report_TransferDetails" + storeProcedureSuffix;
        }

        public override bool IsValid()
        {
            try
            {
                Converter.ToDateTime(this.TransferDate);
            }
            catch (Exception ex)
            {
                LogService.Info(ex.Message);
                return false;
            }

            return true;
        }
    }
}
