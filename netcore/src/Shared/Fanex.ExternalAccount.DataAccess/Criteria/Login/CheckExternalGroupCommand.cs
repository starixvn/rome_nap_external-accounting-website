namespace Fanex.ExternalAccount.DataAccess.Criteria.Login
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CheckExternalGroupCommand : INonQueryCommand
    {
        [SpParam(SqlParamName = "UserID")]
        public int CustId { get; }

        public int? FuncId { get; }

        public string FuncKey { get; }

        public int ErrCode { get; set; }

        public string ErrorMsg { get; set; }

        public CheckExternalGroupCommand(int custId, int? funcId, string funcKey)
        {
            CustId = custId;
            FuncId = funcId;
            FuncKey = funcKey;
        }

        public string GetSettingKey() => "EAS_UserCheckPermission";

        public bool IsValid() => CustId > 0;
    }
}



