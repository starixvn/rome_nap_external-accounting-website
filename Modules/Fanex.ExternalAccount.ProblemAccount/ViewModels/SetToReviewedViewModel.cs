namespace Fanex.ExternalAccount.ProblemAccount.ViewModels
{
    using Fanex.ExternalAccount.Core.Context;

    public class SetToReviewedViewModel
    {
        public string GroupName
            => ExtRequest.Instance.GetGroupName();

        public string CustIds { get; set; }

        public byte RoleId { get; set; }

        public bool IsAffectedAllDownlines { get; set; }

        public int UserId { get; set; }

        public string IP { get; set; }
    }
}
