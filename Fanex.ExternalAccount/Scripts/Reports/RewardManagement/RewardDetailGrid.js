var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var BaseRewardManagementGrid = /** @class */ (function () {
    function BaseRewardManagementGrid() {
    }
    return BaseRewardManagementGrid;
}());
var RewardDetailGrid = /** @class */ (function (_super) {
    __extends(RewardDetailGrid, _super);
    function RewardDetailGrid(isMobile) {
        var _this = _super.call(this) || this;
        _this.gridOptions = {
            columns: RewardDetailGrid.DefaultColumns,
            //numberOfFrozenColumn: 2,
            autoCalculateGridExpandHeight: false,
            fullHeight: !isMobile.phone,
            gridHeight: isMobile.phone ? $(window).height() - 70 : 700,
            showTitle: true,
            titleInfo: {
                customButtons: "<span id=" + RewardDetailGrid.ExcelBtnId + " title=\"Export Excel\" class=\"icon-excel\"></span>"
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
    RewardDetailGrid.prototype.buildBodyRows = function (reportData) {
        return undefined;
    };
    RewardDetailGrid.prototype.buildFooterRows = function (reportData) {
        return undefined;
    };
    RewardDetailGrid.prototype.getDefaultColumns = function () {
        return undefined;
    };
    RewardDetailGrid.prototype.getGridOptions = function () {
    };
    RewardDetailGrid.prototype.getGroupHeaderColumns = function () {
        return undefined;
    };
    RewardDetailGrid.DefaultColumns = [
    // { HeaderText: "Date", Name: "WinLostDate", Width: 100, Sort: true, HeaderAlign: "center" },
    // { HeaderText: "White Label", Name: "WhiteLabelName", Width: 180, Sort: true, HeaderAlign: "center" },
    // { HeaderText: "Agent", Name: "Agent", Width: 160, Sort: true,  HeaderAlign: "center" },
    // { HeaderText: "Master", Name: "Master", Width: 160, Sort: true, HeaderAlign: "center" },
    // { HeaderText: "Super", Name: "Super", Width: 140, Sort: true, HeaderAlign: "center", },
    // { HeaderText: "Member Win Loss", Name: "MemberWinLoss", Width: 150,  Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
    // { HeaderText: "Promotion", Name: "Promotion", Width: 120, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" }
    ];
    RewardDetailGrid.ExcelBtnId = 'exportRewardDetailExcel';
    return RewardDetailGrid;
}(BaseRewardManagementGrid));
//# sourceMappingURL=RewardDetailGrid.js.map