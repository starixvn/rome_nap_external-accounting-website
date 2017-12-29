namespace Fanex.CustomerSetting.Module.BetSetting.Base
{
    using Fanex.CustomerSetting.Module.BetSetting.Services.Impl;
    using Fanex.CustomerSetting.Module.BetSetting.Services.Interface;
    using Fanex.CustomerSetting.Module.BetSetting.ViewModels;
    using Fanex.CustomerSetting.Module.Casino.Services.Impl;
    using Fanex.CustomerSetting.Module.Core.Web.Controllers;
    using Fanex.CustomerSetting.Module.Keno.KenoSetting.Services;
    using Fanex.LiveCasinoSetting.Business;

    public class BettingControllerBase : BaseController
    {
        private Fanex.PTEngine.Services.IBettingLimitService _sportsBookService = new Fanex.PTEngine.Services.BettingLimitService();
        
        private Fanex.PTRacing.Services.ICustomerService _racingService = new Fanex.PTRacing.Services.CustomerService();
        
        private IMemberWinLimitService _memberWinLimitService = new MemberWinLimitService();
        
        private IGameSettingService _gameSettingService = new GameSettingService();

        private ISettingService _settingService = new SettingService();

        private ILogWriterService _logService = new LogWriterService();

        private IKenoSettingService _kenoService = new KenoSettingService();

        public ILogWriterService LogService
        {
            get
            {
                return this._logService ?? new LogWriterService();
            }
        }

        public ISettingService SettingService
        {
            get
            {
                return this._settingService ?? new SettingService();
            }
        }

        public Fanex.PTEngine.Services.IBettingLimitService SportsBookService
        {
            get
            {
                return this._sportsBookService ?? new Fanex.PTEngine.Services.BettingLimitService();
            }
        }

        public Fanex.PTRacing.Services.ICustomerService RacingService
        {
            get
            {
                return this._racingService ?? new Fanex.PTRacing.Services.CustomerService();
            }
        }

        public CasinoManager CasinoService
        {
            get
            {
                return CasinoManager.Instance;
            }
        }

        public IMemberWinLimitService MemberWinLimitService
        {
            get
            {
                return this._memberWinLimitService ?? new MemberWinLimitService();
            }
        }

        public IGameSettingService GameSettingService
        {
            get
            {
                return this._gameSettingService ?? new GameSettingService();
            }
        }

        public IKenoSettingService KenoService
        {
            get
            {
                return this._kenoService ?? new KenoSettingService();
            }
        }

        public Customer CustomerInfo(int id, int userId)
        {
            ICustomerService customerService = new CustomerService();

            var customer = customerService.UserInfo(id);
            customer.GroupId = customerService.UserGroupId(userId);

            return customer;
        }
    }
}