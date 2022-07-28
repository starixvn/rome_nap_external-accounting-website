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

    public class TelegramBotService : ITelegramBotService
    {
        private readonly INotificationService notificationService;

        public TelegramBotService(INotificationService notificationService = null)
        {
            this.notificationService = notificationService ?? new NotificationService(CreateTelegramBotHttpClient());
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
    }
}
