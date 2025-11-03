namespace Fanex.ExternalAccount.DataAccess.Criteria.GroupConfiguration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateGroupConfigurationCriteria : INonQueryCommand
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
                foreach (var detail in GroupDetails)
                {
                    details.AppendFormat(template, detail.ProductId, detail.GroupId, detail.IsShow);
                }

                details.Append("</ROOT>");

                return details.ToString();
            }
        }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string GetSettingKey() => "EAS_GroupConfigurationUpd";

        public bool IsValid() => UserId > 0 && GroupDetails.Any();
    }
}