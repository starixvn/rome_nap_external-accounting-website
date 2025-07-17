namespace Fanex.ExternalAccount.Service.Customer.CustomerList
{
    using System.Linq;
    using Fanex.ExternalAccount.Controllers.ViewModels.CustomerList;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerList;
    using Fanex.ExternalAccount.DataAccess.Customer.CustomerList;
    using Fanex.ExternalAccount.Service.Customer.CustomerList.Params;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Utilities;

    public interface ICustomerListService
    {
        SearchCustomerListViewModel Search(SearchCustomerListParams parameters, bool IsViewIPVN);
    }

    public class CustomerListService : ICustomerListService
    {
        private readonly ICustomerListDao _customerListDao;

        private readonly IIPInformatonService _IPInformatonService;

        private readonly IGroupService _groupService;

        public CustomerListService() : this(null, null, null) { }

        public CustomerListService(ICustomerListDao customerListDao, IIPInformatonService ipInformatonService, IGroupService groupService)
        {
            _IPInformatonService = ipInformatonService ?? new IPInformationService();
            _customerListDao = customerListDao ?? new CustomerListDao();
            _groupService = groupService ?? new GroupService();
        }

        public SearchCustomerListViewModel Search(SearchCustomerListParams parameters, bool IsViewIPVN)
        {
            if(Config.IsMultipleCurrencyGroup(parameters.GroupName) && string.IsNullOrWhiteSpace(parameters.CurrencyIds))
            {
                return new SearchCustomerListViewModel();
            }

            var result = _customerListDao.SearchCustomerList(parameters.IsDirectMember ? GetSearchDownLineCriteria(parameters) : GetSearchCriteria(parameters));
            result.HasSubGroups = parameters.GroupId > 0 && _groupService.GetGroupWithSubGroups(parameters.GroupId).Any();
            if (!IsViewIPVN)
            {
                foreach (var item in result.CustomerList)
                {
                    if (!string.IsNullOrWhiteSpace(item.LastLoginIP) && _IPInformatonService.IsSpecialIP(item.LastLoginIP))
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
                        Currencies = parameters.CurrencyIds,
                        UserId = parameters.UserId,
                        Status = parameters.Status,
                        IsNoDownlines = parameters.IsNoDownlines,
                        PageIndex = parameters.PageIndex,
                        PageSize = parameters.PageSize
                    };
                case Role.Master:
                    return new SearchAgentListCriteria
                    {
                        MRecommend = parameters.UplineId,
                        UserName = parameters.UserName,
                        Currencies = parameters.CurrencyIds,
                        UserId = parameters.UserId,
                        Status = parameters.Status,
                        IsNoDownlines = parameters.IsNoDownlines,
                        PageIndex = parameters.PageIndex,
                        PageSize = parameters.PageSize
                    };
                case Role.Agent:
                    return new SearchMemberListCriteria
                    {
                        UplineID = parameters.UplineId,
                        UserName = parameters.UserName,
                        Currencies = parameters.CurrencyIds,
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
                        Currencies = parameters.CurrencyIds,
                        UserId = parameters.UserId,
                        Status = parameters.Status,
                        IsDataLimited = parameters.IsDataLimited,
                        IsNoDownlines = parameters.IsNoDownlines,
                        PageIndex = parameters.PageIndex,
                        PageSize = parameters.PageSize
                    };
            }
        }

        private SearchCustomerListBaseCriteria GetSearchDownLineCriteria(SearchCustomerListParams parameters)
        {
            return new SearchMemberListCriteria
            {
                UplineID = parameters.UplineId,
                UserName = parameters.UserName,
                Currencies = parameters.CurrencyIds,
                UserId = parameters.UserId,
                Status = parameters.Status,
                ServiceType = parameters.ServiceType,
                PageIndex = parameters.PageIndex,
                PageSize = parameters.PageSize
            };
        }
    }
}
