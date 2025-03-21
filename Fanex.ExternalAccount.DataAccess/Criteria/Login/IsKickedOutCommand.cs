namespace Fanex.ExternalAccount.DataAccess.Criteria.Login
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class IsKickedOutCommand : CriteriaBase, INonQueryCommand
    {
        [SpParam(SqlParamName = "On_UserID")]
        public int CustId { get; }
        public bool IsKickedOut { get; set; }

        public IsKickedOutCommand(int custId)
        {
            CustId = custId;
        }

        public override string GetSettingKey() => "EAS_UserRecheckIn";

        public override bool IsValid() => CustId > 0;
    }
}