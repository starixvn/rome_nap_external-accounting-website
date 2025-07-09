namespace Fanex.ExternalAccount.Controllers.ViewModels.Customer
{
    public class EditInfoCashMemberViewModel
    {
        public EditInfoCashMemberViewModel(
            string assetsVersion,
            int custId,
            string username,
            bool isCashMember,
            string applicationPath,
            string cdnUri)
        {
            AssetsVersion = assetsVersion;
            CustId = custId;
            Username = username;
            IsCashMember = isCashMember;
            ApplicationPath = applicationPath;
            CdnUri = cdnUri;
        }

        public string AssetsVersion { get; }

        public bool IsCashMember { get; }

        public int CustId { get; }

        public string Username { get; }

        public string CdnUri { get; }
        
        public string ApplicationPath { get; }
    }
}