var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
function roundMoney(cell, formatNumber) {
    if (formatNumber === void 0) { formatNumber = ""; }
    if (cell.Value == "empty")
        return "";
    if (isNaN(cell.Value) || !formatNumber)
        return cell.Value;
    var absNum = Math.abs(cell.Value);
    var color = cell.Value < 0 ? 'red' : '';
    var fullNumber = numeral(cell.Value).format(formatNumber);
    if (absNum >= 1000000000) {
        return "<span style=\"color:" + color + "\" title=\"" + fullNumber + "\" >" + numeral(cell.Value / 1000000000).format(formatNumber) + "B</span>";
    }
    else if (absNum >= 1000000) {
        return "<span style=\"color:" + color + "\" title=\"" + fullNumber + "\">" + numeral(cell.Value / 1000000).format(formatNumber) + "M</span>";
    }
    return "<span style=\"color:" + color + "\">" + numeral(cell.Value).format(formatNumber) + "</span>";
}
var MatchWinLossGrid = /** @class */ (function (_super) {
    __extends(MatchWinLossGrid, _super);
    function MatchWinLossGrid(options) {
        return _super.call(this, options) || this;
    }
    MatchWinLossGrid.prototype.buildEventName = function (homeTeam, awayTeam, isNeutral) {
        var neutralSuffix = isNeutral ? '(N)' : '';
        if (!homeTeam && !awayTeam)
            return '';
        if (homeTeam && awayTeam)
            return "" + homeTeam + neutralSuffix + "\n" + awayTeam + neutralSuffix;
        if (homeTeam)
            return "" + homeTeam + neutralSuffix + "\n__";
        return "__\n" + awayTeam + neutralSuffix;
    };
    MatchWinLossGrid.prototype.buildCommonColumns = function (element) {
        if (!element)
            return [];
        return [
            { Name: "StakeAvg", Value: element["StakeAvg"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "BetCount", Value: element["BetCount"], CustomCellRender: function (cell) { return roundMoney(cell); } },
            { Name: "Turnover", Value: element["Turnover"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "NetTurnover", Value: element["NetTurnover"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "GrossComm", Value: element["GrossComm"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "MemberWinLoss", Value: element["MemberWinLoss"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "MemberComm", Value: element["MemberComm"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "MemberTotal", Value: element["MemberTotal"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "MemberMargin", Value: element["MemberMargin"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "AgentWinLoss", Value: element["AgentWinLoss"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "AgentComm", Value: element["AgentComm"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "AgentTotal", Value: element["AgentTotal"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "MasterWinLoss", Value: element["MasterWinLoss"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "MasterComm", Value: element["MasterComm"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "MasterTotal", Value: element["MasterTotal"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "SuperWinLoss", Value: element["SuperWinLoss"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "SuperComm", Value: element["SuperComm"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "SuperTotal", Value: element["SuperTotal"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "CompanyTotal", Value: element["CompanyTotal"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } }
        ];
    };
    return MatchWinLossGrid;
}(BaseReportGrid));
var SummaryGrid = /** @class */ (function (_super) {
    __extends(SummaryGrid, _super);
    function SummaryGrid(options) {
        var _this = _super.call(this, options) || this;
        _this.gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [{ Columns: SummaryGrid.GroupHeaderColumns }],
            columns: SummaryGrid.DefaultColumns,
            numberOfFrozenColumn: 2,
            autoCalculateGridExpandHeight: true,
            fullHeight: !options.isMobile.phone,
            gridHeight: options.isMobile.phone ? $(window).height() - 70 : 0,
            showTitle: false,
            // titleInfo: {
            //     text: "",
            //     customButtons: `<span>Columns<div id=${options.selectColumnsId}></div> </span><span id=${options.excelBtnId} title="Export Excel" class="icon-excel"></span>`
            // },
            gridExpandHeight: 400,
            numOfPaddingForGridExpandHeight: 40,
            headerColumnHeight: 30,
            bodyRowHeight: 26,
            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPageArray: options.pageSizeOptions,
                currentPage: 1,
                totalItem: 2000,
                itemsPerPage: options.pageSizeOptions[0],
                pagerSelectCallBack: function (pageIndex, pageSize) {
                }
            },
            marginRight: 0,
            // padding: 10,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
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
    SummaryGrid.prototype.buildBodyRows = function (reportData) {
        var bodyData = [];
        var Events = reportData.Events, GroupLeagues = reportData.GroupLeagues;
        for (var _i = 0, GroupLeagues_1 = GroupLeagues; _i < GroupLeagues_1.length; _i++) {
            var leagueGroup = GroupLeagues_1[_i];
            bodyData.push({
                Columns: [
                    { Name: "KickOffTime", Value: leagueGroup.LeagueName.trim(), CssClass: 'break-space', MergeColNumber: 2, CellAlign: "left" },
                    { Name: "StakeAvg", Value: "empty" },
                    { Name: "BetCount", Value: "empty" },
                    { Name: "Turnover", Value: "empty" },
                    { Name: "NetTurnover", Value: "empty" },
                    { Name: "GrossComm", Value: "empty" },
                    { Name: "MemberWinLoss", Value: "empty" },
                    { Name: "MemberComm", Value: "empty" },
                    { Name: "MemberTotal", Value: "empty" },
                    { Name: "MemberMargin", Value: "empty" },
                    { Name: "AgentWinLoss", Value: "empty" },
                    { Name: "AgentComm", Value: "empty" },
                    { Name: "AgentTotal", Value: "empty" },
                    { Name: "MasterWinLoss", Value: "empty" },
                    { Name: "MasterComm", Value: "empty" },
                    { Name: "MasterTotal", Value: "empty" },
                    { Name: "SuperWinLoss", Value: "empty" },
                    { Name: "SuperComm", Value: "empty" },
                    { Name: "SuperTotal", Value: "empty" },
                    { Name: "CompanyTotal", Value: "empty" }
                ],
                CssClass: "row-league-group"
            });
            for (var _a = 0, Events_1 = Events; _a < Events_1.length; _a++) {
                var element = Events_1[_a];
                if (element.LeagueId != leagueGroup.LeagueId)
                    continue;
                var HomeName = element.HomeName, AwayName = element.AwayName, IsNeutral = element.IsNeutral, KickOffTimeStr = element.KickOffTimeStr;
                bodyData.push({
                    Columns: [
                        {
                            Name: "KickOffTime",
                            Value: moment(KickOffTimeStr).format('MM/DD/YYYY [\n] hh:mm A'),
                            CssClass: 'break-space'
                        },
                        {
                            Name: "Event",
                            Value: this.buildEventName(HomeName.trim(), AwayName.trim(), IsNeutral),
                            CssClass: 'break-space'
                        }
                    ].concat(this.buildCommonColumns(element))
                });
            }
        }
        return bodyData;
    };
    SummaryGrid.prototype.buildFooterRows = function (reportData) {
        var _this = this;
        var Total = reportData.Total, SystemParlayOutright = reportData.SystemParlayOutright, GrandTotal = reportData.GrandTotal, Events = reportData.Events;
        var totalText = reportData.currency ? "Total in " + reportData.currency : "Total";
        var grandTotalText = reportData.currency ? "Grand Total in " + reportData.currency : "Grand Total";
        var footerColumns = [];
        var mergeColNumber = 2;
        if (Events) {
            if (Events.length > 0 && JSON.stringify(Total) !== JSON.stringify(GrandTotal)) {
                footerColumns.push({
                    Columns: [
                        { Name: "KickOffTime", Value: totalText, MergeColNumber: mergeColNumber, CellAlign: "right" }
                    ].concat(this.buildCommonColumns(Total)),
                    RowHeight: 21,
                    CssClass: "row-total"
                });
            }
        }
        if (SystemParlayOutright.length > 0) {
            SystemParlayOutright.forEach(function (e) {
                footerColumns.push({
                    Columns: [
                        { Name: "KickOffTime", Value: e["BetTypeName"], MergeColNumber: mergeColNumber, CellAlign: "right" }
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
                CellAlign: "right",
                CssClass: 'row-grand-total'
            });
        }
        return footerColumns;
    };
    SummaryGrid.prototype.getDefaultColumns = function () {
        return SummaryGrid.DefaultColumns.filter(function (e) { return e.Name != "BetType"; });
    };
    ;
    SummaryGrid.DefaultColumns = [
        { HeaderText: "KO Time \n (GMT+8)", Name: "KickOffTime", Width: 80, Sort: false, HeaderAlign: "center", ColumnName2: "KO Time (GMT+8)", CellAlign: "center" },
        { HeaderText: "Event", Name: "Event", Width: 380, Sort: false, HeaderAlign: "center", ColumnName2: "Event", },
        { HeaderText: "Avg.Stake", Name: "StakeAvg", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", ColumnName2: "Avg.Stake", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Bet Count", Name: "BetCount", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", ColumnName2: "Bet Count", CustomCellRender: function (cell) { return roundMoney(cell); } },
        { HeaderText: "Turnover", Name: "Turnover", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Turnover", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Net Turnover", Name: "NetTurnover", Width: 120, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Net Turnover", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "GrossComm", Name: "GrossComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "GrossComm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Win/Loss", Name: "MemberWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Win Loss", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Comm", Name: "MemberComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Comm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Total", Name: "MemberTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Total", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        {
            HeaderText: "Margin(%)&nbsp;<span title='Member Margin is calculated by dividing the sum of Member Win/Loss and Commission by the Net Turnover.' class=\"bet-type-detail-icon\"></span>",
            Name: "MemberMargin", Width: 120, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Margin", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); }
        },
        { HeaderText: "Win/Loss", Name: "AgentWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Agent-Win Loss", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Comm", Name: "AgentComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Agent-Comm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Total", Name: "AgentTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Agent-Total", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Win/Loss", Name: "MasterWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master-Win Loss", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Comm", Name: "MasterComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master-Comm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Total", Name: "MasterTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master-Total", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Win/Loss", Name: "SuperWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super-Win Loss", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Comm", Name: "SuperComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super-Comm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Total", Name: "SuperTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super-Total", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Company", Name: "CompanyTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Company", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } }
    ];
    SummaryGrid.GroupHeaderColumns = [
        { HeaderText: "KO Time \n (GMT+8)", Name: "KickOffTime", MergeRowNumber: 2 },
        { HeaderText: "Event", Name: "Event", MergeRowNumber: 2 },
        { HeaderText: "AvgStake", Name: "StakeAvg", MergeRowNumber: 2 },
        { HeaderText: "Bet Count", Name: "BetCount", MergeRowNumber: 2 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
        {
            HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" title='Net Turnover is Turnover which excludes the draw tickets and halves of the turnover of the half won, half lost tickets.' class=\"bet-type-detail-icon\"></span>",
            Name: "NetTurnover",
            MergeRowNumber: 2
        },
        { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberWinLoss", MergeColNumber: 4 },
        { HeaderText: "Agent", Name: "AgentWinLoss", MergeColNumber: 3 },
        { HeaderText: "Master", Name: "MasterWinLoss", MergeColNumber: 3 },
        { HeaderText: "Super", Name: "SuperWinLoss", MergeColNumber: 3 },
        { HeaderText: "Company", Name: "CompanyTotal", MergeRowNumber: 2 }
    ];
    return SummaryGrid;
}(MatchWinLossGrid));
var DetailGrid = /** @class */ (function (_super) {
    __extends(DetailGrid, _super);
    function DetailGrid(options) {
        var _this = _super.call(this, options) || this;
        _this.gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [{ Columns: DetailGrid.GroupHeaderColumns }],
            columns: DetailGrid.DefaultColumns,
            numberOfFrozenColumn: 3,
            autoCalculateGridExpandHeight: true,
            fullHeight: !options.isMobile.phone,
            gridHeight: options.isMobile.phone ? $(window).height() - 70 : 0,
            showTitle: false,
            // titleInfo: {
            //     text: "",
            //     customButtons: `<span id=${options.excelBtnId} title="Export Excel" class="icon-excel"></span>`
            // },
            gridExpandHeight: 400,
            numOfPaddingForGridExpandHeight: 40,
            headerColumnHeight: 30,
            bodyRowHeight: 26,
            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPageArray: options.pageSizeOptions,
                currentPage: 1,
                totalItem: 2000,
                itemsPerPage: options.pageSizeOptions[0],
                pagerSelectCallBack: function (pageIndex, pageSize) {
                }
            },
            marginRight: 0,
            // padding: 10,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            bodyRows: [],
            showFooter: true,
            footerRows: [],
            sortOption: {
                serverSortFunction: function (colname) {
                }
            },
            showHighlightRow: false,
            numberOfColumnHaveMergeRow: 2,
            enableHiddenColumns: true
        };
        return _this;
    }
    DetailGrid.prototype.buildBodyRows = function (reportData) {
        var rows = [];
        var currentMergeRowNumber = 0;
        var Events = reportData.Events, GroupLeagues = reportData.GroupLeagues;
        var subTotalText = reportData.currency ? "Sub Total in " + reportData.currency : "Sub Total";
        for (var _i = 0, GroupLeagues_2 = GroupLeagues; _i < GroupLeagues_2.length; _i++) {
            var leagueGroup = GroupLeagues_2[_i];
            rows.push({
                Columns: [
                    { Name: "KickOffTime", Value: leagueGroup.LeagueName.trim(), CssClass: 'break-space', MergeColNumber: 3, CellAlign: "left" },
                    { Name: "StakeAvg", Value: "empty" },
                    { Name: "BetCount", Value: "empty" },
                    { Name: "Turnover", Value: "empty" },
                    { Name: "NetTurnover", Value: "empty" },
                    { Name: "GrossComm", Value: "empty" },
                    { Name: "MemberWinLoss", Value: "empty" },
                    { Name: "MemberComm", Value: "empty" },
                    { Name: "MemberTotal", Value: "empty" },
                    { Name: "MemberMargin", Value: "empty" },
                    { Name: "AgentWinLoss", Value: "empty" },
                    { Name: "AgentComm", Value: "empty" },
                    { Name: "AgentTotal", Value: "empty" },
                    { Name: "MasterWinLoss", Value: "empty" },
                    { Name: "MasterComm", Value: "empty" },
                    { Name: "MasterTotal", Value: "empty" },
                    { Name: "SuperWinLoss", Value: "empty" },
                    { Name: "SuperComm", Value: "empty" },
                    { Name: "SuperTotal", Value: "empty" },
                    { Name: "CompanyTotal", Value: "empty" }
                ],
                CssClass: "row-league-group"
            });
            var _loop_1 = function (element) {
                if (element.LeagueId != leagueGroup.LeagueId)
                    return "continue";
                var currentColumns = [];
                var MatchId = element.MatchId, HomeName = element.HomeName, AwayName = element.AwayName, IsNeutral = element.IsNeutral, KickOffTimeStr = element.KickOffTimeStr, BetTypeName = element.BetTypeName, BetTypeId = element.BetTypeId;
                if (currentMergeRowNumber == 0) {
                    currentMergeRowNumber = Events.filter(function (e) { return e.MatchId == MatchId; }).length;
                    currentColumns.push({
                        Name: "KickOffTime",
                        Value: moment(KickOffTimeStr).format('MM/DD/YYYY hh:mm A'),
                        MergeRowNumber: currentMergeRowNumber,
                        CssClass: 'break-space'
                    }, {
                        Name: "Event",
                        Value: this_1.buildEventName(HomeName.trim(), AwayName.trim(), IsNeutral),
                        MergeRowNumber: currentMergeRowNumber,
                        CssClass: 'break-space'
                    });
                }
                var isBetType0 = BetTypeId == 0;
                rows.push({
                    Columns: currentColumns.concat([
                        {
                            Name: "BetType",
                            Value: isBetType0 ? subTotalText : BetTypeName
                        }
                    ].concat(this_1.buildCommonColumns(element))),
                    CssClass: isBetType0 ? 'sub-total' : ''
                });
                currentMergeRowNumber--;
            };
            var this_1 = this;
            for (var _a = 0, Events_2 = Events; _a < Events_2.length; _a++) {
                var element = Events_2[_a];
                _loop_1(element);
            }
        }
        return rows;
    };
    DetailGrid.prototype.getDefaultColumns = function () {
        return DetailGrid.DefaultColumns;
    };
    DetailGrid.prototype.buildFooterRows = function (reportData) {
        var _this = this;
        var Total = reportData.Total, SystemParlayOutright = reportData.SystemParlayOutright, GrandTotal = reportData.GrandTotal, Events = reportData.Events;
        var totalText = reportData.currency ? "Total in " + reportData.currency : "Total";
        var grandTotalText = reportData.currency ? "Grand Total in " + reportData.currency : "Grand Total";
        var footerColumns = [];
        var mergeColNumber = 3;
        if (Events) {
            if (Events.length > 0 && JSON.stringify(Total) !== JSON.stringify(GrandTotal)) {
                footerColumns.push({
                    Columns: [
                        { Name: "KickOffTime", Value: totalText, MergeColNumber: mergeColNumber, CellAlign: "right" }
                    ].concat(this.buildCommonColumns(Total)),
                    RowHeight: 21,
                    CssClass: "row-total"
                });
            }
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
                CellAlign: "right",
                CssClass: "row-grand-total"
            });
        }
        return footerColumns;
    };
    DetailGrid.DefaultColumns = [
        { HeaderText: "KO Time\n(GMT+8)", Name: "KickOffTime", Width: 80, Sort: false, HeaderAlign: "center", ColumnName2: "KO Time (GMT+8)", CellAlign: "center", },
        { HeaderText: "Event", Name: "Event", Width: 380, Sort: false, HeaderAlign: "center", ColumnName2: "Event", },
        { HeaderText: "BetType", Name: "BetType", Width: 200, Sort: false, HeaderAlign: "center", ColumnName2: "BetType", },
        { HeaderText: "Avg.Stake", Name: "StakeAvg", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", ColumnName2: "Avg.Stake", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Bet Count", Name: "BetCount", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", ColumnName2: "Bet Count", CustomCellRender: function (cell) { return roundMoney(cell); } },
        { HeaderText: "Turnover", Name: "Turnover", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Turnover", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Net Turnover", Name: "NetTurnover", Width: 120, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Net Turnover", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "GrossComm", Name: "GrossComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "GrossComm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Win/Loss", Name: "MemberWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Win Loss", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Comm", Name: "MemberComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Comm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Total", Name: "MemberTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Total", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        {
            HeaderText: "Margin(%)&nbsp;<span title='Member Margin is calculated by dividing the sum of Member Win/Loss and Commission by the Net Turnover.' class=\"bet-type-detail-icon\"></span>",
            Name: "MemberMargin", Width: 120, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Margin", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); }
        },
        { HeaderText: "Win/Loss", Name: "AgentWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Agent-Win Loss", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Comm", Name: "AgentComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Agent-Comm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Total", Name: "AgentTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Agent-Total", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Win/Loss", Name: "MasterWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master-Win Loss", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Comm", Name: "MasterComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master-Comm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Total", Name: "MasterTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master-Total", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Win/Loss", Name: "SuperWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super-Win Loss", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Comm", Name: "SuperComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super-Comm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Total", Name: "SuperTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super-Total", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Company", Name: "CompanyTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Company", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } }
    ];
    DetailGrid.GroupHeaderColumns = [
        { HeaderText: "KO Time\n(GMT+8)", Name: "KickOffTime", MergeRowNumber: 2 },
        { HeaderText: "Event", Name: "Event", MergeRowNumber: 2 },
        { HeaderText: "Bet Type", Name: "BetType", MergeRowNumber: 2 },
        { HeaderText: "AvgStake", Name: "StakeAvg", MergeRowNumber: 2 },
        { HeaderText: "Bet Count", Name: "BetCount", MergeRowNumber: 2 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
        {
            HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" title='Net Turnover is Turnover which excludes the draw tickets and halves of the turnover of the half won, half lost tickets.' class=\"bet-type-detail-icon\"></span>",
            Name: "NetTurnover",
            MergeRowNumber: 2
        },
        { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberWinLoss", MergeColNumber: 4 },
        { HeaderText: "Agent", Name: "AgentWinLoss", MergeColNumber: 3 },
        { HeaderText: "Master", Name: "MasterWinLoss", MergeColNumber: 3 },
        { HeaderText: "Super", Name: "SuperWinLoss", MergeColNumber: 3 },
        { HeaderText: "Company", Name: "CompanyTotal", MergeRowNumber: 2 }
    ];
    return DetailGrid;
}(MatchWinLossGrid));
//# sourceMappingURL=MatchWinLossGrid.js.map