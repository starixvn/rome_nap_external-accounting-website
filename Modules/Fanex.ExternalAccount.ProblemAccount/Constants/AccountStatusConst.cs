namespace Fanex.ExternalAccount.ProblemAccount.Constants
{
    public static class AccountStatusConst
    {
        public const byte Open = 1;
        public const byte Suspended = 2;
        public const byte Closed = 3;
        public const byte Disabled = 4;

        public static string GetAccountStatusName(byte status)
        {
            switch (status)
            {
                case Open:
                    return nameof(Open);

                case Suspended:
                    return nameof(Suspended);

                case Closed:
                    return nameof(Closed);

                case Disabled:
                    return nameof(Disabled);

                default:
                    return string.Empty;
            }
        }

        public const int OpenRowIndex = 1;
        public const int SuspendedRowIndex = 3;
        public const int DisabledRowIndex = 7;
    }
}
