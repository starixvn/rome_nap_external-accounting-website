namespace Fanex.ProductStatus.Setting.Models
{
    public class ProductStatusModel
    {
        public int SportType { get; set; }

        public int ParentSportType { get; set; }

        public int BetType { get; set; }

        public int ParentBetType { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsDisabledUpline { get; set; }

        public int TotalDisabledMasters { get; set; }

        public int TotalEnabledMasters { get; set; }

        public int TotalDisabledAgents { get; set; }

        public int TotalEnabledAgents { get; set; }

        public int TotalDisabledMembers { get; set; }

        public int TotalEnabledMembers { get; set; }

        public string Name { get; set; }
    }
}
