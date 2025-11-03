namespace Fanex.ProductStatus.Setting.ViewModels
{
    public class DownlineStatusViewModel
    {
        private readonly bool isDisabledUpline;

        public DownlineStatusViewModel(bool isDisabledUpline, int totalDisabled, int totalEnabled)
        {
            this.isDisabledUpline = isDisabledUpline;
            TotalDisabled = totalDisabled;
            TotalEnabled = totalEnabled;
        }

        public int TotalDisabled { get; }

        public int TotalEnabled { get; }

        public int Total => TotalDisabled + TotalEnabled;

        public bool IsDisabled => isDisabledUpline || Total == 0;
    }
}
