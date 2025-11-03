namespace Fanex.ExternalAccount.Statement.Services
{
    using Fanex.ExternalAccount.Statement.Criteria.StatementReport;
    
    public class ReportService
    {
        public const string BetTrans14SPSuffix = "_14";
        public const string WarehouseSPSuffix = "_WH";
        
        public static string GetStoreMappingSuffix(DateRangeReportCriteria criteria)
        {
            
            if (criteria.FromDate > criteria.MoveBetsDay)
            {
                return string.Empty;
            }
            
            if (criteria.FromDate >= criteria.BalanceUpDay)
            {
                return BetTrans14SPSuffix;
            }
            
            return WarehouseSPSuffix;
        }
    }
}
