namespace Fanex.ExternalAccount.Service.Interface.Report
{
    public interface IViewExtraPTRequest
    {
        int GroupId { get; }

        string GroupName { get; }

        int UserId { get; set; }

        string ParentGroupName { get; set; }
    }
}