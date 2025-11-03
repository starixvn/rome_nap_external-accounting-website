namespace Fanex.CustomerSetting.Module.BetSetting.Repositories.Impl
{
    using System.Collections;
    using System.Configuration;
    using Common.Extensions;
    using Interface;
    using ViewModels;
    using Data;
    using ExternalAccount.Core.Shared;
    using Config = Fanex.ExternalAccount.Utilities.Config;
    using Fanex.Data.Repository;
    using Fanex.CustomerSetting.Module.BetSetting.Criteria;

    public class CustomerDao : ICustomerDao
    {
        private IDynamicRepository dynamicRepository;

        public CustomerDao() : this(null) { }

        public CustomerDao(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public Customer UserInfo(GetUserInfoCriteria criteria)
            => dynamicRepository.Get<Customer>(criteria);
    }
}