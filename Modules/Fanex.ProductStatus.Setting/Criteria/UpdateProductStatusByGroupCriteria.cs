using Fanex.Data.Repository;

namespace Fanex.ProductStatus.Setting.Criteria
{
    public class UpdateProductStatusByGroupCriteria : IBaseNonQueryCommand
    {
        public UpdateProductStatusByGroupCriteria(
            int custId,
            string isDisabled,
            int userId,
            string ip,
            int groupId,
            byte settingStatus)
        {
            CustId = custId;
            IsDisabled = isDisabled;
            UserId = userId;
            IP = ip;
            GroupId = groupId;
            SettingStatus = settingStatus;
        }

        public int CustId { get; }

        public string IsDisabled { get; }

        public int UserId { get; }

        public string IP { get; }

        public int GroupId { get; }

        public byte SettingStatus { get; }

        public int ErrCode { get; set; }

        public int MsgType { get; set; }

        public string ErrMsg { get; set; }

        public string GetSettingKey() => "Acc_Admin_DisableNewProductStatus_160_Upd";

        public bool IsValid() => CustId > 0 && UserId > 0 && GroupId > 0;
    }
}