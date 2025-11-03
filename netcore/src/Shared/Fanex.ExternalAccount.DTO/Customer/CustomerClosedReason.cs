namespace Fanex.ExternalAccount.DTO.Customer
{
    using System;

    public sealed class CustomerClosedReason
    {
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
            const byte byAdmin = 1; // purple: #b3a2c7
            const byte byUpline = 2; // orange: #f79646
            const byte byArchive = 4; // dark grey hi-light: #969696

            ClassName = "closed-";

            switch (closedReason)
            {
                case byAdmin:
                    ClassName += "byAdmin";
                    ClosedReason = "This account was closed by admin.";
                    break;

                case byUpline:
                    ClassName += "byUpline";
                    ClosedReason = "This account was closed by upline.";
                    break;

                case byArchive:
                    ClassName += "byArchive";
                    ClosedReason = "This account was closed by inactivity.";
                    break;

                default:
                    ClassName = string.Empty;
                    ClosedReason = string.Empty;
                    break;
            }
        }
    }
}