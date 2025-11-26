namespace Fanex.ExternalAccount.Statement.Models.StatementReport
{
    using System;
    using System.Collections.Generic;

    //[Serializable]
    public class StatementReportModel
    {
        public List<StatementModel> StatementList { get; set; }

        public StatementModel Total { get; set; }

        public int TotalItems { get; set; }

        public int PageIndex { get; set; }
    }
}
