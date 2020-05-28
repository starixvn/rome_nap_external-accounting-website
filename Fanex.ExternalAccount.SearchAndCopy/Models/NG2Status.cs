namespace Fanex.ExternalAccount.SearchAndCopy.Models
{
    public class NG2Status
    {
        public NG2Status()
        {
        }

        public NG2Status(bool isSync, bool isDisabled)
        {
            IsSync = isSync;
            IsDisabled = isDisabled;
        }

        public bool IsSync { get; }

        public bool IsDisabled { get; }
    }
}
