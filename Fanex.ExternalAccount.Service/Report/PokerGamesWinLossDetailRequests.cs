namespace Fanex.ExternalAccount.Service.Interface.Report
{
    using System;

    public interface IDrillDownPokerGamesWinLossDetailRequest
    {
        string GroupName { get; }
        int SuperId { get; }
        int MasterId { get; }
        int AgentId { get; }
        int CustId { get; }
        int ViewLevel { get; }
        DateTime FromDate { get; }
        DateTime ToDate { get; }
        bool IsValid { get; }
        bool IsNotValid { get; }
    }

    public interface ISearchPokerGamesWinLossDetailRequest
    {
        string GroupName { get; }
        string Username { get; }
        DateTime FromDate { get; }
        DateTime ToDate { get; }
    }
}