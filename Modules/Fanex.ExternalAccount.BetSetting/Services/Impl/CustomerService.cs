namespace Fanex.CustomerSetting.Module.BetSetting.Services.Impl
{
    using Fanex.CustomerSetting.Module.BetSetting.Criteria;
    using Interface;
    using Repositories.Impl;
    using Repositories.Interface;
    using ViewModels;

    public class CustomerService : ICustomerService
    {
        public Customer UserInfo(int custId)
        {
            ICustomerDao dao = new CustomerDao();

            return dao.UserInfo(new GetUserInfoCriteria(custId));
        }

        public int UserGroupId(int custId)
        {
            ICustomerDao dao = new CustomerDao();

            return dao.UserGroupId(custId);
        }
    }
}