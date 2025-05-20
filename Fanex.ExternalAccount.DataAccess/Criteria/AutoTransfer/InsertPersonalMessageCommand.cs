using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.ExternalAccount.DTO.AutoTransfer;

namespace Fanex.ExternalAccount.DataAccess.Criteria.AutoTransfer
{
    public class InsertPersonalMessageCommand : INonQueryCommand
    {
        public InsertPersonalMessageCommand(
            string contentEN,
            string contentCH,
            string contentCS,
            string contentTH,
            string contentVN)
        {
            ContentEN = contentEN;
            ContentCH = contentCH;
            ContentCS = contentCS;
            ContentTH = contentTH;
            ContentVN = contentVN;

            CreatorName = string.Empty;
            MessageType = MessageTypeConstants.Personal;
            IsBatch = true;
        }

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

        [SpParam(SqlParamName = "msgtype")]
        public int MessageType { get; set; }

        public string CreatorName { get; }

        public bool IsBatch { get; }

        public int MessageIDOutput { get; set; }

        public string GetSettingKey() => "Age_Msg_SetMessage";

        public bool IsValid() => true;
    }
}
