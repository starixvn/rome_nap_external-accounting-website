namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.NAP.Framework.Data;

    public class UpdateDoubleCommCriteria : CriteriaBase
    {
        public UpdateDoubleCommCriteria(int custId, bool allow, int userId, string ip)
        {
            CustId = custId;
            Allow = allow;
            UserId = userId;
            Language = "en";
            SubAccId = 0;            
            IP = ip;
        }

        public int CustId { get; }
        [SpParam(SqlParamName = "doublecommlosing")]
        public bool Allow { get; }
        public int UserId { get; }        
        public string Language { get; }
        public int SubAccId { get; }
        public string IP { get; }
        public int ErrCode { get; }
        public string ErrMsg { get; }

        public override string GetSettingKey() => "Acc_CustomerUpdDoubleCommLosing";
    }
}
