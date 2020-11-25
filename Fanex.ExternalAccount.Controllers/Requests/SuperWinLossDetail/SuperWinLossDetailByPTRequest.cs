namespace Fanex.ExternalAccount.Controllers.Requests
{
    using System;
    using System.Collections.Generic;
    using Core.Context;
    using Service.UserControl;

    public class SuperWinLossDetailByPTRequest : WinLossDetailRequest
    {
        public SuperWinLossDetailByPTRequest() : this(null, null, null, null)
        {
        }

        public SuperWinLossDetailByPTRequest(
            Func<int, ICollection<KeyValuePair<string, bool>>> selectedProductQueryGetter,
            IExtRequest extRequest,
            IGroupConfigurationService groupConfigurationService,
            CurrentSession currentSession) :
            base(selectedProductQueryGetter, extRequest, groupConfigurationService, currentSession)
        {
        }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }
    }
}
