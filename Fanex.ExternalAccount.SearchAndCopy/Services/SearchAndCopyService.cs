namespace Fanex.ExternalAccount.SearchAndCopy.Services
{
    using Fanex.CustomerSetting.Module.Core.Web;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.SearchAndCopy.Criteria;
    using Fanex.ExternalAccount.SearchAndCopy.Models;
    using Fanex.ExternalAccount.SearchAndCopy.Repositories;
    using Fanex.ExternalAccount.SearchAndCopy.Requests;
    using IBCUtils;
    using System;
    using System.Linq;

    public interface ISearchAndCopyService
    {
        SearchCustomerModel SearchCustomer(string username, int roleId);

        Result CloneSetting(CloneSettingRequest request);

        //void SendEmail(CloneSettingContext context, string emailBody);
    }

    public class SearchAndCopyService : ISearchAndCopyService
    {
        private readonly ISearchAndCopyRepository searchAndCopyRepository;
        private readonly ICurrentSession currentSession;
        private readonly ICurrentContext currentContext;

        public SearchAndCopyService()
            : this(null, null, null)
        {
        }

        public SearchAndCopyService(ISearchAndCopyRepository searchAndCopyRepository, ICurrentSession currentSession, ICurrentContext currentContext)
        {
            this.searchAndCopyRepository = searchAndCopyRepository ?? new SearchAndCopyRepository();
            this.currentSession = currentSession;
            this.currentContext = currentContext;
        }

        public SearchCustomerModel SearchCustomer(string username, int roleId)
        {
            return searchAndCopyRepository.SearchCustomer(username, currentSession.CurrencyGroup)
                .FirstOrDefault(c => c.RoleId == roleId && username.Equals(c.Username, StringComparison.InvariantCultureIgnoreCase) && c.IsExistGroup);
        }

        public Result CloneSetting(CloneSettingRequest request)
        {
            var searchCustomer = SearchCustomer(request.ExistingUsername, Role.Super);
            if (searchCustomer == null)
            {
                return new Result(1, "Unauthorized");
            }

            var existingCustId = searchCustomer.CustId.Value;
            var cloneSettingComCriteria = new CloneSettingComCriteria
            {
                Username = request.Username,
                UserPwd = Encryption.CfsEnCode(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                MobilePhone = request.MobilePhone,
                Fax = request.LastName,
                Credit = request.Credit,
                ExistingCustId = existingCustId,
                Creator = currentSession.CustId,
                IP = currentContext.Ip4Address
            };
            var result = searchAndCopyRepository.CloneSettingComm(cloneSettingComCriteria);
            if (!result.IsSuccess)
            {
                return result;
            }

            var custId = Convert.ToInt32(result.ResultData);
            var cloneSettingExtraPTCriteria = new CloneSettingExtraPTCriteria
            {
                CustId = custId,
                ExistingCustId = existingCustId,
                Creator = currentSession.CustId,
                IP = currentContext.Ip4Address
            };
            result = searchAndCopyRepository.CloneSettingExtraPT(cloneSettingExtraPTCriteria);
            return result;
        }

        //public void SendEmail(CloneSettingContext context, string emailBody)
        //{
        //    try
        //    {
        //    }
        //    catch(Exception ex)
        //    {
        //        ExceptionHandler.Handle(ex);
        //    }
        //}
    }
}
