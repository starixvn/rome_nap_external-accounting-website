using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.ExternalAccount.DTO.AutoTransfer;
using Newtonsoft.Json;

namespace Fanex.ExternalAccount.DataAccess.Criteria.AutoTransfer
{
    public class InsertToastMessageCommand : INonQueryCommand
    {
        public InsertToastMessageCommand(
            int typeId,
            int siteFunctionId,
            int targetId,
            int creator,
            ToastContentDto contentEN,
            ToastContentDto contentCH,
            ToastContentDto contentCS,
            ToastContentDto contentTH,
            ToastContentDto contentVN)
        {
            TypeId = typeId;
            SiteFunctionId = siteFunctionId;
            TargetId = targetId;
            Creator = creator;

            ContentEN = JsonConvert.SerializeObject(contentEN, Formatting.None);
            ContentCH = JsonConvert.SerializeObject(contentCH, Formatting.None);
            ContentCS = JsonConvert.SerializeObject(contentCS, Formatting.None);
            ContentTH = JsonConvert.SerializeObject(contentTH, Formatting.None);
            ContentVN = JsonConvert.SerializeObject(contentVN, Formatting.None);
        }

        public int TypeId { get; set; }

        public int SiteFunctionId { get; set; }

        public int TargetId { get; set; }

        public int Creator { get; set; }

        [SpParam(SqlParamName = "content_en")]
        public string ContentEN { get; set; }

        [SpParam(SqlParamName = "content_ch")]
        public string ContentCH { get; set; }

        [SpParam(SqlParamName = "content_cs")]
        public string ContentCS { get; set; }

        [SpParam(SqlParamName = "content_jp")]
        public string ContentJP { get; set; }

        [SpParam(SqlParamName = "content_it")]
        public string ContentIT => string.Empty;

        [SpParam(SqlParamName = "content_th")]
        public string ContentTH { get; set; }

        [SpParam(SqlParamName = "content_ko")]
        public string ContentKO { get; set; }

        [SpParam(SqlParamName = "content_es")]
        public string ContentES => string.Empty;

        [SpParam(SqlParamName = "content_vn")]
        public string ContentVN { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public int ToastId { get; set; }

        public string GetSettingKey() => "Age_ToastMessage_Ins";

        public bool IsValid() => true;
    }
}
