declare var numeral: any;

function roundMoney( cell, formatNumber = "" ) {
    if (cell.Value == "empty") return "";
    if (isNaN(cell.Value) || !formatNumber) return cell.Value;
    const absNum = Math.abs(cell.Value);
    const color = cell.Value < 0 ? 'red' : '';
    const fullNumber = numeral(cell.Value).format(formatNumber);
    if (absNum >= 1_000_000_000) {
        return `<span style="color:${color}" title="${fullNumber}" >${numeral(cell.Value / 1_000_000_000).format(formatNumber)}B</span>`;
    } else if (absNum >= 1_000_000) {
        return `<span style="color:${color}" title="${fullNumber}">${numeral(cell.Value / 1_000_000).format(formatNumber)}M</span>`;
    }

    return `<span style="color:${color}">${numeral(cell.Value).format(formatNumber)}</span>`;
}

class MatchWinLossGrid extends BaseReportGrid {
    constructor( options: IGridComponentConfig ) {
        super(options);
    }

    public buildEventName( homeTeam, awayTeam, isNeutral ) {
        const neutralSuffix = isNeutral ? '(N)' : '';

        if (!homeTeam && !awayTeam) return '';
        if (homeTeam && awayTeam) return `${homeTeam}${neutralSuffix}\n${awayTeam}${neutralSuffix}`;
        if (homeTeam) return `${homeTeam}${neutralSuffix}\n__`;
        return `__\n${awayTeam}${neutralSuffix}`;
    }


    public buildCommonColumns( element ) {
        if (!element) return [];
        return [
            { Name: "StakeAvg", Value: element["StakeAvg"], CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
            { Name: "BetCount", Value: element["BetCount"], CustomCellRender: ( cell ) => roundMoney(cell) },
            { Name: "Turnover", Value: element["Turnover"], CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
            { Name: "NetTurnover", Value: element["NetTurnover"], CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
            { Name: "GrossComm", Value: element["GrossComm"], CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
            { Name: "MemberWinLoss", Value: element["MemberWinLoss"], CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
            { Name: "MemberComm", Value: element["MemberComm"], CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
            { Name: "MemberTotal", Value: element["MemberTotal"], CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
            { Name: "MemberMargin", Value: element["MemberMargin"], CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
            { Name: "AgentWinLoss", Value: element["AgentWinLoss"], CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
            { Name: "AgentComm", Value: element["AgentComm"], CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
            { Name: "AgentTotal", Value: element["AgentTotal"], CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
            { Name: "MasterWinLoss", Value: element["MasterWinLoss"], CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
            { Name: "MasterComm", Value: element["MasterComm"], CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
            { Name: "MasterTotal", Value: element["MasterTotal"], CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
            { Name: "SuperWinLoss", Value: element["SuperWinLoss"], CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
            { Name: "SuperComm", Value: element["SuperComm"], CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
            { Name: "SuperTotal", Value: element["SuperTotal"], CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
            { Name: "CompanyTotal", Value: element["CompanyTotal"], CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") }
        ]
    }
}

class SummaryGrid extends MatchWinLossGrid {
    public static readonly DefaultColumns = [
        { HeaderText: "KO Time \n (GMT+8)", Name: "KickOffTime", Width: 80, Sort: false, HeaderAlign: "center", ColumnName2: "KO Time (GMT+8)", CellAlign: "center" },
        { HeaderText: "Event", Name: "Event", Width: 380, Sort: false, HeaderAlign: "center", ColumnName2: "Event", },
        { HeaderText: "Avg.Stake", Name: "StakeAvg", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", ColumnName2: "Avg.Stake", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Bet Count", Name: "BetCount", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", ColumnName2: "Bet Count", CustomCellRender: ( cell ) => roundMoney(cell) },
        { HeaderText: "Turnover", Name: "Turnover", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Turnover", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Net Turnover", Name: "NetTurnover", Width: 120, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Net Turnover", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "GrossComm", Name: "GrossComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "GrossComm", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Win/Loss", Name: "MemberWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Win Loss", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Comm", Name: "MemberComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Comm", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Total", Name: "MemberTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Total", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        {
            HeaderText: "Margin(%)&nbsp;<span title='Member Margin is calculated by dividing the sum of Member Win/Loss and Commission by the Net Turnover.' class=\"bet-type-detail-icon\"></span>",
            Name: "MemberMargin", Width: 120, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Margin", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00")
        },
        { HeaderText: "Win/Loss", Name: "AgentWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Agent-Win Loss", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Comm", Name: "AgentComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Agent-Comm", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Total", Name: "AgentTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Agent-Total", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Win/Loss", Name: "MasterWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master-Win Loss", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Comm", Name: "MasterComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master-Comm", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Total", Name: "MasterTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master-Total", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Win/Loss", Name: "SuperWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super-Win Loss", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Comm", Name: "SuperComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super-Comm", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Total", Name: "SuperTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super-Total", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Company", Name: "CompanyTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Company", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") }
    ]
    public static readonly GroupHeaderColumns = [
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

    constructor( options: IGridComponentConfig ) {
        super(options);
        this.gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [ { Columns: SummaryGrid.GroupHeaderColumns } ],
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
                pagerSelectCallBack: ( pageIndex, pageSize ) => {
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
                serverSortFunction: ( colname ) => {
                }
            },
            showHighlightRow: false,
        }
    }


    public buildBodyRows( reportData: any ): Array<any> {
        const bodyData = [];
        const { Events, GroupLeagues } = reportData;
        for (const leagueGroup of GroupLeagues) {
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
            for (const element of Events) {
                if (element.LeagueId != leagueGroup.LeagueId) continue
                const { HomeName, AwayName, IsNeutral, KickOffTimeStr } = element
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
                        },
                        ...this.buildCommonColumns(element),
                    ]
                });
            }
        }
        return bodyData;
    }

    public buildFooterRows( reportData ): Array<any> {
        const { Total, SystemParlayOutright, GrandTotal, Events } = reportData;
        const totalText = reportData.currency ? `Total in ${reportData.currency}` : "Total";
        const grandTotalText = reportData.currency ? `Grand Total in ${reportData.currency}` : "Grand Total";
        const footerColumns = [];
        const mergeColNumber = 2;

        if (Events) {
            if (Events.length > 0 && JSON.stringify(Total) !== JSON.stringify(GrandTotal)) {
                footerColumns.push({
                    Columns: [
                        { Name: "KickOffTime", Value: totalText, MergeColNumber: mergeColNumber, CellAlign: "right" },
                        ...this.buildCommonColumns(Total),
                    ],
                    RowHeight: 21,
                    CssClass: "row-total"
                })
            }
        }

        if (SystemParlayOutright.length > 0) {
            SystemParlayOutright.forEach(( e ) => {
                footerColumns.push({
                    Columns: [
                        { Name: "KickOffTime", Value: e["BetTypeName"], MergeColNumber: mergeColNumber, CellAlign: "right" },
                        ...this.buildCommonColumns(e),
                    ],
                    RowHeight: 21,
                    CssClass: 'system-parlay'
                })
            })
        }

        if (GrandTotal) {
            footerColumns.push({
                Columns: [
                    { Name: "KickOffTime", Value: grandTotalText, MergeColNumber: mergeColNumber, CellAlign: "right" },
                    ...this.buildCommonColumns(GrandTotal),
                ],
                RowHeight: 21,
                CellAlign: "right",
                CssClass: 'row-grand-total'
            })
        }

        return footerColumns
    }

    public getDefaultColumns(): Array<any> {
        return SummaryGrid.DefaultColumns.filter(e => e.Name != "BetType");
    };

}

class DetailGrid extends MatchWinLossGrid {
    public static readonly DefaultColumns = [
        { HeaderText: "KO Time\n(GMT+8)", Name: "KickOffTime", Width: 80, Sort: false, HeaderAlign: "center", ColumnName2: "KO Time (GMT+8)", CellAlign: "center", },
        { HeaderText: "Event", Name: "Event", Width: 380, Sort: false, HeaderAlign: "center", ColumnName2: "Event", },
        { HeaderText: "BetType", Name: "BetType", Width: 200, Sort: false, HeaderAlign: "center", ColumnName2: "BetType", },
        { HeaderText: "Avg.Stake", Name: "StakeAvg", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", ColumnName2: "Avg.Stake", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Bet Count", Name: "BetCount", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", ColumnName2: "Bet Count", CustomCellRender: ( cell ) => roundMoney(cell) },
        { HeaderText: "Turnover", Name: "Turnover", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Turnover", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Net Turnover", Name: "NetTurnover", Width: 120, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Net Turnover", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "GrossComm", Name: "GrossComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "GrossComm", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Win/Loss", Name: "MemberWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Win Loss", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Comm", Name: "MemberComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Comm", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Total", Name: "MemberTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Total", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        {
            HeaderText: "Margin(%)&nbsp;<span title='Member Margin is calculated by dividing the sum of Member Win/Loss and Commission by the Net Turnover.' class=\"bet-type-detail-icon\"></span>",
            Name: "MemberMargin", Width: 120, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Margin", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00")
        },
        { HeaderText: "Win/Loss", Name: "AgentWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Agent-Win Loss", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Comm", Name: "AgentComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Agent-Comm", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Total", Name: "AgentTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Agent-Total", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Win/Loss", Name: "MasterWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master-Win Loss", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Comm", Name: "MasterComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master-Comm", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Total", Name: "MasterTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master-Total", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Win/Loss", Name: "SuperWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super-Win Loss", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Comm", Name: "SuperComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super-Comm", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Total", Name: "SuperTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super-Total", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") },
        { HeaderText: "Company", Name: "CompanyTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Company", CustomCellRender: ( cell ) => roundMoney(cell, "0,0.00") }
    ]
    public static readonly GroupHeaderColumns = [
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

    constructor( options: IGridComponentConfig ) {
        super(options);
        this.gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [ { Columns: DetailGrid.GroupHeaderColumns } ],
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
                pagerSelectCallBack: ( pageIndex, pageSize ) => {
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
                serverSortFunction: ( colname ) => {
                }
            },
            showHighlightRow: false,
            numberOfColumnHaveMergeRow: 2,
            enableHiddenColumns: true
        }
    }

    public buildBodyRows( reportData: any ): Array<any> {
        const rows = [];
        let currentMergeRowNumber = 0;
        const { Events, GroupLeagues } = reportData;
        const subTotalText = reportData.currency ? `Sub Total in ${reportData.currency}` : "Sub Total";
        for (const leagueGroup of GroupLeagues) {
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
            for (const element of Events) {
                if (element.LeagueId != leagueGroup.LeagueId) continue
                const currentColumns = [];
                const { MatchId, HomeName, AwayName, IsNeutral, KickOffTimeStr, BetTypeName, BetTypeId } = element;
                if (currentMergeRowNumber == 0) {
                    currentMergeRowNumber = Events.filter(e => e.MatchId == MatchId).length;
                    currentColumns.push(
                        {
                            Name: "KickOffTime",
                            Value: moment(KickOffTimeStr).format('MM/DD/YYYY hh:mm A'),
                            MergeRowNumber: currentMergeRowNumber,
                            CssClass: 'break-space'
                        },
                        {
                            Name: "Event",
                            Value: this.buildEventName(HomeName.trim(), AwayName.trim(), IsNeutral),
                            MergeRowNumber: currentMergeRowNumber,
                            CssClass: 'break-space'
                        },
                    );
                }
                const isBetType0 = BetTypeId == 0;
                rows.push({
                    Columns: currentColumns.concat([
                        {
                            Name: "BetType",
                            Value: isBetType0 ? subTotalText : BetTypeName
                        },
                        ...this.buildCommonColumns(element),
                    ]),
                    CssClass: isBetType0 ? 'sub-total' : ''
                });
                currentMergeRowNumber--;
            }
        }
        return rows;
    }

    getDefaultColumns(): Array<any> {
        return DetailGrid.DefaultColumns;
    }

    public buildFooterRows( reportData ): Array<any> {
        const { Total, SystemParlayOutright, GrandTotal, Events } = reportData;
        const totalText = reportData.currency ? `Total in ${reportData.currency}` : "Total";
        const grandTotalText = reportData.currency ? `Grand Total in ${reportData.currency}` : "Grand Total";
        const footerColumns = [];
        const mergeColNumber = 3;

        if (Events) {
            if (Events.length > 0 && JSON.stringify(Total) !== JSON.stringify(GrandTotal)) {
                footerColumns.push({
                    Columns: [
                        { Name: "KickOffTime", Value: totalText, MergeColNumber: mergeColNumber, CellAlign: "right" },
                        ...this.buildCommonColumns(Total),
                    ],
                    RowHeight: 21,
                    CssClass: "row-total"
                })
            }
        }

        if (SystemParlayOutright.length > 0) {
            SystemParlayOutright.forEach(( e ) => {
                footerColumns.push({
                    Columns: [
                        {
                            Name: "KickOffTime",
                            Value: e["BetTypeName"],
                            MergeColNumber: mergeColNumber,
                            CellAlign: "right"
                        },
                        ...this.buildCommonColumns(e),
                    ],
                    RowHeight: 21,
                    CssClass: 'system-parlay'
                })
            })
        }

        if (GrandTotal) {
            footerColumns.push({
                Columns: [
                    { Name: "KickOffTime", Value: grandTotalText, MergeColNumber: mergeColNumber, CellAlign: "right" },
                    ...this.buildCommonColumns(GrandTotal),
                ],
                RowHeight: 21,
                CellAlign: "right",
                CssClass: "row-grand-total"
            })
        }

        return footerColumns;
    }
}