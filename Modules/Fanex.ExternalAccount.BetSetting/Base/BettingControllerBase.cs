namespace Fanex.CustomerSetting.Module.BetSetting.Base
{
    using Services.Impl;
    using Services.Interface;
    using ViewModels;
    using Casino.Services.Impl;
    using Core.Web.Controllers;
    using Keno.KenoSetting.Services;
    using Casino.Services.Interface;

    public class BettingControllerBase : BaseController
    {
        private readonly PTEngine.Services.IBettingLimitService _sportsBookService = new PTEngine.Services.BettingLimitService();        
        
        private readonly IKenoSettingService _kenoService = new KenoSettingService();
        
        public PTEngine.Services.IBettingLimitService SportsBookService => _sportsBookService ?? new PTEngine.Services.BettingLimitService();        

        public SuperServices superCasinoService => new SuperServicesImpl();

        public CasinoServiceBase casinoServiceBaseImpl => new CasinoServiceBaseImpl();

        public IKenoSettingService KenoService => _kenoService ?? new KenoSettingService();

        public Customer CustomerInfo(int id, int userId)
        {
            ICustomerService customerService = new CustomerService();

            var customer = customerService.UserInfo(id);
            customer.GroupId = customerService.UserGroupId(userId);

            return customer;
        }
    }
}