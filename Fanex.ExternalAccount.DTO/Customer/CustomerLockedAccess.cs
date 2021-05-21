namespace Fanex.ExternalAccount.DTO.Customer
{
    public sealed class CustomerLockedAccess
    {
        private const byte None = 0;
        private const byte FalseOTPSC = 1; // blue: #86c5e4
        private const byte FalsePassword = 2; // gray: #ccc7ba

        public CustomerLockedAccess(byte locked)
        {
            Locked = locked;
            AssignParameters();
        }

        public byte Locked { get; }

        public bool IsLocked => Locked != None;

        public string ClassName { get; private set; }

        public string Reason { get; private set; }

        private void AssignParameters()
        {
            if (Locked == FalseOTPSC)
            {
                ClassName = "locked-falseOTPSC";
                Reason = "This account has been locked due to multiple failed OTP/Security Code authentication attempts.";
            }
            else if (Locked == FalsePassword)
            {
                ClassName = "locked-falsePassword";
                Reason = "This account has been locked due to multiple failed password attempts.";
            }
        }
    }
}
