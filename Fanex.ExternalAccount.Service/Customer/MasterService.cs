namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using System.Collections.Generic;
    using System.Data;
    using DataAccess.Impl.Customer;
    using DataAccess.Interface.Customer;
    using DTO;
    using DTO.Customer;
    using Fanex.ExternalAccount.DataAccess.AutoTransfer;
    using Fanex.ExternalAccount.DataAccess.Criteria.AutoTransfer;
    using Fanex.ExternalAccount.DTO.AutoTransfer;
    using Interface.Customer;

    public class MasterService : CustomerService, IMasterService
    {
        private const int ErrorCode = 100;

        private readonly IMasterDao _dao = new MasterDao();
        private readonly ISiteDBConfigService siteDBService = new SiteDBConfigService();
        private readonly IAutoTransferDao autoTransferDao = new AutoTransferDao();

        public DataTable GetData(int? userId, int? recommend = null, int? custId = null, bool? enabled = null,
            bool? closed = null, bool? suspended = null)
            => _dao.GetData(userId, recommend, custId, enabled, closed, suspended);

        public IEnumerable<int> GetSuperInAutomateTransfer(int custId)
            => _dao.GetSuperInAutomateTransfer(custId);

        public Transfer GetTransferOption(int custId)
            => _dao.GetTransferOption(custId);

        public Result EditTransferOption(
            string transferOption,
            int autoTransferCondition,
            int custId,
            int userId,
            string ip,
            int subAccId,
            string language)
        {
            var siteDB = siteDBService.GetSiteDBConfigByCustId(custId);
            var autoTransferStatus = autoTransferDao.GetAutoTransferJobStatus(custId, siteDB);

            if (autoTransferStatus)
            {
                return new Result(ErrorCode, Resources.Lang.AutoTransferProcessTime);
            }

            var dbResult = _dao.EditTransferOption(transferOption, autoTransferCondition, custId, userId, ip, subAccId, language);
            var result = new Result(dbResult.Outputs["errcode"].ToString(), dbResult.Outputs["errmsg"].ToString(), dbResult.ResultSet);

            if (int.Parse(result.ErrorCode) != 0)
            {
                return result;
            }

            var rows = dbResult.ResultSet.Tables[0].Rows;

            if (rows.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in rows)
            {
                var uplineId = int.Parse(row["custId"].ToString());

                var toastMessageCriteria = new InsertToastMessageCriteria(
                    ToastMessageTypes.Warning,
                    ToastMessageSiteFunctionIds.ConditionChange,
                    uplineId,
                    userId,
                    new ToastContentDto(Resources.Lang.TransferCondition_ToastTitle_en, Resources.Lang.TransferCondition_ToastBody_en),
                    new ToastContentDto(Resources.Lang.TransferCondition_ToastTitle_ch, Resources.Lang.TransferCondition_ToastBody_ch),
                    new ToastContentDto(Resources.Lang.TransferCondition_ToastTitle_cs, Resources.Lang.TransferCondition_ToastBody_cs),
                    new ToastContentDto(Resources.Lang.TransferCondition_ToastTitle_th, Resources.Lang.TransferCondition_ToastBody_th),
                    new ToastContentDto(Resources.Lang.TransferCondition_ToastTitle_vn, Resources.Lang.TransferCondition_ToastBody_vn));

                autoTransferDao.InsertToastMessage(toastMessageCriteria);

                var uplineName = row["username"].ToString();
                var dowlines = row["names"].ToString();
                var personalMessageCriteria = new InsertPersonalMessageCriteria(
                    string.Format(Resources.Lang.TransferCondition_PersonalMessage_en, dowlines),
                    string.Format(Resources.Lang.TransferCondition_PersonalMessage_ch, dowlines),
                    string.Format(Resources.Lang.TransferCondition_PersonalMessage_cs, dowlines),
                    string.Format(Resources.Lang.TransferCondition_PersonalMessage_th, dowlines),
                    string.Format(Resources.Lang.TransferCondition_PersonalMessage_vn, dowlines));

                var messageId = autoTransferDao.InsertPersonalMessage(personalMessageCriteria);
                if (messageId > 0)
                {
                    autoTransferDao.AssignPersonalMessage(messageId, uplineId, uplineName);
                }
            }

            return result;
        }
    }
}