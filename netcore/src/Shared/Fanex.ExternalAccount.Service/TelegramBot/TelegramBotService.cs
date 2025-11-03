namespace Fanex.ExternalAccount.Service.TelegramBot
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Fanex.Logging.Extensions.Web;
    using Fanex.ExternalAccount.Core.Shared;
    using System.Threading;
    using global::TelegramBot.Client.Services;
    using System.Linq;
    using global::TelegramBot.Constants;
    using global::TelegramBot.Client.Models;

    public class TelegramBotService : ITelegramBotService
    {
        private readonly INotificationService notificationService;
        private readonly ISettingService settingService;

        public TelegramBotService(INotificationService notificationService = null, ISettingService settingService=null)
        {
            this.notificationService = notificationService ?? new NotificationService(CreateTelegramBotHttpClient());
            this.settingService = settingService ?? new SettingService(CreateTelegramBotHttpClient());
        }

        private static HttpClient CreateTelegramBotHttpClient()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var telegramClient = HttpClientFactory.Create();
            telegramClient.BaseAddress = AppSettings.TelegramBotClientUri;
            telegramClient.DefaultRequestHeaders.Add("Accept-Language", Thread.CurrentThread.CurrentCulture.Name);

            return telegramClient;
        }

        public void NotifyChangePassword(int custId, int subAccId)
            => NotifyChangePasswordAsync(custId, subAccId).GetAwaiter().GetResult();

        public async Task NotifyChangePasswordAsync(int custId, int subAccId)
        {
            try
            {
                await notificationService.NotifyChangePassword(custId, subAccId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }

        public void NotifyChangeSecurityCode(int custId, int subAccId)
           => NotifyChangeSecurityCodeAsync(custId, subAccId).GetAwaiter().GetResult();

        public async Task NotifyChangeSecurityCodeAsync(int custId, int subAccId)
        {
            try
            {
                await notificationService.NotifyChangeSecurityCode(custId, subAccId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }

        public Task<CustBotMessenger> GetMessenger(int custId, int subAccId)
        {
            return settingService.GetCustBotMessengerAsync(custId, subAccId, "en-US");
        }

        public bool IsEnabledOTPViaTelegram(CustBotMessenger messenger)
        {
            return messenger != null && messenger.Functions.FirstOrDefault(f => f.FunctionId == FunctionIds.OTP).Enabled;
        }

        public Task<Result> DisableSendOTPViaTelegram(int userId,int botId, int actorId ,string ip)
        {
            return settingService.UpdateUserFunction(userId, botId, new Function { FunctionId = FunctionIds.OTP, Enabled = false }, actorId, ip);
        }
    }
}
