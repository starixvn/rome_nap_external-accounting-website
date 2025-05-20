namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.Menu
{
    using Fanex.Data.Repository;

    public class UpdateMenuCriteria : INonQueryCommand
    {
        public int UserId { get; set; }
        public int MenuId { get; set; }
        public int FuncId { get; set; }
        public int ParentId { get; set; }
        public int DisplayOrder { get; set; }
        public string MenuName { get; set; }
        public string MenuDesc { get; set; }
        public string ParentName { get; set; }
        public string URL { get; set; }
        public string IP { get; set; }

        // Output parameters
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public string GetSettingKey() => "EAS_MenuUpd";

        public bool IsValid() => MenuId > 0;
    }
} 