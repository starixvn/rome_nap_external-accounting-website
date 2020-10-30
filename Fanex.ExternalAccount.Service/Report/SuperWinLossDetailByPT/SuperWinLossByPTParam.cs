namespace Fanex.ExternalAccount.Service.Report.SuperWinLossDetailByPT
{
    using Fanex.ExternalAccount.DTO;
    using System;
    using System.Collections.Generic;

    public class SuperWinLossDetailByPTParam
    {
        public SuperWinLossDetailByPTParam(
            int custId,
            DateTime fromDate,
            DateTime toDate,
            string userSelectedProductIds,
            //IEnumerable<int> userSelectedProductIds,
            //IEnumerable<int> userSelectedResourceIds,
            //UserLevel custLevel,
            string productDetailsXML,
            int pageSize,
            int pageIndex)
        {
            CustId = custId;
            //CustLevel = custLevel;
            UserSelectedProductIds = userSelectedProductIds;
            //UserSelectedProductIds = userSelectedProductIds;
            //UserSelectedResourceIds = userSelectedResourceIds;

            FromDate = fromDate == DateTime.MinValue ? DateTime.Now : fromDate;
            ToDate = toDate == DateTime.MinValue ? DateTime.Now : toDate;

            ProductDetailsXML = productDetailsXML;
            PageSize = pageSize;
            PageIndex = pageIndex;
        }

        public int CustId { get; }

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }

        public string UserSelectedProductIds { get; }

        //public IEnumerable<int> UserSelectedProductIds { get; }

        //public IEnumerable<int> UserSelectedResourceIds { get; }

        //public UserLevel CustLevel { get; }

        public string ProductDetailsXML { get; }

        public int PageSize { get; }

        public int PageIndex { get; }
    }
}