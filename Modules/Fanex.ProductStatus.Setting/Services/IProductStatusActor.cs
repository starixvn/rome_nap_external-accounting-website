namespace Fanex.ProductStatus.Setting.Services
{
    public interface IProductStatusActor
    {
        int Id { get; }

        string Name { get; }

        string ClientIP { get; }

        byte SystemId { get; }

        byte SettingStatus { get; }
    }
}
