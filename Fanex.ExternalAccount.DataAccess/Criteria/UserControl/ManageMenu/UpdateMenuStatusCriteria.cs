namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.Menu
{
    using Fanex.Data.Repository;

    public class UpdateMenuStatusCriteria : INonQueryCommand
    {
        public int UserId { get; set; }
        public int MenuId { get; set; }
        public bool Active { get; set; }

        // Output parameters
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public string GetSettingKey() => "EAS_MenuUpdStatus";

        public bool IsValid() => UserId > 0 && MenuId > 0;
    }
} 