namespace Fanex.CustomerSetting.Module.BetSetting.Services.Impl
{
    using Repositories.Impl;
    using Repositories.Interface;
    using Interface;
    using ViewModels;

    public class CustomerService : ICustomerService
    {
        public Customer UserInfo(int custId)
        {
            ICustomerDao dao = new CustomerDao();

            return dao.UserInfo(custId);
        }

        public int UserGroupId(int custId)
        {
            ICustomerDao dao = new CustomerDao();

            return dao.UserGroupId(custId);
        }
    }
}