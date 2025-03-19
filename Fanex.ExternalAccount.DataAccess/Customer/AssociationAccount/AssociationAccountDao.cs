namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using Criteria.Customer;
    using DTO.Customer;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.Utilities;

    public interface IAssociationAccountDao
    {
        AssociationAccountViewModel Get(GetAssociationAccountCriteria criteria);
    }

    public class AssociationAccountDao : IAssociationAccountDao
    {
        private readonly IDynamicRepository _dynamicRepository;

        public AssociationAccountDao()
            : this(null)
        {
        }

        public AssociationAccountDao(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository?? new DynamicRepository();
        }

        public AssociationAccountViewModel Get(GetAssociationAccountCriteria criteria)
        {
            var accounts = new AssociationAccountViewModel();
            accounts.Accounts = _dynamicRepository.Fetch<AssociationAccountModel>(criteria);
            accounts.PageIndex = criteria.PageIndex;
            accounts.TotalAccount = criteria.TotalAccount;
            accounts.TotalRows = criteria.TotalRows;
            return accounts;
        }
    }
}