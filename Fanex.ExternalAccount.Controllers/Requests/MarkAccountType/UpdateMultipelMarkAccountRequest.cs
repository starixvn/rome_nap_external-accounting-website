namespace Fanex.ExternalAccount.Controllers.Requests.MarkAccountType
{
    using System.Collections.Generic;

    public class UpdateMultipelMarkAccountRequest
    {
        public IEnumerable<UpdateMarkAccountRequest> UpdateAccounts { get; set; }
    }
}
