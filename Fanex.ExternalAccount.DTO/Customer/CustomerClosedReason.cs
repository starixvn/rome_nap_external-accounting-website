using System;

namespace Fanex.ExternalAccount.DTO.Customer
{
    public sealed class CustomerClosedReason
    {
        private const byte WrongPasswordOthers = 0; // gray: #ccc7ba
        private const byte ByAdmin = 1; // purple: #b3a2c7
        private const byte ByUpline = 2; // orange: #f79646
        private const byte WrongOTPSC = 3; // blue: #86c5e4

        public CustomerClosedReason(object closedReason)
        {
            if (closedReason != null && closedReason != DBNull.Value)
            {
                AssignParameters(Convert.ToByte(closedReason));
            }
        }

        public CustomerClosedReason(byte closedReason)
        {
            AssignParameters(closedReason);
        }

        public string ClassName { get; private set; }

        public string ClosedReason { get; private set; }

        private void AssignParameters(byte closedReason)
        {
            ClassName = "closed-";

            switch (closedReason)
            {
                case WrongPasswordOthers:
                    ClassName += "wrongPwdOthers";
                    ClosedReason = "This account was closed by failed login or account expired.";
                    break;

                case ByAdmin:
                    ClassName += "byAdmin";
                    ClosedReason = "This account was closed by admin.";
                    break;

                case ByUpline:
                    ClassName += "byUpline";
                    ClosedReason = "This account was closed by upline.";
                    break;

                case WrongOTPSC:
                    ClassName += "wrongOTPSC";
                    ClosedReason = "This account was closed by false input of OTP/Security Code.";
                    break;

                default:
                    ClassName = string.Empty;
                    ClosedReason = string.Empty;
                    break;
            }
        }
    }
}
