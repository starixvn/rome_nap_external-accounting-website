using System;
using System.Collections;

namespace Fanex.ExternalAccount.Service.Interface.Report
{
    public interface IDrillDownRequest
    {
        int CustId { get; }

        int SuperId { get; }

        int MasterId { get; }

        int AgentId { get; }

        int ViewLevel { get; }
        DateTime FromDate { get; }
        DateTime ToDate { get; }
        string ProductIds { get; }
        ArrayList SortingOrderList { get; }
        string GroupName { get; }
        bool IsValid { get; }

        bool IsNotValid { get; }
    }
}