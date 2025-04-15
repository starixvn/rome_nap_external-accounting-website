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
var RewardDetailGrid = /** @class */ (function (_super) {
    __extends(RewardDetailGrid, _super);
    function RewardDetailGrid(options) {
        var _this = _super.call(this, options) || this;
        _this.gridOptions = {
            customizeHeader: true,
            columns: RewardDetailGrid.DefaultColumns,
            groupHeaderRows: [{ Columns: RewardDetailGrid.GroupHeaderColumns }],
            //numberOfFrozenColumn: 2,
            autoCalculateGridExpandHeight: false,
            fullHeight: !options.isMobile.phone,
            gridHeight: options.isMobile.phone ? $(window).height() - 70 : 700,
            showTitle: true,
            titleInfo: {
                customButtons: "<span id=" + options.copyBtnId + " title=\"Copy\" class=\"icon-copy\"></span><span id=" + options.excelBtnId + " title=\"Export Excel\" class=\"icon-excel\"></span>"
            },
            gridExpandHeight: 260,
            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPageArray: options.pageSizeOptions,
                currentPage: 1,
                totalItem: 10,
                itemsPerPage: options.pageSizeOptions[0],
                pagerSelectCallBack: function (pageIndex, pageSize) {
                }
            },
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            bodyRows: [],
            showFooter: true,
            footerRows: [],
            showHighlightRow: false
        };
        _this.initSelectColumnsDropdown(RewardDetailGrid.DefaultColumns, [0, 1, 2]);
        _this.initCopyBtn(options.copyBtnId);
        return _this;
    }
    RewardDetailGrid.sortDateTime = function (col1Value, col2Value, sortDirection) {
        var parsedCol1Value = moment(col1Value);
        var parsedCol2Value = moment(col2Value);
        if (sortDirection === 'asc') {
            return parsedCol1Value > parsedCol2Value ? 1 : -1;
        }
        else {
            return parsedCol1Value < parsedCol2Value ? 1 : -1;
        }
    };
    RewardDetailGrid.customUsernameCell = function (currentCell) {
        var _a = currentCell.Value, username = _a.username, isDirectMember = _a.isDirectMember;
        return isDirectMember ? username + "<span class='icon-directmember' title=\"Direct Member\"></span>" : username;
    };
    RewardDetailGrid.prototype.buildBodyRows = function (reportData) {
        var bodyData = [];
        function StatusColor(status) {
            return status ? "green" : "red";
        }
        if (reportData) {
            for (var _i = 0, reportData_1 = reportData; _i < reportData_1.length; _i++) {
                var element = reportData_1[_i];
                var CreatedDate = element.CreatedDate, ProgramName = element.ProgramName, MemberUsername = element.MemberUsername, MemberAmount = element.MemberAmount, MemberCurrency = element.MemberCurrency, MemberPaymentStatus = element.MemberPaymentStatus, MemberPaymentStatusMapped = element.MemberPaymentStatusMapped, MemberPaymentDateString = element.MemberPaymentDateString, MemberFeedBackMapped = element.MemberFeedBackMapped, AgentUsername = element.AgentUsername, AgentPaymentStatus = element.AgentPaymentStatus, AgentPaymentStatusMapped = element.AgentPaymentStatusMapped, AgentPaymentDateString = element.AgentPaymentDateString, MasterUsername = element.MasterUsername, MasterPaymentStatus = element.MasterPaymentStatus, MasterPaymentStatusMapped = element.MasterPaymentStatusMapped, MasterPaymentDateString = element.MasterPaymentDateString, SuperUsername = element.SuperUsername, SuperPaymentStatus = element.SuperPaymentStatus, SuperPaymentStatusMapped = element.SuperPaymentStatusMapped, SuperPaymentDateString = element.SuperPaymentDateString, Recommend = element.Recommend;
                bodyData.push({
                    Columns: [
                        { Name: "CreatedDate", Value: this.getDateTime(CreatedDate), CellAlign: "center" },
                        { Name: "ProgramName", Value: ProgramName },
                        { Name: "MemberUsername", Value: { username: MemberUsername, isDirectMember: Recommend == -3 || Recommend == -4 } },
                        { Name: "MemberAmount", Value: MemberAmount },
                        { Name: "MemberCurrency", Value: MemberCurrency, CellAlign: "center" },
                        { Name: "MemberPaymentStatus", Value: MemberPaymentStatusMapped, CellAlign: "center", CssClass: StatusColor(MemberPaymentStatus) },
                        { Name: "MemberPaymentDate", Value: MemberPaymentDateString, CellAlign: "center" },
                        { Name: "MemberFeedBack", Value: MemberFeedBackMapped, CellAlign: "center", CssClass: MemberFeedBackMapped === "Not Received" ? "red" : "" },
                        { Name: "AgentUsername", Value: AgentUsername },
                        { Name: "AgentPaymentStatus", Value: AgentPaymentStatusMapped, CellAlign: "center", CssClass: StatusColor(AgentPaymentStatus) },
                        { Name: "AgentPaymentDate", Value: AgentPaymentDateString, CellAlign: "center" },
                        { Name: "MasterUsername", Value: MasterUsername },
                        { Name: "MasterPaymentStatus", Value: MasterPaymentStatusMapped, CellAlign: "center", CssClass: StatusColor(MasterPaymentStatus) },
                        { Name: "MasterPaymentDate", Value: MasterPaymentDateString, CellAlign: "center" },
                        { Name: "SuperUsername", Value: SuperUsername },
                        { Name: "SuperPaymentStatus", Value: SuperPaymentStatusMapped, CellAlign: "center", CssClass: StatusColor(SuperPaymentStatus) },
                        { Name: "SuperPaymentDate", Value: SuperPaymentDateString, CellAlign: "center" }
                    ]
                });
            }
        }
        this.gridBodyRowsCache = bodyData;
        return bodyData;
    };
    RewardDetailGrid.prototype.buildFooterRows = function (reportData) {
        return undefined;
    };
    RewardDetailGrid.prototype.getDefaultColumns = function () {
        return RewardDetailGrid.DefaultColumns;
    };
    RewardDetailGrid.prototype.getGridOptions = function () {
        return this.gridOptions;
    };
    RewardDetailGrid.prototype.getGroupHeaderColumns = function () {
        return RewardDetailGrid.GroupHeaderColumns;
    };
    RewardDetailGrid.prototype.getDateTime = function (date) {
        if (!date)
            return "";
        return moment(date).format("MM/DD/YYYY hh:mm:ss A");
    };
    RewardDetailGrid.DefaultColumns = [
        { HeaderText: "Created Date", Name: "CreatedDate", Width: 170, Sort: true, HeaderAlign: "center", ColumnName2: "Created Date", CustomSortFunction: RewardDetailGrid.sortDateTime },
        { HeaderText: "Program", Name: "ProgramName", Width: 250, Sort: true, HeaderAlign: "center", AllowTruncate: true, ColumnName2: "Program" },
        { HeaderText: "Username", Name: "MemberUsername", Width: 180, Sort: true, HeaderAlign: "center", AllowTruncate: true, ColumnName2: "Member Username", CustomCellRender: RewardDetailGrid.customUsernameCell },
        { HeaderText: "Amount", Name: "MemberAmount", Width: 150, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member Amount" },
        { HeaderText: "Reward Currency", Name: "MemberCurrency", Width: 140, Sort: true, HeaderAlign: "center", ColumnName2: "Member Reward Currency" },
        { HeaderText: "Payment Status", Name: "MemberPaymentStatus", Width: 120, Sort: true, HeaderAlign: "center", ColumnName2: "Member Payment Status" },
        { HeaderText: "Payment Date", Name: "MemberPaymentDate", Width: 110, Sort: true, HeaderAlign: "center", ColumnName2: "Member Payment Date" },
        { HeaderText: "Feedback", Name: "MemberFeedBack", Width: 110, Sort: true, HeaderAlign: "center", ColumnName2: "Member Feedback" },
        { HeaderText: "Username", Name: "AgentUsername", Width: 180, Sort: true, HeaderAlign: "center", AllowTruncate: true, ColumnName2: "Agent Username" },
        { HeaderText: "Payment Status", Name: "AgentPaymentStatus", Width: 120, Sort: true, HeaderAlign: "center", ColumnName2: "Agent Payment Status" },
        { HeaderText: "Payment Date", Name: "AgentPaymentDate", Width: 110, Sort: true, HeaderAlign: "center", ColumnName2: "Agent Payment Date" },
        { HeaderText: "Username", Name: "MasterUsername", Width: 180, Sort: true, HeaderAlign: "center", AllowTruncate: true, ColumnName2: "Master Username" },
        { HeaderText: "Payment Status", Name: "MasterPaymentStatus", Width: 120, Sort: true, HeaderAlign: "center", ColumnName2: "Master Payment Status" },
        { HeaderText: "Payment Date", Name: "MasterPaymentDate", Width: 110, Sort: true, HeaderAlign: "center", ColumnName2: "Master Payment Date" },
        { HeaderText: "Username", Name: "SuperUsername", Width: 180, Sort: true, HeaderAlign: "center", AllowTruncate: true, ColumnName2: "Super Username" },
        { HeaderText: "Payment Status", Name: "SuperPaymentStatus", Width: 120, Sort: true, HeaderAlign: "center", ColumnName2: "Super Payment Status" },
        { HeaderText: "Payment Date", Name: "SuperPaymentDate", Width: 110, Sort: true, HeaderAlign: "center", ColumnName2: "Super Payment Date" },
    ];
    RewardDetailGrid.GroupHeaderColumns = [
        { HeaderText: "Created Date", Name: "CreatedDate", MergeRowNumber: 2 },
        { HeaderText: "Program", Name: "ProgramName", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberUsername", MergeColNumber: 6 },
        { HeaderText: "Agent", Name: "AgentUsername", MergeColNumber: 3 },
        { HeaderText: "Master", Name: "MasterUsername", MergeColNumber: 3 },
        { HeaderText: "Super", Name: "SuperUsername", MergeColNumber: 3 },
    ];
    return RewardDetailGrid;
}(BaseReportGrid));
//# sourceMappingURL=RewardDetailGrid.js.map