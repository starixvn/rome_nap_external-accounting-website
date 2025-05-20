namespace Fanex.ExternalAccount.DataAccess.Criteria.DataRange
{
    using System;
    using Fanex.Data.Repository;
    using Fanex.Data;
    
    public class UpdateDataRangeCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ListCustID")]
        public string CustIds { get; set; }

        public DateTime? MinDate { get; set; }

        [SpParam(SqlParamName = "TimePeriod")]
        public int? LimitDays { get; set; }

        public int Choice { get; set; }

        public int UserId { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public override string GetSettingKey() => "EAS_CustomerDateLimitMultipleUpd";

        public override bool IsValid() => UserId > 0 && !string.IsNullOrWhiteSpace(CustIds);
    }
}
