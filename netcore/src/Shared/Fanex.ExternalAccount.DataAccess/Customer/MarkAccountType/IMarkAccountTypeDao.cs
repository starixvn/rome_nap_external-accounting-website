namespace Fanex.ExternalAccount.DataAccess.Customer.MarkAccountType
{
    using Fanex.ExternalAccount.DataAccess.Criteria.Customer.MarkAccountType;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.ViewModel.Customer.MarkAccountType;

    public interface IMarkAccountTypeDao
    {
        MarkAccountViewModel SearchMarkAccount(SearchMarkAccountCriteria criteria);

        Result Check(CheckMarkAccountCriteria criteria);

        Result Add(AddMarkAccountCommand command);

        Result Update(UpdateMarkAccountCommand command);

        Result Unmark(UnmarkMarkAccountCommand command);
    }
}
