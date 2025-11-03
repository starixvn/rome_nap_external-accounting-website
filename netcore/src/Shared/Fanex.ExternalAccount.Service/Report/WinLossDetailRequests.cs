namespace Fanex.ExternalAccount.Service.Interface.Report
{
    using System;

    public interface IDrillDownWinLossDetailRequest
    {
        string MainGroupName { get; }
        int GroupId { get; }
        string GroupName { get; }
        int SuperId { get; }
        int MasterId { get; }
        int AgentId { get; }
        int CustId { get; }
        int ViewLevel { get; }
        DateTime FromDate { get; }
        DateTime ToDate { get; }
        string ProductIds { get; }
        dynamic ToolTipsProducts { get; }
        string ResourceIds { get; }
        int UserId { get; }
        bool IsValid { get; }
        bool IsNotValid { get; }
        string Currencies { get; }
        string BaseCurrencyName { get; }
        bool IsMultipleCurrency { get; }
    }

    public interface ISearchWinLossDetailRequest
    {
        string MainGroupName { get; }
        int GroupId { get; }
        string GroupName { get; }
        string Username { get; }
        DateTime FromDate { get; }
        DateTime ToDate { get; }
        string ProductIds { get; }
        dynamic ToolTipsProducts { get; }
        string ResourceIds { get; }
        int UserId { get; }
        string Currencies { get; }
        string BaseCurrencyName { get; }
        bool IsMultipleCurrency { get; }
        bool IsValid { get; }
    }

    public interface IDrillDownCashFeeRequest
    {
        int CustId { get;  }

        string UserName { get; }

        DateTime FromDate { get; } 

        DateTime ToDate { get; } 
    }
}