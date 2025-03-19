namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl
{
    using Fanex.Data.Repository;

    public class CreateFunctionCriteria : INonQueryCommand
    {
        public int UserId { get; set; }
        public int ParentId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsTest { get; set; }
        public string ParentName { get; set; }
        public string FuncName { get; set; }
        public string FuncDesc { get; set; }
        public string FuncKey { get; set; }
        public string IP { get; set; }
        public string AssignedGroupIDs { get; set; }

        // Output parameters
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public string GetSettingKey()
        {
            return "EAS_FunctionIns";
        }
        
        public bool IsValid() => true;
    }
}