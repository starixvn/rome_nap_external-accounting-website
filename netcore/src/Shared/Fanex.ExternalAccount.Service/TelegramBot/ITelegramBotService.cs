namespace Fanex.ExternalAccount.Service.TelegramBot
{
    using global::TelegramBot.Client.Models;
    using System.Threading.Tasks;

    public interface ITelegramBotService
    {
        void NotifyChangePassword(int custId, int subAccId);

        Task NotifyChangePasswordAsync(int custId, int subAccId);

        void NotifyChangeSecurityCode(int custId, int subAccId);

        Task NotifyChangeSecurityCodeAsync(int custId, int subAccId);

        Task<CustBotMessenger> GetMessenger(int custId, int subAccId);

        bool IsEnabledOTPViaTelegram(CustBotMessenger messenger);

        Task<Result> DisableSendOTPViaTelegram(int userId, int botId, int actorId, string ip);
    }
}
