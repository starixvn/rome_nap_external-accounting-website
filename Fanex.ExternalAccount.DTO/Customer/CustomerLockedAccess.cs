namespace Fanex.ExternalAccount.DTO.Customer
{
    public sealed class CustomerLockedAccess
    {
        private const byte None = 0;
        private const byte FalseInputOTPSC = 1; // blue: #86c5e4
        private const byte FalseInputPassword = 2; // gray: #ccc7ba

        public CustomerLockedAccess(byte locked)
        {
            Locked = locked;
            AssignParameters();
        }

        public byte Locked { get; }

        public bool IsLocked { get; private set; }

        public string ClassName { get; private set; }

        public string Reason { get; private set; }

        public string UnlockConfirmMessage { get; private set; }

        private void AssignParameters()
        {
            IsLocked = Locked != None;

            if (Locked == FalseInputOTPSC)
            {
                ClassName = "locked-falseOTPSC";
                Reason = "This account was locked by false input of OTP/Security Code.";
                UnlockConfirmMessage = "This account was locked by false input of OTP/Security Code. Are you sure you want to unlock this account?";
            }
            else if (Locked == FalseInputPassword)
            {
                ClassName = "locked-falsePassword";
                Reason = "This account was locked by failed login.";
                UnlockConfirmMessage = "This account was locked by false input Password. Are you sure you want to unlock this account?";
            }
        }
    }
}
