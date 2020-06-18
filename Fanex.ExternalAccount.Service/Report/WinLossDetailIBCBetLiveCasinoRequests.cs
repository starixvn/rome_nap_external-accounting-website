namespace Fanex.ExternalAccount.Service.Interface.Report
{
    using System;

    public interface ISearchWinLossDetailIBCBetLiveCasinoRequest
    {
        string GroupName { get; }
        string Username { get; }
        DateTime FromDate { get; }
        DateTime ToDate { get; }
        string ProductIds { get; }
        dynamic ToolTipsProducts { get; }
    }
}