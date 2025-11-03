namespace Fanex.ProductStatus.Setting.Criteria
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using Models;

    internal class UpdateDCSProductStatusCriteria : IBaseNonQueryCommand
    {
        private readonly int custId;
        private readonly int actorId;
        private readonly string ip;
        private readonly IEnumerable<ProductStatusCommand> productStatusCommands;
        private readonly int? settingType;
        private readonly byte settingStatus;
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }
        public int MsgType { get; set; }

        public UpdateDCSProductStatusCriteria(
            int custId,
            int actorId,
            string ip,
            IEnumerable<ProductStatusCommand> commands,
            byte settingStatus,
            int? settingType = null)
        {
            this.custId = custId;
            this.actorId = actorId;
            this.ip = ip;
            this.settingStatus = settingStatus;

            productStatusCommands = commands;
            this.settingType = settingType;
        }

        public string GetSettingKey() => "Acc_Admin_DisableNewProductStatusUpd";

        public bool IsValid()
            => custId > 0 && actorId > 0;

        public Hashtable GetSpParams() => new Hashtable
        {
            ["custid"] = custId,
            ["isdisabled"] = GetIsDisabledXmlParam(),
            ["userid"] = actorId,
            ["ip"] = ip,
            ["settingtype"] = settingType,
            ["settingstatus"] = settingStatus
        };

        private string GetIsDisabledXmlParam()
        {
            var builder = new StringBuilder();
            builder.Append("<ROOT>");

            foreach (var item in productStatusCommands)
            {
                builder.Append(BuildProductStatusElement(item));
            }

            builder.Append("</ROOT>");

            return builder.ToString();
        }

        private static string BuildProductStatusElement(ProductStatusCommand command)
        {
            return string.Format(
                "<ProductStatus SportType=\"{0}\" BetType=\"{1}\" IsDisabled=\"{2}\" UpdateAllMasters=\"{3}\" UpdateAllAgents=\"{4}\" UpdateAllMembers=\"{5}\""
                + " DisableAllMasters=\"{6}\" DisableAllAgents=\"{7}\" DisableAllMembers=\"{8}\"/>",
                command.SportType,
                command.BetType,
                Convert.ToInt32(command.Disable),
                Convert.ToInt32(command.UpdateAllMasters),
                Convert.ToInt32(command.UpdateAllAgents),
                Convert.ToInt32(command.UpdateAllMembers),
                Convert.ToInt32(command.DisableAllMasters),
                Convert.ToInt32(command.DisableAllAgents),
                Convert.ToInt32(command.DisableAllMembers));
        }
    }
}
