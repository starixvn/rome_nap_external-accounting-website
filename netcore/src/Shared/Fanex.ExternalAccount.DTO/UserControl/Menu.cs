namespace Fanex.ExternalAccount.DTO.UserControl
{
    public class Menu
    {
        public int MenuId { get; set; }

        public string MenuName { get; set; }

        public string MenuDesc { get; set; }

        public string Url { get; set; }

        public int FuncId { get; set; }

        public string FuncName { get; set; }

        public string FuncKey { get; set; }

        public int ParentId { get; set; }

        public string ParentName { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public int Creator { get; set; }

        public string CreatedDate { get; set; }

        public int Modifier { get; set; }

        public string ModifiedDate { get; set; }
    }
}