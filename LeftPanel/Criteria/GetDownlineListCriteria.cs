namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;

    public class GetDownlineListCriteria : CriteriaBase
    {
        public int CustId { get; }
        public int PageSize { get; }
        public int PageIndex { get; }
        public int TotalPages { get; }

        public GetDownlineListCriteria(int custId, int pageSize, int pageIndex, int totalPages)
        {
            CustId = custId;
            PageSize = pageSize;
            PageIndex = pageIndex;
            TotalPages = totalPages;
        }

        public override string GetSettingKey()
        {
            return "Acc_CustomerSelList";
        }

        public override bool IsValid()
        {
            return CustId > 0 && PageSize > 0 && PageIndex >= 0;
        }
    }
}







