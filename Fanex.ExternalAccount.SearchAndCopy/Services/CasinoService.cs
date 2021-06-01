namespace Fanex.ExternalAccount.SearchAndCopy.Services
{
    using CasinoLibrary.Model.UserException;
    using CasinoLibrary.SBAgent;
    using Fanex.CustomerSetting.Module.Casino.Services.Impl;
    using Fanex.CustomerSetting.Module.Casino.Services.Interface;
    using Fanex.ExternalAccount.SearchAndCopy.Criteria;
    using Fanex.ExternalAccount.SearchAndCopy.Models;
    using Fanex.ExternalAccount.SearchAndCopy.Repositories;
    using System;

    public interface ICasinoService
    {
        void SyncCustomer(CloneSettingContext context);
    }

    public class CasinoService : ICasinoService
    {
        private const string CasinoNo = "01";
        private readonly Customer casinoCustomer;
        private readonly SuperServices superServices;
        private readonly MemberWinLimitService memberWinLimitService;
        private readonly ICasinoRepository casinoRepository;

        public CasinoService()
            :this(null, null, null, null)
        {
        }

        public CasinoService(
            Customer casinoCustomer, 
            SuperServices superServices, 
            MemberWinLimitService memberWinLimitService,
            ICasinoRepository casinoRepository)
        {
            this.casinoCustomer = casinoCustomer ?? new Customer();
            this.superServices = superServices ?? new SuperServicesImpl();
            this.memberWinLimitService = memberWinLimitService ?? new MemberWinLimitServiceImpl();
            this.casinoRepository = casinoRepository ?? new CasinoRepository();
        }

        public void SyncCustomer(CloneSettingContext context)
        {
            var isEnabledJackpot = superServices.IsEnabledJackpot(context.ExistingCustId);
            var isSuccessful = casinoCustomer.AutoInsertSuperFromExisting(
                    CasinoNo,
                    context.CustId,
                    context.ExistingCustId,
                    context.Request.Username,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    context.Request.Password,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    context.ExistingCustomerInfo.CurrencyId,
                    isEnabledJackpot,
                    DateTime.Now,
                    context.CurrentContext.Ip4Address,
                    context.CurrentSession.UserId,
                    context.CurrentSession.UserName,
                    context.CurrentSession.FromSystem);
            if (!isSuccessful)
            {
                throw new ErrorException("AutoInsertSuperFromExisting failed");
            }

            var winLimit = memberWinLimitService.GetMemberWinLimit(context.ExistingCustId);
            memberWinLimitService.UpdateMemberWinLimit(context.CustId, context.CurrentSession.UserId, 0, winLimit);

            var criteria = new UpdateSyncCasinoForIBCCriteria(context.ExistingCustomerInfo.DisableCasino)
            {
                CustId = context.CustId,
                LoginIP = context.CurrentContext.Ip4Address,
                UserId = context.CurrentSession.UserId
            };
            var result = casinoRepository.UpdateSyncCasinoForIBC(criteria);
            if (!result.IsSuccess)
            {
                throw new Exception(result.ErrorMessage);
            }
        }
    }
}
