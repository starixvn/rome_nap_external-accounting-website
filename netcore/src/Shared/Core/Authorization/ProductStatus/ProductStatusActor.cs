namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
{
    using Context;
    // using Fanex.ProductStatus.Setting.Services;
    using Utilities;

    public class ProductStatusActor : IProductStatusActor
    {
        // Stub implementation - external library not available
        public int Id => CurrentSession.Instance.UserId;

        public string Name => CurrentSession.Instance.UserName;

        public string ClientIP => CurrentContext.Instance.Ip4Address;

        public byte SystemId { get; } = (byte)Config.SystemId;

        public byte SettingStatus => Config.SettingStatus;
    }

    public interface IProductStatusActor
    {
        int Id { get; }
        string Name { get; }
        string ClientIP { get; }
        byte SystemId { get; }
        byte SettingStatus { get; }
    }
}
