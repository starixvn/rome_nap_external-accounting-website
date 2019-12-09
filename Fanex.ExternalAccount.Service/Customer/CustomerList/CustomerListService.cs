namespace Fanex.ExternalAccount.Service.Customer.CustomerList
{
    using Fanex.ExternalAccount.Controllers.ViewModels.CustomerList;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerList;
    using Fanex.ExternalAccount.DataAccess.Customer.CustomerList;
    using Fanex.ExternalAccount.DTO.Customer.CustomerList;
    using Fanex.ExternalAccount.Service.Customer.CustomerList.Params;

    public interface ICustomerListService
    {
        SearchCustomerListViewModel Search(SearchCustomerListParams parameters, bool IsViewIPVN);

        CustomerStatusDTO GetCustomerAllStatus(int custId);
    }

    public class CustomerListService : ICustomerListService
    {
        private readonly ICustomerListDao _customerListDao;

        public CustomerListService() : this(null) { }

        public CustomerListService(ICustomerListDao customerListDao) => _customerListDao = customerListDao ?? new CustomerListDao();

        public SearchCustomerListViewModel Search(SearchCustomerListParams parameters, bool IsViewIPVN)=> _customerListDao.SearchCustomerList(GetSearchCriteria(parameters),IsViewIPVN);

        public CustomerStatusDTO GetCustomerAllStatus(int custId) => _customerListDao.GetCustomerAllStatus(new GetCustomerAllStatusCriteria { CustId = custId });

        private SearchCustomerListBaseCriteria GetSearchCriteria(SearchCustomerListParams parameters)
        {
            switch (parameters.UplineRoleId)
            {
                case Role.Super:
                    return new SearchMasterListCriteria
                    {
                        SRecommend = parameters.UplineId,
                        UserName = parameters.UserName,
                        UserId = parameters.UserId,
                        Status = parameters.Status,
                        PageIndex = parameters.PageIndex,
                        PageSize = parameters.PageSize
                    };
                case Role.Master:
                    return new SearchAgentListCriteria
                    {
                        MRecommend = parameters.UplineId,
                        UserName = parameters.UserName,
                        UserId = parameters.UserId,
                        Status = parameters.Status,
                        PageIndex = parameters.PageIndex,
                        PageSize = parameters.PageSize
                    };
                case Role.Agent:
                    return new SearchMemberListCriteria
                    {
                        Recommend = parameters.UplineId,
                        UserName = parameters.UserName,
                        UserId = parameters.UserId,
                        Status = parameters.Status,
                        PageIndex = parameters.PageIndex,
                        PageSize = parameters.PageSize
                    };
                default:
                    return new SearchSuperListCriteria
                    {
                        GroupName = parameters.GroupName,
                        UserName = parameters.UserName,
                        UserId = parameters.UserId,
                        Status = parameters.Status,
                        IsDataLimited = parameters.IsDataLimited,
                        PageIndex = parameters.PageIndex,
                        PageSize = parameters.PageSize
                    };
            }
        }
    }
}
