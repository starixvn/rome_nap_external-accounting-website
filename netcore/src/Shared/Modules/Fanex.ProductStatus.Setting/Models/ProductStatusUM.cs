namespace Fanex.ProductStatus.Setting.Models
{
    using System;

    public class ProductStatusUM
    {
        public ProductStatusUM(
            int custId,
            int productId,
            int sportType,
            int betType,
            byte settingType,
            DateTime beginTime,
            DateTime endTime)
        {
            CustId = custId;
            ProductId = productId;
            SportType = sportType;
            BetType = betType;
            SettingType = settingType;
            BeginTime = beginTime;
            EndTime = endTime;
        }

        public int CustId { get; }

        public int ProductId { get; }

        public int SportType { get; }

        public int BetType { get; }

        public byte SettingType { get; }

        public DateTime BeginTime { get; }

        public DateTime EndTime { get; }
    }
}
