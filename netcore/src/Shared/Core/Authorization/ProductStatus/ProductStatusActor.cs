namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
{
    using Context;
    using Fanex.ProductStatus.Setting.Services;
    using Utilities;

    public class ProductStatusActor : IProductStatusActor
    {
        public int Id => CurrentSession.Instance.UserId;

        public string Name => CurrentSession.Instance.UserName;

        public string ClientIP => CurrentContext.Instance.Ip4Address;

        public byte SystemId { get; } = (byte)Config.SystemId;

        public byte SettingStatus => Config.SettingStatus;
    }
}
