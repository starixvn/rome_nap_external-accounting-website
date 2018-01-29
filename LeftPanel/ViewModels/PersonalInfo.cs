namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Collections.Generic;
    using System.Data;
    using Core.Shared;
    using Extensions;

    public class PersonalInfo : DownlineList
    {
        public PersonalInfo(DataRow customerInfoRow, DataRow lineRow)
        {
            var role = customerInfoRow.RoleId();
            var enabled = customerInfoRow.Enabled();
            var closed = customerInfoRow.Closed();
            var suspended = customerInfoRow.Suspended();

            CustId = ObjectInfo.CreateInstance("Cust Id", customerInfoRow.CustId());
            Fax = ObjectInfo.CreateInstance("Fax", customerInfoRow.Fax());
            Mobile = ObjectInfo.CreateInstance("Mobile", customerInfoRow.MobilePhone());
            Name = ObjectInfo.CreateInstance("Name", customerInfoRow.Name());
            Nickname = ObjectInfo.CreateInstance("Nickname", customerInfoRow.Nickname());
            Phone = ObjectInfo.CreateInstance("Phone", customerInfoRow.Phone());
            RoleId = ObjectInfo.CreateInstance("Role Id", role);
            UplineRoleId = ObjectInfo.CreateInstance("Upline Role Id", role.DirectUplineLevel());
            CustList = new ObjectInfo
            {
                Key = "CustList",
                Value = $"{(role.IsNotMember() ? role.DirectDownlineLevel().DisplayRoleName() : string.Empty)} List"
            };
            Status = new ObjectInfo { Tooltip = customerInfoRow.StatusTooltip(enabled, closed, suspended) };
            BackgroundColor = new ObjectInfo { Value = customerInfoRow.StatusBackground(enabled, closed, suspended) };
            IsInternal = new ObjectInfo { Value = customerInfoRow.IsInternal() };

            SetLineInfo(role, lineRow);
        }

        internal void SetLineInfo(int role, DataRow lineRow)
        {
            CurrentLevel = ObjectInfo.CreateInstance(text: role.DisplayRoleName(), value: lineRow.CustomerName(role));

            Downlines = GetLines(lineRow, role);

            if (role.IsNotSuper())
            {
                UplineId = ObjectInfo.CreateInstance(
                    text: $"{role.DirectUplineLevel().DisplayRoleName()} ID",
                    value: lineRow.CustomerId(role.DirectUplineLevel()));
            }
        }

        internal static List<ObjectInfo> GetLines(DataRow lineRow, int roleId)
        {
            var downLines = new List<ObjectInfo>
            {
                new ObjectInfo(
                    lineRow.CustomerId(Role.Super).ToString(),
                    lineRow.CustomerName(Role.Super),
                    Role.Super.DisplayRoleName())
            };

            if (roleId.IsSuper())
            {
                return downLines;
            }

            downLines.Add(new ObjectInfo(
                lineRow.CustomerId(Role.Master).ToString(),
                lineRow.CustomerName(Role.Master),
                Role.Master.DisplayRoleName()));

            if (roleId.IsNotMaster())
            {
                downLines.Add(new ObjectInfo(
                    lineRow.CustomerId(Role.Agent).ToString(),
                    lineRow.CustomerName(Role.Agent),
                    Role.Agent.DisplayRoleName()));
            }

            return downLines;
        }

        public ObjectInfo CurrentLevel { get; private set; }

        public ObjectInfo Name { get; }

        public ObjectInfo Nickname { get; }

        public ObjectInfo Phone { get; }

        public ObjectInfo Mobile { get; }

        public ObjectInfo Fax { get; }

        public ObjectInfo CustList { get; }

        public ObjectInfo Status { get; }

        public ObjectInfo BackgroundColor { get; }

        public ObjectInfo IsInternal { get; }
    }
}