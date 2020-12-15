namespace Fanex.ExternalAccount.Controllers.Requests
{
    using Core.Context;
    using Service.Interface.Report;

    public class ViewExtraPTRequest : IViewExtraPTRequest
    {
        private readonly IExtRequest extRequest;

        public ViewExtraPTRequest() : this(null)
        {
        }

        public ViewExtraPTRequest(IExtRequest extRequest)
        {
            this.extRequest = extRequest ?? new ExtRequest();
        }

        public int GroupId => extRequest.GetGroupId();

        public string GroupName => extRequest.GetGroupName();

        public int UserId { get; set; }

        public string ParentGroupName { get; set; }
    }
}