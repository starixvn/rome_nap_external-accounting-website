namespace Fanex.ExternalAccount.Controllers.Requests
{
    using Core.Context;

    public class SuperWinLossDetailByPTRequest : WinLossDetailRequest
    {
        public SuperWinLossDetailByPTRequest()
            : this(null, null)
        {
        }

        public SuperWinLossDetailByPTRequest(IExtRequest extRequest, CurrentSession currentSession)
            : base(extRequest, currentSession)
        {
        }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }
    }
}
