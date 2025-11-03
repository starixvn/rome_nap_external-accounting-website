namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Collections.Generic;
    using System.Data;
    using Core.Shared;
    using Extensions;

    public class PersonalInfo : DownlineList
    {
        public PersonalInfo(DataRow customerInfoRow)
        {
            var role = customerInfoRow.RoleId();
            var enabled = customerInfoRow.Enabled();
            var closed = customerInfoRow.Closed();
            var suspended = customerInfoRow.Suspended();
            var locked = customerInfoRow.Locked();

            CustId = ObjectInfo.CreateInstance("Cust Id", customerInfoRow.CustId());
            Fax = ObjectInfo.CreateInstance("Fax", customerInfoRow.Fax());
            Mobile = ObjectInfo.CreateInstance("Mobile", customerInfoRow.MobilePhone());
            Name = ObjectInfo.CreateInstance("Name", customerInfoRow.Name());
            Nickname = ObjectInfo.CreateInstance("Nickname", customerInfoRow.Nickname());
            Phone = ObjectInfo.CreateInstance("Phone", customerInfoRow.Phone());
            Headline = ObjectInfo.CreateInstance("Headline", customerInfoRow.Headline());
            RoleId = ObjectInfo.CreateInstance("Role Id", role);
            UplineRoleId = ObjectInfo.CreateInstance("Upline Role Id", role.DirectUplineLevel());
            Recommend = ObjectInfo.CreateInstance("Recommend", customerInfoRow.Recommend());
            CustList = new ObjectInfo
            {
                Key = "CustList",
                Value = $"{(role.IsNotMember() ? role.DirectDownlineLevel().DisplayRoleName() : string.Empty)} List"
            };
            Status = new ObjectInfo { Tooltip = customerInfoRow.StatusTooltip(enabled, closed, suspended, locked) };
            BackgroundColor = new ObjectInfo { Value = customerInfoRow.StatusBackground(enabled, closed, suspended, locked) };

            SetLineInfo(role, customerInfoRow);
        }

        internal void SetLineInfo(int role, DataRow customerInfoRow)
        {
            CurrentLevel = ObjectInfo.CreateInstance(text: role.DisplayRoleName(), value: customerInfoRow.CustomerName(role));

            Downlines = GetLines(customerInfoRow, role);

            if (role.IsNotSuper())
            {
                UplineId = ObjectInfo.CreateInstance(
                    text: $"{role.DirectUplineLevel().DisplayRoleName()} ID",
                    value: customerInfoRow.CustomerId(role.DirectUplineLevel()));
            }
        }

        internal static List<ObjectInfo> GetLines(DataRow customerInfoRow, int roleId)
        {
            var downLines = new List<ObjectInfo>
            {
                new ObjectInfo(
                    customerInfoRow.CustomerId(Role.Super).ToString(),
                    customerInfoRow.CustomerName(Role.Super),
                    Role.Super.DisplayRoleName())
            };

            if (roleId.IsSuper())
            {
                return downLines;
            }

            downLines.Add(new ObjectInfo(
                customerInfoRow.CustomerId(Role.Master).ToString(),
                customerInfoRow.CustomerName(Role.Master),
                Role.Master.DisplayRoleName()));

            if (roleId.IsNotMaster())
            {
                downLines.Add(new ObjectInfo(
                    customerInfoRow.CustomerId(Role.Agent).ToString(),
                    customerInfoRow.CustomerName(Role.Agent),
                    Role.Agent.DisplayRoleName()));
            }

            return downLines;
        }

        public ObjectInfo CurrentLevel { get; private set; }

        public ObjectInfo Recommend { get; }

        public ObjectInfo Name { get; set; }

        public ObjectInfo Nickname { get; set; }

        public ObjectInfo Phone { get; set; }

        public ObjectInfo Mobile { get; set; }

        public ObjectInfo Headline { get; set; }

        public ObjectInfo Fax { get; set; }

        public ObjectInfo CustList { get; }

        public ObjectInfo Status { get; }

        public ObjectInfo BackgroundColor { get; }
    }
}