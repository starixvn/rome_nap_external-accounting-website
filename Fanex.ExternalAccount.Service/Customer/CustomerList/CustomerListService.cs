namespace Fanex.ExternalAccount.Service.Customer.CustomerList
{
    using Fanex.ExternalAccount.Controllers.ViewModels.CustomerList;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerList;
    using Fanex.ExternalAccount.DataAccess.Customer.CustomerList;
    using Fanex.ExternalAccount.Service.Customer.CustomerList.Params;
    using Fanex.ExternalAccount.Utilities;

    public interface ICustomerListService
    {
        SearchCustomerListViewModel Search(SearchCustomerListParams parameters, bool IsViewIPVN);
    }

    public class CustomerListService : ICustomerListService
    {
        private readonly ICustomerListDao _customerListDao;

        private readonly IIPInformatonService _IPInformatonService;

        public CustomerListService() : this(null, null) { }

        public CustomerListService(ICustomerListDao customerListDao, IIPInformatonService ipInformatonService)
        {
            _IPInformatonService = ipInformatonService ?? new IPInformationService();
            _customerListDao = customerListDao ?? new CustomerListDao();
        }

        public SearchCustomerListViewModel Search(SearchCustomerListParams parameters, bool IsViewIPVN)
        {
            var result = _customerListDao.SearchCustomerList(GetSearchCriteria(parameters));
            if (!IsViewIPVN)
            {
                var specialIPList = _IPInformatonService.GetSpecialIPs();
                foreach (var item in result.CustomerList)
                {
                    if (!string.IsNullOrWhiteSpace(item.LastLoginIP) && specialIPList.IsExistedInList(item.LastLoginIP))
                    {
                        item.LastLoginIP = string.Empty;
                    }
                }
            }

            return result;
        }

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
