namespace Fanex.ExternalAccount.SearchAndCopy.Services
{
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.SearchAndCopy.Criteria;
    using Fanex.ExternalAccount.SearchAndCopy.Models;
    using Fanex.ExternalAccount.SearchAndCopy.Repositories;
    using Fanex.ExternalAccount.SearchAndCopy.Resources;
    using Fanex.Logging.Extensions.Web;
    using IBCUtils;
    using System;
    using System.Linq;
    using System.Threading;

    public interface ISearchAndCopyService
    {
        Result SearchCustomer(SearchCustomerContext context);

        Result CloneSetting(CloneSettingContext context);

        Result Sync3rdProducts(CloneSettingContext context);
    }

    public class SearchAndCopyService : ISearchAndCopyService
    {
        private readonly ISearchAndCopyRepository searchAndCopyRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly ING2Service ng2Service;
        private readonly ICasinoService casinoService;

        public SearchAndCopyService()
            : this(null, null, null, null)
        {
        }

        public SearchAndCopyService(
            ISearchAndCopyRepository searchAndCopyRepository,
            ICustomerRepository customerRepository,
            ING2Service ng2Service,
            ICasinoService casinoService)
        {
            this.searchAndCopyRepository = searchAndCopyRepository ?? new SearchAndCopyRepository();
            this.customerRepository = customerRepository ?? new CustomerRepository();
            this.ng2Service = ng2Service ?? new NG2Service();
            this.casinoService = casinoService ?? new CasinoService();
        }

        public Result SearchCustomer(SearchCustomerContext context)
        {
            var customer = searchAndCopyRepository.SearchCustomer(context.Request.Username, context.CurrentSession.CurrencyGroup)
                .FirstOrDefault(c => c.RoleId == context.RoleId && context.Request.Username.Equals(c.Username, StringComparison.InvariantCultureIgnoreCase) && c.IsExistGroup);
            if(customer == null)
            {
                return new Result(1, Lang.SearchSuperNotExisted);
            }

            if (!customer.IsValidSuperPrefix)
            {
                return new Result(2, Lang.SuperNotFollowPrefix);
            }


            return new Result(0, string.Empty, customer);
        }

        public Result CloneSetting(CloneSettingContext context)
        {
            var result = CloneCustomerAndSettings(context);
            if (!result.IsSuccess)
            {
                return result;
            }

            return Sync3rdProducts(context);
        }

        private Result CloneCustomerAndSettings(CloneSettingContext context)
        {
            var cloneSettingComCriteria = new CloneSettingComCriteria
            {
                Username = context.Request.Username,
                UserPwd = Encryption.CfsEnCode(context.Request.Password),
                FirstName = context.Request.FirstName,
                LastName = context.Request.LastName,
                Phone = context.Request.Phone,
                MobilePhone = context.Request.MobilePhone,
                Fax = context.Request.LastName,
                Credit = context.Request.Credit,
                ExistingCustId = context.ExistingCustId,
                Creator = context.CurrentSession.CustId,
                IP = context.CurrentContext.Ip4Address
            };
            context.Result = searchAndCopyRepository.CloneSettingComm(cloneSettingComCriteria);
            if (!context.Result.IsSuccess)
            {
                return context.Result;
            }

            context.CustId = Convert.ToInt32(context.Result.ResultData);
            var cloneSettingExtraPTCriteria = new CloneSettingExtraPTCriteria
            {
                CustId = context.CustId,
                ExistingCustId = context.ExistingCustId,
                Creator = context.CurrentSession.CustId,
                IP = context.CurrentContext.Ip4Address
            };
            context.Result = searchAndCopyRepository.CloneSettingExtraPT(cloneSettingExtraPTCriteria);
            if (!context.Result.IsSuccess)
            {
                return context.Result;
            }

            return context.Result;
        }

        public Result Sync3rdProducts(CloneSettingContext context)
        {
            try
            {
                context.NG2Status = ng2Service.GetStatus(context.ExistingCustId);
                context.ExistingCustomerInfo = customerRepository.GetCustomerInfoByUserId(context.ExistingCustId, false);
                if(!context.NG2Status.IsSync && !context.ExistingCustomerInfo.SyncCasino)
                {
                    return new Result(0, string.Empty);
                }

                Thread.Sleep(3000);
                if (context.NG2Status.IsSync)
                {
                    ng2Service.SyncCustomer(context);
                }

                if (context.ExistingCustomerInfo.SyncCasino)
                {
                    casinoService.SyncCustomer(context);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
                return new Result(1, Lang.SearchAndCopyFailureMessage);
            }

            return new Result(0, string.Empty);
        }
    }
}
