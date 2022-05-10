namespace Fanex.ExternalAccount.Service.TelegramBot
{
    using System.Threading.Tasks;

    public interface ITelegramBotService
    {
        void NotifyChangePassword(int custId, int subAccId, string ip);

        Task NotifyChangePasswordAsync(int custId, int subAccId, string ip);

        void NotifyChangeSecurityCode(int custId, int subAccId, string ip);

        Task NotifyChangeSecurityCodeAsync(int custId, int subAccId, string ip);
    }
}
