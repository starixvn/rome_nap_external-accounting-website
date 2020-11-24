namespace Fanex.ExternalAccount.Controllers.Requests
{
    using Core.Context;
    using Service.UserControl;
    using System;
    using System.Collections.Generic;

    public class SuperWinLossDetailByPTRequest : WinLossDetailRequest
    {
        public SuperWinLossDetailByPTRequest() : this(null, null, null)
        {
        }

        public SuperWinLossDetailByPTRequest(
            Func<int, ICollection<KeyValuePair<string, bool>>> selectedProductQueryGetter,
            IExtRequest extRequest,
            IGroupConfigurationService groupConfigurationService) :
            base(selectedProductQueryGetter, extRequest, groupConfigurationService)
        {
        }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }
    }
}
