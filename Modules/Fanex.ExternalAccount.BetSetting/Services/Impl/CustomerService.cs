namespace Fanex.CustomerSetting.Module.BetSetting.Services.Impl
{
    using Fanex.CustomerSetting.Module.BetSetting.Repositories.Impl;
    using Fanex.CustomerSetting.Module.BetSetting.Repositories.Interface;
    using Fanex.CustomerSetting.Module.BetSetting.Services.Interface;
    using Fanex.CustomerSetting.Module.BetSetting.ViewModels;

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