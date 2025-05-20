namespace Fanex.ProductStatus.Setting.ViewModels
{
    using Models;

    public class SimpleProductStatusViewModel
    {
        public SimpleProductStatusViewModel(ProductStatusModel model)
        {
            Name = model.Name;
            SportType = model.SportType;
            BetType = model.BetType;
            IsDisabled = model.IsDisabledUpline || model.IsDisabled;
            IsDisabledUpline = model.IsDisabledUpline;
            IsDisabledDownlines = model.TotalDisabledMasters > 0
                || model.TotalDisabledAgents > 0
                || model.TotalDisabledMembers > 0;
            HasDownlines = model.TotalEnabledMasters
                + model.TotalDisabledMasters
                + model.TotalEnabledAgents
                + model.TotalDisabledAgents
                + model.TotalEnabledMembers
                + model.TotalDisabledMembers > 0;
        }

        public string Name { get; }

        public int SportType { get; }

        public int BetType { get; }

        public bool IsDisabled { get; }

        public bool IsDisabledUpline { get; }

        public bool IsDisabledDownlines { get; }

        public bool HasDownlines { get; set; }
    }
}
