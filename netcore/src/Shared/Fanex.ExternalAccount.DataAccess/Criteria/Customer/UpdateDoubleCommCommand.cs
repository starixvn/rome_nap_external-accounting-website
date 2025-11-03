namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateDoubleCommCommand : INonQueryCommand
    {
        public UpdateDoubleCommCommand(int custId, bool allow, int userId, string ip)
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
        public int ErrCode { get; set; }
        public string ErrMsg { get; set;  }

        public string GetSettingKey() => "Acc_CustomerUpdDoubleCommLosing";

        public bool IsValid() => true;
    }
}
