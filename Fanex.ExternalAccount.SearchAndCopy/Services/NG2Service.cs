namespace Fanex.ExternalAccount.SearchAndCopy.Services
{
    using Fanex.ExternalAccount.SearchAndCopy.Criteria;
    using Fanex.ExternalAccount.SearchAndCopy.Models;
    using Fanex.ExternalAccount.SearchAndCopy.Repositories;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.Module.NG2.Entities;
    using Fanex.Module.NG2.Repository;
    using Fanex.Module.NG2.Services;
    using System;

    public interface ING2Service
    {
        void SyncCustomer(CloneSettingContext context);

        NG2Status GetStatus(int custId);
    }

    public class NG2Service : ING2Service
    {
        private readonly ING2Repository ng2Repository;
        private readonly ING2StatusService ng2StatusService;

        public NG2Service()
            :this(null, null)
        {
        }

        public NG2Service(ING2Repository ng2Repository, ING2StatusService ng2StatusService)
        {
            this.ng2Repository = ng2Repository ?? new NG2Repository();
            this.ng2StatusService = ng2StatusService ?? new NG2StatusService();
        }

        public NG2Status GetStatus(int custId)
        {
            var criteria = new GetNG2StatusCriteria(custId);
            var status = ng2Repository.GetStatus(criteria);
            var isSync = status.IsSync || (bool)BaseRepository.CheckAllowedCurrencyAll(custId).Outputs["allowedng2"];
            var isDisabled = status.IsDisabled;
            return new NG2Status(isSync, isDisabled);
        }

        public void SyncCustomer(CloneSettingContext context)
        {
            var result = ng2StatusService.AutoInsertAccount(
                context.CustId,
                context.Request.Username,
                RoleId.Super,
                context.CustId,
                context.NG2Status.IsDisabled,
                SiteId.NAP,
                context.ExistingCustomerInfo.CurrencyId,
                context.CurrentSession.UserId,
                (byte)context.CurrentSession.FromSystem,
                Config.SettingStatus);
            if (!result.IsSuccess)
            {
                throw new Exception(result.ErrorMessage);
            }
        }
    }
}
