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
var DailyWinLossGrid = /** @class */ (function (_super) {
    __extends(DailyWinLossGrid, _super);
    function DailyWinLossGrid(options) {
        var _this = _super.call(this, options) || this;
        _this.gridOptions = {
            columns: DailyWinLossGrid.DefaultColumns,
            //numberOfFrozenColumn: 2,
            autoCalculateGridExpandHeight: false,
            fullHeight: !options.isMobile.phone,
            gridHeight: options.isMobile.phone ? $(window).height() - 70 : 700,
            showTitle: true,
            titleInfo: {
                customButtons: "<span id=" + options.excelBtnId + " title=\"Export Excel\" class=\"icon-excel\"></span>"
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
        };
        return _this;
    }
    DailyWinLossGrid.prototype.buildBodyRows = function (reportData) {
        var bodyData = [];
        if (reportData) {
            for (var _i = 0, reportData_1 = reportData; _i < reportData_1.length; _i++) {
                var element = reportData_1[_i];
                var WinLostDate = element.WinLostDate, WhiteLabelName = element.WhiteLabelName, Super = element.Super, Master = element.Master, Agent = element.Agent, MemberWinLoss = element.MemberWinLoss, Promotion = element.Promotion;
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
    };
    DailyWinLossGrid.prototype.buildFooterRows = function (data) {
        var footerColumns = [];
        if (data) {
            footerColumns.push({
                Columns: [
                    { Name: "WinLostDate", Value: "Grand Total", MergeColNumber: 5, CellAlign: "right" },
                    { Name: "MemberWinLoss", Value: data.MemberWinLoss, CellAlign: "right" },
                    { Name: "Promotion", Value: data.Promotion, CellAlign: "right" },
                ]
            });
        }
        return footerColumns;
    };
    DailyWinLossGrid.prototype.getDefaultColumns = function () {
        return DailyWinLossGrid.DefaultColumns;
    };
    DailyWinLossGrid.prototype.getGroupHeaderColumns = function () {
        return [];
    };
    DailyWinLossGrid.prototype.getGridOptions = function () {
        return this.gridOptions;
    };
    DailyWinLossGrid.DefaultColumns = [
        { HeaderText: "Date", Name: "WinLostDate", Width: 100, Sort: true, HeaderAlign: "center" },
        { HeaderText: "White Label", Name: "WhiteLabelName", Width: 180, Sort: true, HeaderAlign: "center" },
        { HeaderText: "Agent", Name: "Agent", Width: 160, Sort: true, HeaderAlign: "center" },
        { HeaderText: "Master", Name: "Master", Width: 160, Sort: true, HeaderAlign: "center" },
        { HeaderText: "Super", Name: "Super", Width: 140, Sort: true, HeaderAlign: "center", },
        { HeaderText: "Member Win Loss", Name: "MemberWinLoss", Width: 150, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Promotion", Name: "Promotion", Width: 120, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" }
    ];
    DailyWinLossGrid.ExcelBtnId = 'exportWhiteLabelDailyWinLoss';
    return DailyWinLossGrid;
}(BaseReportGrid));
var SettlementWinLossGrid = /** @class */ (function (_super) {
    __extends(SettlementWinLossGrid, _super);
    function SettlementWinLossGrid(options) {
        var _this = _super.call(this, options) || this;
        _this.gridOptions = {
            customizeHeader: true,
            columns: SettlementWinLossGrid.DefaultColumns,
            groupHeaderRows: [{ Columns: SettlementWinLossGrid.GroupHeaderColumns }],
            //numberOfFrozenColumn: 4,
            autoCalculateGridExpandHeight: false,
            fullHeight: !options.isMobile.phone,
            gridHeight: options.isMobile.phone ? $(window).height() - 70 : 700,
            showTitle: true,
            titleInfo: {
                customButtons: "<span id=" + options.copyBtnId + " title=\"Copy\" class=\"icon-copy\"></span><span id=" + options.excelBtnId + " title=\"Export Excel\" class=\"icon-excel\"></span>"
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
        };
        _this.initSelectColumnsDropdown(SettlementWinLossGrid.DefaultColumns, [0, 1]);
        _this.initCopyBtn(options.copyBtnId);
        return _this;
    }
    SettlementWinLossGrid.prototype.initSelectColumnsDropdown = function (columns, ignoreColumns) {
        var _this = this;
        var options = [];
        var numberOfColumns = columns.length;
        for (var i = 0; i < numberOfColumns; i++) {
            if (ignoreColumns.includes(i))
                continue;
            options.push({
                id: i.toString(),
                value: i.toString(),
                text: columns[i].ColumnName2,
                checked: true,
                enable: true
            });
        }
        $("#" + this.options.selectColumnsId).fanexMultiSelectSearchable({
            showFilter: true,
            checkAllText: "Select All",
            noneSelectedText: "None Selected",
            useCustomCheckAllText: true,
            customCheckAllText: "All",
            isCheckAll: true,
            listData: options,
            onLeave: function () { return _this.showHideColumns(); },
            toggleWidth: this.options.isMobile.phone ? 255 : 265,
            minWidth: this.options.isMobile.phone ? 255 : 265
        });
    };
    SettlementWinLossGrid.prototype.buildBodyRows = function (reportData) {
        var bodyData = [];
        if (!reportData) {
            return bodyData;
        }
        for (var _i = 0, reportData_2 = reportData; _i < reportData_2.length; _i++) {
            var element = reportData_2[_i];
            bodyData.push({
                Columns: [
                    { Name: "Month", Value: element.Month, CellAlign: "center" },
                    { Name: "WhiteLabel", Value: element.WhiteLabel },
                    { Name: "Agent", Value: element.Agent },
                    { Name: "Master", Value: element.Master },
                    { Name: "Super", Value: element.Super },
                    { Name: "MemberWinLoss", Value: element.MemberWL },
                    { Name: "MemberPromotion", Value: element.MemberPromotion },
                    { Name: "LossCarryforward", Value: element.LossCarryforward },
                    { Name: "WhiteLabelWinLoss", Value: element.WhiteLabelWinLoss },
                    { Name: "WhiteLabelNetWinLoss", Value: element.WhiteLabelNetWinLoss },
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
    };
    SettlementWinLossGrid.prototype.buildFooterRows = function (data) {
        var footerColumns = [];
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
    };
    SettlementWinLossGrid.prototype.getDefaultColumns = function () {
        return SettlementWinLossGrid.DefaultColumns;
    };
    SettlementWinLossGrid.prototype.getGroupHeaderColumns = function () {
        return SettlementWinLossGrid.GroupHeaderColumns;
    };
    SettlementWinLossGrid.prototype.getGridOptions = function () {
        return this.gridOptions;
    };
    SettlementWinLossGrid.DefaultColumns = [
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
        { HeaderText: "Promotion Settlement", Name: "WhiteLabelPromotionSettlement", Width: 160, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "WhiteLabel - Promotion Settlement" },
        { HeaderText: "Promotion Share", Name: "WhiteLabelPromotionShare", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "White Label - Promotion Share" },
        { HeaderText: "Total Fee", Name: "WhiteLabelTotal", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "White Label - Total Fee" },
        { HeaderText: "Master Earning", Name: "MasterEarning", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master Earning" },
        { HeaderText: "Super Earning", Name: "SuperEarning", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super Earning" },
        { HeaderText: "Headline Earning", Name: "HeadlineEarning", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Headline Earning" },
        { HeaderText: "Income", Name: "CompanyIncome", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Company - Income" },
        { HeaderText: "Promotion Share", Name: "CompanyPromotionShare", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Company - Promotion Share" },
        { HeaderText: "Total", Name: "CompanyTotal", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Company - Total" }
    ];
    SettlementWinLossGrid.GroupHeaderColumns = [
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
    SettlementWinLossGrid.ExcelBtnId = 'exportWhiteLabelSettlement';
    SettlementWinLossGrid.CopyBtnId = 'copyWhiteLabelSettlement';
    return SettlementWinLossGrid;
}(BaseReportGrid));
//# sourceMappingURL=WhiteLabelWinLossGrid.js.map