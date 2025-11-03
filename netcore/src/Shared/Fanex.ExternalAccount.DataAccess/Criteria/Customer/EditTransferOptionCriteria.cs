namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class EditTransferOptionCriteria : CriteriaBase
    {
        public int CustId { get; }
        public string TransferOption { get; }
        [SpParam(SqlParamName = "turnoffautotransfer")]
        public int AutoTransferCondition { get; }
        public int UserId { get; }
        public string IP { get; }
        public int SubAccId { get; }
        public string Language { get; }
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public EditTransferOptionCriteria(string transferOption, int autoTransferCondition, int custId, int userId, string ip, int subAccId, string language)
        {
            TransferOption = transferOption;
            AutoTransferCondition = autoTransferCondition;
            CustId = custId;
            UserId = userId;
            IP = ip;
            SubAccId = subAccId;
            Language = language;
        }

        public override string GetSettingKey() => "EAS_CustomerUpdTransferOption";

        public override bool IsValid() => CustId > 0;
    }
}


