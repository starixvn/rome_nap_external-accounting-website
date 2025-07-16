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
            string cdnUri,
            int systemId)
        {
            AssetsVersion = assetsVersion;
            CustId = custId;
            Username = username;
            IsCashMember = isCashMember;
            ApplicationPath = applicationPath;
            CdnUri = cdnUri;
            SystemId = systemId;
        }

        public string AssetsVersion { get; }

        public bool IsCashMember { get; }

        public int CustId { get; }
        
        public int AgentId { get; set; }

        public string Username { get; }

        public string CdnUri { get; }
        
        public string ApplicationPath { get; }

        public int SystemId { get; }
    }
}