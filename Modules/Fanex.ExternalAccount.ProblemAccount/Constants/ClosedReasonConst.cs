namespace Fanex.ExternalAccount.ProblemAccount.Constants
{
    public static class ClosedReasonConst
    {
        public const byte ClosedWrongPwdOthers = 0;
        public const byte ClosedByAdmin = 1;
        public const byte ClosedByUpline = 2;
        public const byte ClosedWrongOTPSC = 3;

        public const int ClosedWrongPwdOthersRowIndex = 5;
        public const int ClosedByAdminRowIndex = 11;
        public const int ClosedByUplineRowIndex = 13;
        public const int ClosedWrongOTPSCRowIndex = 15;
    }
}
