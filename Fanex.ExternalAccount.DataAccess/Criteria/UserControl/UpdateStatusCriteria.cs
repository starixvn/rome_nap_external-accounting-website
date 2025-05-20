namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl
{
    using Fanex.Data.Repository;

    public class UpdateStatusCriteria : INonQueryCommand
    {
        public int UserId { get; set; }
        public int FuncId { get; set; }
        public bool IsTest { get; set; }
        public string IP { get; set; }

        // Output parameters
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public string GetSettingKey() => "EAS_FuncUpdStatus";

        public bool IsValid() => true;
    }
} 