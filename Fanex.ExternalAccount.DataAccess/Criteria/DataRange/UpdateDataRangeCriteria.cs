namespace Fanex.ExternalAccount.DataAccess.Criteria.DataRange
{
    using System;
    using System.Configuration;
    using Fanex.ExternalAccount.Utilities;
    using NAP.Framework.Data;

    public class UpdateDataRangeCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ListCustID")]
        public string CustIds { get; set; }

        public DateTime? MinDate { get; set; }

        [SpParam(SqlParamName = "TimePeriod")]
        public int? LimitDays { get; set; }

        public int Choice { get; set; }

        public int UserId { get; set; }

        public byte SystemId
            => (byte)Utils.ToInt32(ConfigurationManager.AppSettings["SystemId"]);

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey() => "EAS_CustomerDateLimitMultipleUpd";

        public override bool IsValid() => UserId > 0 && !string.IsNullOrWhiteSpace(CustIds);
    }
}
