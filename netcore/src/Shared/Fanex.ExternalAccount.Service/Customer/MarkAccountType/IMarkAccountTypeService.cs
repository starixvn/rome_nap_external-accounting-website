namespace Fanex.ExternalAccount.Service.Customer.MarkAccountType
{
    using System.Collections.Generic;
    using DTO;
    using Fanex.ExternalAccount.Service.Customer.MarkAccountType.Params;
    using ViewModel.Customer.MarkAccountType;

    public interface IMarkAccountTypeService
    {
        MarkAccountViewModel Search(SearchMarkAccountTypeParams parameters);

        Result Unmark(UnmarkAccountTypeParams parameters);

        Result Add(NewMarkAccountTypeParams newMarkAccountTypeParams);

        Result SingleUpdate(UpdateMarkAccountTypeParams updateMarkAccountTypeParams);

        Result MultipleUpdate(IEnumerable<UpdateMarkAccountTypeParams> updateAccountTypes);

        Result UnmarkAll(UnmarkAccountTypeParams unmarkAccountTypeParams);
    }
}
