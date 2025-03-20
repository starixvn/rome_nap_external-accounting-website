namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.Menu
{
    using Fanex.Data.Repository;

    public class DeleteMenuCriteria : INonQueryCommand
    {
        public int UserId { get; set; }
        public int MenuId { get; set; }

        // Output parameters
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public string GetSettingKey() => "EAS_MenuDel";

        public bool IsValid() => UserId > 0 && MenuId > 0;
    }
} 