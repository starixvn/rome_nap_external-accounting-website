namespace Fanex.CustomerSetting.Module.BetSetting.Base
{
    using Services.Impl;
    using Services.Interface;
    using ViewModels;
    using Casino.Services.Impl;
    using Core.Web.Controllers;
    using Keno.KenoSetting.Services;
    using LiveCasinoSetting.Business;

    public class BettingControllerBase : BaseController
    {
        private readonly PTEngine.Services.IBettingLimitService _sportsBookService = new PTEngine.Services.BettingLimitService();

        private readonly PTRacing.Services.ICustomerService _racingService = new PTRacing.Services.CustomerService();

        private readonly IMemberWinLimitService _memberWinLimitService = new MemberWinLimitService();

        private readonly IGameSettingService _gameSettingService = new GameSettingService();

        private readonly ISettingService _settingService = new SettingService();

        private readonly ILogWriterService _logService = new LogWriterService();

        private readonly IKenoSettingService _kenoService = new KenoSettingService();

        public ILogWriterService LogService => _logService ?? new LogWriterService();

        public ISettingService SettingService => _settingService ?? new SettingService();

        public PTEngine.Services.IBettingLimitService SportsBookService => _sportsBookService ?? new PTEngine.Services.BettingLimitService();

        public PTRacing.Services.ICustomerService RacingService => _racingService ?? new PTRacing.Services.CustomerService();

        public CasinoManager CasinoService => CasinoManager.Instance;

        public IMemberWinLimitService MemberWinLimitService => _memberWinLimitService ?? new MemberWinLimitService();

        public IGameSettingService GameSettingService => _gameSettingService ?? new GameSettingService();

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