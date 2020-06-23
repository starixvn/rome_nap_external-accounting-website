namespace Fanex.ExternalAccount.Service.Interface.Report
{
    using System;

    public interface ISearchWinLossDetailIBCBetLiveCasinoRequest
    {
        string GroupName { get; }
        DateTime FromDate { get; }
        DateTime ToDate { get; }
        dynamic ToolTipsProducts { get; }
    }
}