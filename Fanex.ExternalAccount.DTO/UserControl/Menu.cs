// -----------------------------------------------------------------------
// <copyright file="Menu.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.DTO.UserControl
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Menu
    {
        public int MenuID { get; set; }

        public string MenuName { get; set; }

        public string MenuDesc { get; set; }

        public string Url { get; set; }

        public int FuncID { get; set; }

        public string FuncName { get; set; }

        public int ParentID { get; set; }

        public string ParentName { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public int Creator { get; set; }

        public string CreatedDate { get; set; }

        public int Modifier { get; set; }

        public string ModifiedDate { get; set; }
    }
}