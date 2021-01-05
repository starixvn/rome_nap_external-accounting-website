namespace Fanex.ExternalAccount.HeadlineBonus.Constants
{
    internal static class BonusStatusIdConst
    {
        public const byte Pending = 0;

        public const byte Cancelled = 1;

        public const byte Completed = 2;

        public static string GetBonusStatus(byte statusId)
        {
            switch (statusId)
            {
                case Pending:
                    return nameof(Pending);

                case Cancelled:
                    return nameof(Cancelled);

                case Completed:
                    return nameof(Completed);

                default:
                    return string.Empty;
            }
        }
    }
}
