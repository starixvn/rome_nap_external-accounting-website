namespace Fanex.ExternalAccount.DataAccess.Criteria.GroupConfiguration
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using NAP.Framework.Data;
    using System.Linq;

    public class UpdateGroupConfigurationCriteria : CriteriaBase
    {
        private const string template = "<Row ProductId='{0}' GroupId='{1}' IsShow='{2}' />";
        public int UserId { get; set; }

        [SpParam(Ignored = true)]
        public IEnumerable<GroupConfigurationDetail> GroupDetails { get; set; }

        public string Data
        {
            get
            {
                var details = new StringBuilder("<ROOT>");
                foreach(var detail in GroupDetails)
                {
                    details.AppendFormat(template, detail.ProductId, detail.GroupId, detail.IsShow);
                }

                details.Append("</ROOT>");

                return details.ToString();
            }
        }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey() => "EAS_GroupConfigurationUpd";

        public override bool IsValid() => UserId > 0 && GroupDetails.Any();
    }
}
