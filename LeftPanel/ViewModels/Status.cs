namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    /// <summary>
    /// Status object.
    /// </summary>
    public class Status
    {
        public int TotalActive { get; set; }

        public int TotalClosed { get; set; }

        public int TotalSuspended { get; set; }

        public string Text { get; set; }

        public string Tooltip { get; set; }
    }
}
