namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl
{
    using Fanex.Data.Repository;

    public class UpdateFunctionCriteria : INonQueryCommand
    {
        public int UserId { get; set; }
        public int FuncId { get; set; }
        public int ParentId { get; set; }
        public int DisplayOrder { get; set; }
        public string ParentName { get; set; }
        public string FuncName { get; set; }
        public string FuncDesc { get; set; }
        public string IP { get; set; }
        public string AssignedGroupIDs { get; set; }
        public string OldAssignedGroupIDs { get; set; }
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public string GetSettingKey() => "EAS_FunctionUpd";

        public bool IsValid() => true;
    }
}