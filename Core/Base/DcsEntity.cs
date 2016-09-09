namespace Fanex.ExternalAccount.Core.Base
{
    public class DcsEntity
    {
        public bool IsSyncedUpline { get; set; }

        public bool IsDisabledUpline { get; set; }

        public bool IsHidden { get; set; }

        public bool IsSyncedDownline { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsDisabledDownline { get; set; }

        public bool IsSynced { get; set; }

        public bool HasDownline { get; set; }
    }
}