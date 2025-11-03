namespace Fanex.ExternalAccount.HeadlineBonus.Criteria
{
    using System;
    using Fanex.Data.Repository;
    internal class GetMemberBonusCriteria : CriteriaBase
    {
        public GetMemberBonusCriteria(
            string groupName,
            string username,
            DateTime fromDate,
            DateTime toDate,
            byte? statusId,
            bool? isComplained,
            int pageIndex,
            int pageSize)
        {
            GroupName = groupName;
            Username = username;
            FromDate = fromDate;
            ToDate = toDate;
            StatusId = statusId;
            IsComplained = isComplained;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public string GroupName { get; }

        public string Username { get; }

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }

        public byte? StatusId { get; }

        public bool? IsComplained { get; }

        public int PageIndex { get; }

        public int PageSize { get; }

        public int TotalRows { get; set; }

        public override string GetSettingKey()
            => "EAS_Bonus_Sel";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(GroupName) && FromDate <= ToDate;
    }
}
