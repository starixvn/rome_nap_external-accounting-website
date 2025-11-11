var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var BaseGridBuilder = /** @class */ (function () {
    function BaseGridBuilder() {
    }
    BaseGridBuilder.prototype.ConvertDotNetJsonDateToJsDate = function (jsonDate) {
        var regex = /^\/Date\((\d+)\)\/$/;
        var match = regex.exec(jsonDate);
        var datetime = null;
        if (match !== null) {
            datetime = new Date(parseInt(match[1]));
        }
        var formattedDate = moment(datetime).format('MMM DD, YYYY');
        var formattedTime = moment(datetime).format('hh:mm A');
        return formattedDate + "\n" + formattedTime;
    };
    BaseGridBuilder.prototype.BuildEventName = function (leagueName, homeTeam, awayTeam, isNeutral) {
        return isNeutral
            ? "<b>" + leagueName + "</b>\n" + homeTeam + "(N)\n" + awayTeam + "(N)"
            : "<b>" + leagueName + "</b>\n" + homeTeam + "\n" + awayTeam;
    };
    BaseGridBuilder.DefaultColumns = [
        {
            HeaderText: "KO Time (GMT-4)",
            Name: "KickOffTime", Width: 120,
            Sort: false,
            HeaderAlign: "center",
            ColumnName2: "KO Time (GMT-4)",
        },
        {
            HeaderText: "Event",
            Name: "Event",
            Width: 400,
            Sort: false,
            HeaderAlign: "center",
            ColumnName2: "Event",
        },
        {
            HeaderText: "BetType",
            Name: "BetType",
            Width: 200,
            Sort: false,
            HeaderAlign: "center",
            ColumnName2: "BetType",
        },
        {
            HeaderText: "Avg.Stake",
            Name: "AvgStake",
            Width: 100,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            ColumnName2: "Avg.Stake",
        },
        {
            HeaderText: "Bet Count",
            Name: "BetCount",
            Width: 120,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            ColumnName2: "Bet Count",
        },
        {
            HeaderText: "Turnover",
            Name: "Turnover",
            Width: 120,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$",
            ColumnName2: "Turnover",
        },
        {
            HeaderText: "Net Turnover",
            Name: "NetTurnover",
            Width: 120,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$",
            ColumnName2: "Net Turnover",
        },
        {
            HeaderText: "GrossComm",
            Name: "GrossComm",
            Width: 120,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$",
            ColumnName2: "GrossComm",
        },
        {
            HeaderText: "Win/Loss",
            Name: "MemberWinLoss",
            Width: 100,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$",
            ColumnName2: "Member-Win Loss",
        },
        {
            HeaderText: "Comm",
            Name: "MemberComm",
            Width: 100,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$",
            ColumnName2: "Member-Comm",
        },
        {
            HeaderText: "Total",
            Name: "MemberTotal",
            Width: 100,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$",
            ColumnName2: "Member-Total",
        },
        {
            HeaderText: "Win/Loss",
            Name: "AgentWinLoss",
            Width: 100,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$",
            ColumnName2: "Agent-Win Loss",
        },
        {
            HeaderText: "Comm",
            Name: "AgentComm",
            Width: 100,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$",
            ColumnName2: "Agent-Comm",
        },
        {
            HeaderText: "Total",
            Name: "AgentTotal",
            Width: 100,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$",
            ColumnName2: "Agent-Total",
        },
        {
            HeaderText: "Win/Loss",
            Name: "MasterWinLoss",
            Width: 100,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$",
            ColumnName2: "Master-Win Loss",
        },
        {
            HeaderText: "Comm",
            Name: "MasterComm",
            Width: 100,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$",
            ColumnName2: "Master-Comm",
        },
        {
            HeaderText: "Total",
            Name: "MasterTotal",
            Width: 100,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$",
            ColumnName2: "Master-Total",
        },
        {
            HeaderText: "Win/Loss",
            Name: "SuperWinLoss",
            Width: 100,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$",
            ColumnName2: "Super-Win Loss",
        },
        {
            HeaderText: "Comm",
            Name: "SuperComm",
            Width: 100,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$",
            ColumnName2: "Super-Comm",
        },
        {
            HeaderText: "Total",
            Name: "SuperTotal",
            Width: 100,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$",
            ColumnName2: "Super-Total",
        },
        {
            HeaderText: "Margin(%)",
            Name: "SuperMargin",
            Width: 100,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$",
            ColumnName2: "Super-Margin",
        },
        {
            HeaderText: "Company",
            Name: "Company",
            Width: 100,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$",
            ColumnName2: "Company",
        }
    ];
    return BaseGridBuilder;
}());
var SummaryGridBuilder = /** @class */ (function (_super) {
    __extends(SummaryGridBuilder, _super);
    function SummaryGridBuilder(isMobile) {
        var _this = _super.call(this) || this;
        _this.buildCommonColumns = function (element) {
            if (!element)
                return [];
            return [
                { Name: "AvgStake", Value: element["StakeAvg"] },
                { Name: "BetCount", Value: element["BetCount"] },
                { Name: "Turnover", Value: element["Turnover"] },
                { Name: "NetTurnover", Value: element["NetTurnover"] },
                { Name: "GrossComm", Value: element["GrossComm"] },
                { Name: "MemberWinLoss", Value: element["MemberWinLoss"] },
                { Name: "MemberComm", Value: element["MemberComm"] },
                { Name: "MemberTotal", Value: element["MemberTotal"] },
                { Name: "AgentWinLoss", Value: element["AgentWinLoss"] },
                { Name: "AgentComm", Value: element["AgentComm"] },
                { Name: "AgentTotal", Value: element["AgentTotal"] },
                { Name: "MasterWinLoss", Value: element["MasterWinLoss"] },
                { Name: "MasterComm", Value: element["MasterComm"] },
                { Name: "MasterTotal", Value: element["MasterTotal"] },
                { Name: "SuperWinLoss", Value: element["SuperWinLoss"] },
                { Name: "SuperComm", Value: element["SuperComm"] },
                { Name: "SuperTotal", Value: element["SuperTotal"] },
                { Name: "SuperMargin", Value: element["SuperMargin"] },
                { Name: "Company", Value: element["CompanyComm"] }
            ];
        };
        _this.gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [{ Columns: SummaryGridBuilder.GroupHeaderColumns }],
            columns: SummaryGridBuilder.DefaultColumns.filter(function (e) { return e.Name != "BetType"; }),
            numberOfFrozenColumn: 2,
            autoCalculateGridExpandHeight: true,
            fullHeight: !isMobile.phone,
            gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
            showTitle: true,
            titleInfo: {
                text: "",
                customButtons: "<span id=" + SummaryGridBuilder.ExcelBtnId + " title=\"Export Excel\" class=\"icon-excel\"></span>"
            },
            gridExpandHeight: 400,
            numOfPaddingForGridExpandHeight: 40,
            headerColumnHeight: 30,
            bodyRowHeight: 58,
            highlightFrozenWidth: 10,
            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPageArray: SummaryGridBuilder.PageSizeOptions,
                currentPage: 1,
                totalItem: 2000,
                itemsPerPage: SummaryGridBuilder.PageSizeOptions[0],
                pagerSelectCallBack: function (pageIndex, pageSize) {
                }
            },
            // marginRight: 10,
            // padding: 10,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: false,
            bodyRows: [],
            showFooter: true,
            footerRows: [],
            sortOption: {
                serverSortFunction: function (colname) {
                }
            },
            showHighlightRow: false,
        };
        return _this;
    }
    SummaryGridBuilder.prototype.getGridOptions = function () {
        return this.gridOptions;
    };
    SummaryGridBuilder.prototype.buildBodyRows = function (reportData) {
        var bodyData = [];
        for (var _i = 0, reportData_1 = reportData; _i < reportData_1.length; _i++) {
            var element = reportData_1[_i];
            var HomeName = element.HomeName, AwayName = element.AwayName, LeagueName = element.LeagueName, IsNeutral = element.IsNeutral;
            bodyData.push({
                Columns: [
                    {
                        Name: "KickOffTime",
                        Value: this.ConvertDotNetJsonDateToJsDate(element["KickOffTime"]),
                        CssClass: 'break-space'
                    },
                    {
                        Name: "Event",
                        Value: this.BuildEventName(LeagueName.trim(), HomeName.trim(), AwayName.trim(), IsNeutral),
                        CssClass: 'break-space'
                    }
                ].concat(this.buildCommonColumns(element))
            });
        }
        return bodyData;
    };
    SummaryGridBuilder.prototype.buildFooterRows = function (reportData, baseCurrency, isMYHeadline) {
        var _this = this;
        var Total = reportData.Total, SystemParlayOutright = reportData.SystemParlayOutright, GrandTotal = reportData.GrandTotal;
        var totalText = isMYHeadline ? "Total in " + baseCurrency : "Total";
        var grandTotalText = isMYHeadline ? "Grand Total in " + baseCurrency : "Grand Total";
        var footerColumns = [];
        var mergeColNumber = 2;
        if (Total) {
            footerColumns.push({
                Columns: [
                    { Name: "KickOffTime", Value: totalText, MergeColNumber: mergeColNumber, CellAlign: "right" }
                ].concat(this.buildCommonColumns(Total)),
                RowHeight: 21,
            });
        }
        if (SystemParlayOutright.length > 0) {
            SystemParlayOutright.forEach(function (e) {
                footerColumns.push({
                    Columns: [
                        {
                            Name: "KickOffTime",
                            Value: e["BetTypeName"],
                            MergeColNumber: mergeColNumber,
                            CellAlign: "right"
                        }
                    ].concat(_this.buildCommonColumns(e)),
                    RowHeight: 21,
                    CssClass: 'system-parlay'
                });
            });
        }
        if (GrandTotal) {
            footerColumns.push({
                Columns: [
                    { Name: "KickOffTime", Value: grandTotalText, MergeColNumber: mergeColNumber, CellAlign: "right" }
                ].concat(this.buildCommonColumns(GrandTotal)),
                RowHeight: 21,
                CellAlign: "right"
            });
        }
        return footerColumns;
    };
    SummaryGridBuilder.prototype.getDefaultColumns = function () {
        return SummaryGridBuilder.DefaultColumns.filter(function (e) { return e.Name != "BetType"; });
    };
    ;
    SummaryGridBuilder.prototype.initGrid = function (gridOptions, gridContainer) {
        this.gridOptions = gridOptions;
        this.gridContainer = gridContainer;
    };
    SummaryGridBuilder.GroupHeaderColumns = [
        { HeaderText: "KO Time (GMT-4)", Name: "KickOffTime", MergeRowNumber: 2 },
        { HeaderText: "Event", Name: "Event", MergeRowNumber: 2 },
        { HeaderText: "AvgStake", Name: "AvgStake", MergeRowNumber: 2 },
        { HeaderText: "Bet Count", Name: "BetCount", MergeRowNumber: 2 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
        {
            HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" title='Net Turnover is Turnover which excludes the draw tickets and halves of the turnover of the half won, half lost tickets.' class=\"bet-type-detail-icon\"></span>",
            Name: "NetTurnover",
            MergeRowNumber: 2
        },
        { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberWinLoss", MergeColNumber: 3 },
        { HeaderText: "Agent", Name: "AgentWinLoss", MergeColNumber: 3 },
        { HeaderText: "Master", Name: "MasterWinLoss", MergeColNumber: 3 },
        { HeaderText: "Super", Name: "SuperWinLoss", MergeColNumber: 4 },
        { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 }
    ];
    SummaryGridBuilder.PageSizeOptions = [500, 1000, 1500];
    SummaryGridBuilder.ExcelBtnId = 'exportSummaryMatchWinLoss';
    return SummaryGridBuilder;
}(BaseGridBuilder));
var DetailGridBuilder = /** @class */ (function (_super) {
    __extends(DetailGridBuilder, _super);
    function DetailGridBuilder(isMobile) {
        var _this = _super.call(this) || this;
        _this.buildCommonColumns = function (element) {
            if (!element)
                return [];
            return [
                { Name: "AvgStake", Value: element["StakeAvg"] },
                { Name: "BetCount", Value: element["BetCount"] },
                { Name: "Turnover", Value: element["Turnover"] },
                { Name: "NetTurnover", Value: element["NetTurnover"] },
                { Name: "GrossComm", Value: element["GrossComm"] },
                { Name: "MemberWinLoss", Value: element["MemberWinLoss"] },
                { Name: "MemberComm", Value: element["MemberComm"] },
                { Name: "MemberTotal", Value: element["MemberTotal"] },
                { Name: "AgentWinLoss", Value: element["AgentWinLoss"] },
                { Name: "AgentComm", Value: element["AgentComm"] },
                { Name: "AgentTotal", Value: element["AgentTotal"] },
                { Name: "MasterWinLoss", Value: element["MasterWinLoss"] },
                { Name: "MasterComm", Value: element["MasterComm"] },
                { Name: "MasterTotal", Value: element["MasterTotal"] },
                { Name: "SuperWinLoss", Value: element["SuperWinLoss"] },
                { Name: "SuperComm", Value: element["SuperComm"] },
                { Name: "SuperTotal", Value: element["SuperTotal"] },
                { Name: "SuperMargin", Value: element["SuperMargin"] },
                { Name: "Company", Value: element["CompanyComm"] }
            ];
        };
        _this.gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [{ Columns: DetailGridBuilder.GroupHeaderColumns }],
            columns: DetailGridBuilder.DefaultColumns,
            numberOfFrozenColumn: 3,
            autoCalculateGridExpandHeight: true,
            fullHeight: !isMobile.phone,
            gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
            showTitle: true,
            titleInfo: {
                text: "",
                customButtons: "<span id=" + DetailGridBuilder.ExcelBtnId + " title=\"Export Excel\" class=\"icon-excel\"></span>"
            },
            gridExpandHeight: 350,
            numOfPaddingForGridExpandHeight: 40,
            headerColumnHeight: 30,
            bodyRowHeight: 40,
            highlightFrozenWidth: 10,
            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPageArray: DetailGridBuilder.PageSizeOptions,
                currentPage: 1,
                totalItem: 2000,
                itemsPerPage: DetailGridBuilder.PageSizeOptions[0],
                pagerSelectCallBack: function (pageIndex, pageSize) { }
            },
            // marginRight: 10,
            // padding: 10,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: false,
            bodyRows: [],
            showFooter: true,
            footerRows: [],
            sortOption: {
                serverSortFunction: function (colname) { }
            },
            showHighlightRow: false,
            numberOfColumnHaveMergeRow: 2
        };
        return _this;
    }
    DetailGridBuilder.prototype.buildBodyRows = function (reportData) {
        var rows = [];
        var SubTotals = reportData.SubTotals, Mains = reportData.Mains, Events = reportData.Events;
        var currentMergeRowNumber = 0;
        var numberRows = Events.length;
        var _loop_1 = function (index) {
            currentColumns = [];
            var _a = Events[index], MatchId = _a.MatchId, LeagueName = _a.LeagueName, HomeName = _a.HomeName, AwayName = _a.AwayName, IsNeutral = _a.IsNeutral, KickOffTime = _a.KickOffTime, BetTypeName = _a.BetTypeName, BetTypeId = _a.BetTypeId;
            if (currentMergeRowNumber == 0) {
                currentMergeRowNumber = Events.filter(function (e) { return e.MatchId == MatchId; }).length;
                currentColumns.push({
                    Name: "KickOffTime",
                    Value: this_1.ConvertDotNetJsonDateToJsDate(KickOffTime),
                    MergeRowNumber: currentMergeRowNumber,
                    CssClass: 'break-space'
                }, {
                    Name: "Event",
                    Value: this_1.BuildEventName(LeagueName.trim(), HomeName.trim(), AwayName.trim(), IsNeutral),
                    MergeRowNumber: currentMergeRowNumber,
                    CssClass: 'break-space'
                });
            }
            rows.push({
                Columns: currentColumns.concat([
                    {
                        Name: "BetType",
                        Value: BetTypeId == 0 ? "Sub Total" : BetTypeName
                    }
                ].concat(this_1.buildCommonColumns(Events[index]))),
                CssClass: BetTypeId == 0 ? 'sub-total' : ''
            });
            currentMergeRowNumber--;
        };
        var this_1 = this, currentColumns;
        for (var index = 0; index < numberRows; index++) {
            _loop_1(index);
        }
        return rows;
    };
    DetailGridBuilder.prototype.getDefaultColumns = function () {
        return DetailGridBuilder.DefaultColumns;
    };
    DetailGridBuilder.prototype.buildFooterRows = function (reportData, baseCurrency, isMYHeadline) {
        var _this = this;
        var Total = reportData.Total, SystemParlayOutright = reportData.SystemParlayOutright, GrandTotal = reportData.GrandTotal;
        var totalText = isMYHeadline ? "Total in " + baseCurrency : "Total";
        var grandTotalText = isMYHeadline ? "Grand Total in " + baseCurrency : "Grand Total";
        var footerColumns = [];
        var mergeColNumber = 3;
        if (Total) {
            footerColumns.push({
                Columns: [
                    { Name: "KickOffTime", Value: totalText, MergeColNumber: mergeColNumber, CellAlign: "right" }
                ].concat(this.buildCommonColumns(Total)),
                RowHeight: 21,
            });
        }
        if (SystemParlayOutright.length > 0) {
            SystemParlayOutright.forEach(function (e) {
                footerColumns.push({
                    Columns: [
                        {
                            Name: "KickOffTime",
                            Value: e["BetTypeName"],
                            MergeColNumber: mergeColNumber,
                            CellAlign: "right"
                        }
                    ].concat(_this.buildCommonColumns(e)),
                    RowHeight: 21,
                    CssClass: 'system-parlay'
                });
            });
        }
        if (GrandTotal) {
            footerColumns.push({
                Columns: [
                    { Name: "KickOffTime", Value: grandTotalText, MergeColNumber: mergeColNumber, CellAlign: "right" }
                ].concat(this.buildCommonColumns(GrandTotal)),
                RowHeight: 21,
                CellAlign: "right"
            });
        }
        return footerColumns;
    };
    DetailGridBuilder.prototype.getGridOptions = function () {
        return this.gridOptions;
    };
    DetailGridBuilder.prototype.initGrid = function (gridOptions, gridContainer, grid, msgNoInfo) {
    };
    DetailGridBuilder.GroupHeaderColumns = [
        { HeaderText: "KO Time (GMT-4)", Name: "KickOffTime", MergeRowNumber: 2 },
        { HeaderText: "Event", Name: "Event", MergeRowNumber: 2 },
        { HeaderText: "Bet Type", Name: "BetType", MergeRowNumber: 2 },
        { HeaderText: "AvgStake", Name: "AvgStake", MergeRowNumber: 2 },
        { HeaderText: "Bet Count", Name: "BetCount", MergeRowNumber: 2 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
        {
            HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" title='Net Turnover is Turnover which excludes the draw tickets and halves of the turnover of the half won, half lost tickets.' class=\"bet-type-detail-icon\"></span>",
            Name: "NetTurnover",
            MergeRowNumber: 2
        },
        { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberWinLoss", MergeColNumber: 3 },
        { HeaderText: "Agent", Name: "AgentWinLoss", MergeColNumber: 3 },
        { HeaderText: "Master", Name: "MasterWinLoss", MergeColNumber: 3 },
        { HeaderText: "Super", Name: "SuperWinLoss", MergeColNumber: 4 },
        { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 }
    ];
    DetailGridBuilder.PageSizeOptions = [100, 150, 200];
    DetailGridBuilder.ExcelBtnId = 'exportDetailMatchWinLoss';
    return DetailGridBuilder;
}(BaseGridBuilder));
//# sourceMappingURL=GridBuilder.js.map