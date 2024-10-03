class DailyWinLossGrid extends BaseReportGrid {
    protected static DefaultColumns = [
        { HeaderText: "Date", Name: "WinLostDate", Width: 100, Sort: true, HeaderAlign: "center" },
        { HeaderText: "White Label", Name: "WhiteLabelName", Width: 180, Sort: true, HeaderAlign: "center" },
        { HeaderText: "Agent", Name: "Agent", Width: 160, Sort: true,  HeaderAlign: "center" },
        { HeaderText: "Master", Name: "Master", Width: 160, Sort: true, HeaderAlign: "center" },
        { HeaderText: "Super", Name: "Super", Width: 140, Sort: true, HeaderAlign: "center", },
        { HeaderText: "Member Win Loss", Name: "MemberWinLoss", Width: 150,  Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Promotion", Name: "Promotion", Width: 120, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" }
    ]

    public static readonly ExcelBtnId = 'exportWhiteLabelDailyWinLoss';

    constructor( options: IGridComponentConfig) {
        super(options);
        this.gridOptions = {
            columns: DailyWinLossGrid.DefaultColumns,
            //numberOfFrozenColumn: 2,
            autoCalculateGridExpandHeight: false,
            fullHeight: !options.isMobile.phone,
            gridHeight: options.isMobile.phone ? $(window).height() - 70 : 700,
            showTitle: true,
            titleInfo: {
                customButtons: `<span id=${options.excelBtnId} title="Export Excel" class="icon-excel"></span>`
            },
            gridExpandHeight: 260,
            showPager: false,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            bodyRows: [],
            showFooter: true,
            footerRows: [],
            showHighlightRow: false
        }
    }

    buildBodyRows(reportData: any): Array<any> {
        const bodyData = [];

        if (reportData) {
            for (const element of reportData) {
                const { WinLostDate, WhiteLabelName, Super, Master, Agent, MemberWinLoss, Promotion } = element
                bodyData.push({
                    Columns: [
                        { Name: "WinLostDate", Value: moment(WinLostDate).format("MM/DD/YYYY"), CellAlign: "center" },
                        { Name: "WhiteLabelName", Value: WhiteLabelName },
                        { Name: "Agent", Value: Agent },
                        { Name: "Master", Value: Master },
                        { Name: "Super", Value: Super },
                        { Name: "MemberWinLoss", Value: MemberWinLoss },
                        { Name: "Promotion", Value: Promotion }
                    ]
                });
            }
        }

        return bodyData;
    }

    buildFooterRows(data: any): Array<any> {
        const footerColumns = [];

        if (data) {
            footerColumns.push({
                Columns: [
                    { Name: "WinLostDate", Value: "Grand Total", MergeColNumber: 5, CellAlign: "right" },
                    { Name: "MemberWinLoss", Value: data.MemberWinLoss, CellAlign: "right" },
                    { Name: "Promotion", Value: data.Promotion, CellAlign: "right" },
                ]
            })
        }

        return footerColumns
    }

    getDefaultColumns(): Array<any> {
        return DailyWinLossGrid.DefaultColumns;
    }

    getGroupHeaderColumns(): Array<any> {
        return [];
    }

    getGridOptions(): any {
        return this.gridOptions
    }
}

class SettlementWinLossGrid extends BaseReportGrid {
    protected static DefaultColumns = [
        { HeaderText: "Month", Name: "Month", Width: 80, Sort: true, HeaderAlign: "center", ColumnName2: "Month" },
        { HeaderText: "White Label", Name: "WhiteLabel", Width: 130, Sort: true, HeaderAlign: "center", ColumnName2: "White Label" },
        { HeaderText: "Agent", Name: "Agent", Width: 130, Sort: true, HeaderAlign: "center", ColumnName2: "Agent" },
        { HeaderText: "Master", Name: "Master", Width: 120, Sort: true, HeaderAlign: "center", ColumnName2: "Master" },
        { HeaderText: "Super", Name: "Super", Width: 110, Sort: true, HeaderAlign: "center", ColumnName2: "Super" },

        { HeaderText: "Win Loss", Name: "MemberWinLoss", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member - Win Loss" },
        { HeaderText: "Promotion", Name: "MemberPromotion", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member - Promotion" },

        { HeaderText: "Loss C/f", Name: "LossCarryforward", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "White Label - Loss C/f" },
        { HeaderText: "Win Loss", Name: "WhiteLabelWinLoss", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "White Label - Win Loss" },
        { HeaderText: "Net Win Loss", Name: "WhiteLabelNetWinLoss", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "White Label - Net Win Loss" },
        { HeaderText: "Gross Fee", Name: "WhiteLabelFee", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "White Label - Gross Fee", },
        { HeaderText: "Promotion Settlement", Name: "WhiteLabelPromotionSettlement", Width: 150, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "WhiteLabel - Promotion Settlement" },
        { HeaderText: "Promotion Share", Name: "WhiteLabelPromotionShare", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "White Label - Promotion Share" },
        { HeaderText: "Total", Name: "WhiteLabelTotal", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "White Label - Total" },
       
        { HeaderText: "Master Earning", Name: "MasterEarning", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master Earning" },
        { HeaderText: "Super Earning", Name: "SuperEarning", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super Earning" },
        { HeaderText: "Headline Earning", Name: "HeadlineEarning", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Headline Earning" },
       
        { HeaderText: "Income", Name: "CompanyIncome", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Company - Income" },
        { HeaderText: "Promotion Share", Name: "CompanyPromotionShare", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Company - Promotion Share" },
        { HeaderText: "Total", Name: "CompanyTotal", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Company - Total" }
    ];

    public static readonly GroupHeaderColumns = [
        { HeaderText: "Month", Name: "Month", MergeRowNumber: 2 },
        { HeaderText: "White Label", Name: "WhiteLabel", MergeRowNumber: 2 },
        { HeaderText: "Agent", Name: "Agent", MergeRowNumber: 2 },
        { HeaderText: "Master", Name: "Master", MergeRowNumber: 2 },
        { HeaderText: "Super", Name: "Super", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberWinLoss", MergeColNumber: 2 },
        { HeaderText: "White Label", Name: "LossCarryforward", MergeColNumber: 7 },
        { HeaderText: "Master Earning", Name: "MasterEarning", MergeRowNumber: 2 },
        { HeaderText: "Super Earning", Name: "SuperEarning", MergeRowNumber: 2 },
        { HeaderText: "Headline Earning", Name: "HeadlineEarning", MergeRowNumber: 2 },
        { HeaderText: "Company", Name: "CompanyIncome", MergeColNumber: 3 }
    ];

    public static readonly ExcelBtnId = 'exportWhiteLabelSettlement';
    public static readonly CopyBtnId = 'copyWhiteLabelSettlement';

    constructor(options: IGridComponentConfig) {
        super(options);
        this.gridOptions = {
            customizeHeader: true,
            columns: SettlementWinLossGrid.DefaultColumns,
            groupHeaderRows: [{ Columns: SettlementWinLossGrid.GroupHeaderColumns }],
            //numberOfFrozenColumn: 4,
            autoCalculateGridExpandHeight: false,
            fullHeight: !options.isMobile.phone,
            gridHeight: options.isMobile.phone ? $(window).height() - 70 : 700,
            showTitle: true,
            titleInfo: {
                customButtons: `<span id=${options.copyBtnId} title="Copy" class="icon-copy"></span><span id=${options.excelBtnId} title="Export Excel" class="icon-excel"></span>`
            },
            gridExpandHeight: 320,
            showPager: false,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            bodyRows: [],
            showFooter: true,
            footerRows: [],
            showHighlightRow: false
        }
        this.initSelectColumnsDropdown(SettlementWinLossGrid.DefaultColumns, [ 0, 1 ]);
        this.initCopyBtn(options.copyBtnId);
    }
    
    initSelectColumnsDropdown( columns: any, ignoreColumns: any ) {
        const options = [];
        const numberOfColumns = columns.length;
        for (let i = 0; i < numberOfColumns; i++) {
            if (ignoreColumns.includes(i)) continue;
            options.push({
                id: i.toString(),
                value: i.toString(),
                text: columns[i].ColumnName2,
                checked: true,
                enable: true
            })
        }

        $(`#${this.options.selectColumnsId}`).fanexMultiSelectSearchable({
            showFilter: true,
            checkAllText: "Select All",
            noneSelectedText: "None Selected",
            useCustomCheckAllText: true,
            customCheckAllText: "All",
            isCheckAll: true,
            listData: options,
            onLeave: () => this.showHideColumns(),
            toggleWidth: this.options.isMobile.phone ? 255 : 265,
            minWidth: this.options.isMobile.phone ? 255 : 265
        });
    }

    buildBodyRows(reportData: any): Array<any> {
        const bodyData = [];
        if (!reportData) {
            return bodyData;
        }

        for (const element of reportData) {
            bodyData.push({
                Columns: [
                    { Name: "Month", Value: element.Month, CellAlign: "center" },
                    { Name: "WhiteLabel", Value: element.WhiteLabel },
                    { Name: "Agent",  Value: element.Agent },
                    { Name: "Master", Value: element.Master },
                    { Name: "Super", Value: element.Super },
                    
                    { Name: "MemberWinLoss", Value: element.MemberWL },
                    { Name: "MemberPromotion", Value: element.MemberPromotion },
                    { Name: "LossCarryforward", Value: element.LossCarryforward },
                    { Name: "WhiteLabelWinLoss",  Value: element.WhiteLabelWinLoss },
                    { Name: "WhiteLabelNetWinLoss",  Value: element.WhiteLabelNetWinLoss },
                    { Name: "WhiteLabelFee", Value: element.WhiteLabelFee },
                    { Name: "WhiteLabelPromotionSettlement", Value: element.WhiteLabelPromotionSettlement },
                    { Name: "WhiteLabelPromotionShare", Value: element.WhiteLabelPromotionShare },
                    { Name: "WhiteLabelTotal", Value: element.WhiteLabelTotal },
                    { Name: "MasterEarning", Value: element.MasterEarning },
                    { Name: "SuperEarning", Value: element.SuperEarning },
                    { Name: "HeadlineEarning", Value: element.HeadlineEarning },
                    { Name: "CompanyIncome", Value: element.CompanyIncome },
                    { Name: "CompanyPromotionShare", Value: element.CompanyPromotionShare },
                    { Name: "CompanyTotal", Value: element.CompanyTotal }
                ]
            });
        }
        
        this.gridBodyRowsCache = bodyData;
        return bodyData;
    }

    buildFooterRows(data: any): Array<any> {
        const footerColumns = [];
        data && footerColumns.push({
            Columns: [
                { Name: "Month", Value: "Grand Total", MergeColNumber: 5, CellAlign: "right" },
                
                { Name: "MemberWinLoss", Value: data.MemberWL, CellAlign: "right" },
                { Name: "MemberPromotion", Value: data.MemberPromotion, CellAlign: "right" },
                { Name: "LossCarryforward", Value: data.LossCarryforward, CellAlign: "right" },
                { Name: "WhiteLabelWinLoss", Value: data.WhiteLabelWinLoss, CellAlign: "right" },
                { Name: "WhiteLabelNetWinLoss", Value: data.WhiteLabelNetWinLoss, CellAlign: "right" },
                { Name: "WhiteLabelFee", Value: data.WhiteLabelFee, CellAlign: "right" },
                { Name: "WhiteLabelPromotionSettlement", Value: data.WhiteLabelPromotionSettlement, CellAlign: "right" },
                { Name: "WhiteLabelPromotionShare", Value: data.WhiteLabelPromotionShare, CellAlign: "right" },
                { Name: "WhiteLabelTotal", Value: data.WhiteLabelTotal, CellAlign: "right" },
                { Name: "MasterEarning", Value: data.MasterEarning, CellAlign: "right" },
                { Name: "SuperEarning", Value: data.SuperEarning, CellAlign: "right" },
                { Name: "HeadlineEarning", Value: data.HeadlineEarning, CellAlign: "right" },
                { Name: "CompanyIncome", Value: data.CompanyIncome, CellAlign: "right" },
                { Name: "CompanyPromotionShare", Value: data.CompanyPromotionShare, CellAlign: "right" },
                { Name: "CompanyTotal", Value: data.CompanyTotal, CellAlign: "right" },
            ]
        });
        
        this.gridFooterRowsCache = footerColumns;
        return footerColumns;
    }

    getDefaultColumns(): Array<any> {
        return SettlementWinLossGrid.DefaultColumns;
    }
    
    getGroupHeaderColumns(): Array<any> {
        return SettlementWinLossGrid.GroupHeaderColumns;
    }

    getGridOptions(): any {
        return this.gridOptions
    }
}