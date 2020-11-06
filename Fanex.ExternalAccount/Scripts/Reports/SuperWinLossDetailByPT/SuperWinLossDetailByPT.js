var SuperWinLossDetailByPT = /** @class */ (function () {
    function SuperWinLossDetailByPT($, topWindow) {
        this.$ = $;
        this.topWindow = topWindow;
        this.gridOption = {};
        this.groupName = "";
        this.fixedsortdata = new Array();
        this.fixedsortlist = new Array();
        this.dataSource = [];
        this.grandTotal = null;
        this.pageIndex = 0;
        this.pageSize = 50;
        this.totalRecords = 0;
        this.hideFilter = false;
        this.dateTimeControls = new DateTimeControls($);
        this.productControls = new ProductControls($);
        this.defaultColumns = [
            { HeaderText: "Username / PT / Product", Name: "Username", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "left", ShowCollapseExpand: true },
            { HeaderText: "Turnover", Name: "Turnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Net Turnover", Name: "NetTurnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "GrossComm", Name: "GrossComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Win/Loss", Name: "MemberWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Comm", Name: "PlayerComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Total", Name: "MemberTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Win/Loss", Name: "AgWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Comm", Name: "AgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Total", Name: "AgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Win/Loss", Name: "MAgWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Comm", Name: "MAgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Total", Name: "MAgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Win/Loss", Name: "SAgWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Comm", Name: "SAgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Total", Name: "SAgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Company", Name: "Company", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" }
        ];
        this.defaultHeaderColumns = [
            { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
            { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
            { HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" class=\"bet-type-detail-icon\"></span>", Name: "NetTurnover", MergeRowNumber: 2 },
            { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
            { HeaderText: "Member", Name: "MemberWinlost", MergeColNumber: 3 },
            { HeaderText: "Agent", Name: "AgWinlost", MergeColNumber: 3 },
            { HeaderText: "Master", Name: "MAgWinlost", MergeColNumber: 3 },
            { HeaderText: "Super", Name: "SAgWinlost", MergeColNumber: 3 },
            { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 }
        ];
    }
    SuperWinLossDetailByPT.prototype.Init = function () {
        var _this = this;
        this.dateTimeControls.Init();
        this.$("#userGroup input").attr("readonly", "true");
        $("#userGroup input").bind("click", function () {
            $(this).autocomplete("search", "");
        });
        this.groupName = $("#selectedGroupName").html();
        this.productControls.ProductsList(this.groupName);
        this.$("#Submit").click(function () { _this.CallSubmit(); });
        this.$("#exporttoexcel").click(function () { _this.exportExcel(); });
        this.sorters = this.$(".col-sorter");
        this.sorters.click(function (event) { _this.sortTable(_this.$(event.currentTarget)); });
        Fanex.ExtAcc.OnResize = this.resizeReportGrid;
        this.$(".icon-filter").click(this.showHideFilter);
    };
    SuperWinLossDetailByPT.prototype.exportExcel = function () {
        var dateTimeParams = this.dateTimeControls.GetFromDateToDate();
        var params = "fromdate=" + dateTimeParams.fromdate +
            "&todate=" + dateTimeParams.todate +
            "&groupname=" + this.groupName;
        var url = resolveClientUrl("SuperWinLossDetailByPT/ExportWinLossDetailToExcel?") + params +
            this.productControls.getParameterProducts() +
            this.productControls.getParameterProductDetails();
        this.topWindow.exportExcel(url, document);
    };
    SuperWinLossDetailByPT.prototype.resizeReportGrid = function () {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        }
        catch (err) {
            console.log(err);
        }
    };
    SuperWinLossDetailByPT.prototype.showHideFilter = function () {
        if (this.hideFilter) {
            $(".icon-filter").addClass("active");
            $(".filterbox").removeClass("hide");
        }
        else {
            $(".icon-filter").removeClass("active");
            $(".filterbox").addClass("hide");
        }
        this.resizeReportGrid();
        this.hideFilter = !this.hideFilter;
    };
    SuperWinLossDetailByPT.prototype.CustomUsernameSortFunction = function (cell1Value, cell2Value, sortDirection) {
        return sortDirection === 'asc' ?
            cell1Value.Username > cell2Value.Username ? 1 : -1 :
            cell1Value.Username < cell2Value.Username ? 1 : -1;
    };
    SuperWinLossDetailByPT.prototype.Render = function (dateInfo) {
        var _this = this;
        var arrColData = new Array();
        var arrGroupHeaders = new Array();
        var numberOfFrozenColumn = 3;
        var widthDevice = this.topWindow.$(top.window).width();
        if (widthDevice <= 768) {
            numberOfFrozenColumn = 1;
        }
        arrColData = this.defaultColumns;
        arrGroupHeaders = this.defaultHeaderColumns;
        this.gridOption = {
            customizeHeader: true,
            groupHeaderRows: [{ Columns: arrGroupHeaders }],
            columns: arrColData,
            numberOfFrozenColumn: numberOfFrozenColumn,
            autoCalculateGridExpandHeight: true,
            fullHeight: !isMobile.phone,
            gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
            showTitle: true,
            titleInfo: { text: "Super Win Loss Detail By PT " + dateInfo },
            showFooter: true,
            showTree: true,
            gridExpandHeight: 256,
            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPageArray: [50, 100, 200],
                currentPage: this.pageIndex - 1,
                totalItem: this.totalRecords,
                itemsPerPage: this.pageSize,
                pagerSelectCallBack: function (currentPageIndex, itemsPerPage) {
                    _this.CallSubmit(itemsPerPage, currentPageIndex + 1);
                    _this.$("#jqgrid").fanexGrid(_this.gridOption);
                    _this.$(".product-tooltip").tooltip();
                },
                showDetail: true
            },
            marginRight: 10,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            sortOption: {
                afterGridSorted: function (rows) { return _this.customsortfunc(rows); }
            },
            numOfPaddingForGridExpandHeight: 40
        };
        //// Assing BodyData
        this.InitSort();
        var bodyData = [];
        for (var i = 0; i < this.totalRows; i++) {
            var subRow = [];
            for (var j = 0; j < this.dataSource[i].WinLossByProducts.length; j++) {
                var winLossByProductModel = this.dataSource[i].WinLossByProducts[j];
                subRow.push({
                    Columns: [
                        {
                            HeaderText: "Username", Name: "Username",
                            Value: winLossByProductModel,
                            CustomCellRender: function (cell) { return _this.RenderPositonTakingColumn(cell); }
                        },
                        { Name: "Turnover", Value: winLossByProductModel.Turnover },
                        { Name: "NetTurnover", Value: winLossByProductModel.NetTurnover },
                        { Name: "GrossComm", Value: winLossByProductModel.GrossComm },
                        { Name: "MemberWinlost", Value: winLossByProductModel.MemberWinlost },
                        { Name: "PlayerComm", Value: winLossByProductModel.MemberComm },
                        { Name: "MemberTotal", Value: winLossByProductModel.MemberTotal },
                        { Name: "AgWinlost", Value: winLossByProductModel.AgWinlost },
                        { Name: "AgComm", Value: winLossByProductModel.AgComm },
                        { Name: "AgTotal", Value: winLossByProductModel.AgTotal },
                        { Name: "MAgWinlost", Value: winLossByProductModel.MAgWinlost },
                        { Name: "MAgComm", Value: winLossByProductModel.MAgComm },
                        { Name: "MAgTotal", Value: winLossByProductModel.MAgTotal },
                        { Name: "SAgWinlost", Value: winLossByProductModel.SAgWinlost },
                        { Name: "SAgComm", Value: winLossByProductModel.SAgComm },
                        { Name: "SAgTotal", Value: winLossByProductModel.SAgTotal },
                        { Name: "Company", Value: winLossByProductModel.Company }
                    ]
                });
            }
            var totalRow = this.dataSource[i].Total;
            bodyData.push({
                Columns: [
                    { Name: "Username", Value: totalRow.Username },
                    { Name: "Turnover", Value: totalRow.Turnover },
                    { Name: "NetTurnover", Value: totalRow.NetTurnover },
                    { Name: "GrossComm", Value: totalRow.GrossComm },
                    { Name: "MemberWinlost", Value: totalRow.MemberWinlost },
                    { Name: "PlayerComm", Value: totalRow.MemberComm },
                    { Name: "MemberTotal", Value: totalRow.MemberTotal },
                    { Name: "AgWinlost", Value: totalRow.AgWinlost },
                    { Name: "AgComm", Value: totalRow.AgComm },
                    { Name: "AgTotal", Value: totalRow.AgTotal },
                    { Name: "MAgWinlost", Value: totalRow.MAgWinlost },
                    { Name: "MAgComm", Value: totalRow.MAgComm },
                    { Name: "MAgTotal", Value: totalRow.MAgTotal },
                    { Name: "SAgWinlost", Value: totalRow.SAgWinlost },
                    { Name: "SAgComm", Value: totalRow.SAgComm },
                    { Name: "SAgTotal", Value: totalRow.SAgTotal },
                    { Name: "Company", Value: totalRow.Company }
                ],
                SubRows: subRow,
                CssClass: 'sub-row-class'
            });
        }
        this.gridOption.bodyRows = bodyData;
        this.gridOption.pagerOption.totalItem = this.totalRecords;
        //// Asign Footer Data
        var footerData = [];
        footerData.push({
            Columns: [
                { Name: "Username", Value: "Grand Total" },
                { Name: "Turnover", Value: this.grandTotal.Turnover },
                { Name: "NetTurnover", Value: this.grandTotal.NetTurnover },
                { Name: "GrossComm", Value: this.grandTotal.GrossComm },
                { Name: "MemberWinlost", Value: this.grandTotal.MemberWinlost },
                { Name: "PlayerComm", Value: this.grandTotal.MemberComm },
                { Name: "MemberTotal", Value: this.grandTotal.MemberTotal },
                { Name: "AgWinlost", Value: this.grandTotal.AgWinlost },
                { Name: "AgComm", Value: this.grandTotal.AgComm },
                { Name: "AgTotal", Value: this.grandTotal.AgTotal },
                { Name: "MAgWinlost", Value: this.grandTotal.MAgWinlost },
                { Name: "MAgComm", Value: this.grandTotal.MAgComm },
                { Name: "MAgTotal", Value: this.grandTotal.MAgTotal },
                { Name: "SAgWinlost", Value: this.grandTotal.SAgWinlost },
                { Name: "SAgComm", Value: this.grandTotal.SAgComm },
                { Name: "SAgTotal", Value: this.grandTotal.SAgTotal },
                { Name: "Company", Value: this.grandTotal.Company }
            ]
        });
        this.gridOption.footerRows = footerData;
        setPagerOptionOnMobile(this.gridOption);
        this.$("#jqgrid").fanexGrid(this.gridOption);
        this.$(".product-tooltip").tooltip();
        workaroundForGridOnPhone();
    };
    SuperWinLossDetailByPT.prototype.RenderPositonTakingColumn = function (cell) {
        var tooltip = '';
        this.$.each(cell.Value.ProductNames, function (index, value) {
            tooltip += "<div>" + value + "</div>";
        });
        return "<span class=\"product-tooltip\" title=\"" + tooltip + "\">" + cell.Value.PositionTakingText + "<a>('" + cell.Value.ProductNames.length + ")</a></span>";
    };
    SuperWinLossDetailByPT.prototype.customsortfunc = function (rows) {
        var countFixedSort = this.fixedsortlist.length;
        if (countFixedSort > 0) {
            for (var i = 0; i < countFixedSort; i++) {
                for (var j = 0; j < rows.length; j++) {
                    if (this.fixedsortlist[i] === rows[j].Columns[0].Value.CustId) {
                        this.fixedsortdata.push(rows.slice(j, j + 1)[0]);
                        rows.unshift(rows.splice(j, 1)[0]);
                        break;
                    }
                }
            }
        }
        return rows;
    };
    SuperWinLossDetailByPT.prototype.InitSort = function () {
        var countFixedSort = this.fixedsortlist.length;
        if (countFixedSort > 0) {
            for (var i = 0; i < countFixedSort; i++) {
                for (var j = 0; j < this.dataSource.length; j++) {
                    if (this.fixedsortlist[i] === this.dataSource[j].CustId) {
                        this.fixedsortdata.push(this.dataSource.slice(j, j + 1)[0]);
                        this.dataSource.unshift(this.dataSource.splice(j, 1)[0]);
                        break;
                    }
                }
            }
        }
    };
    SuperWinLossDetailByPT.prototype.showNetTurnoverInfo = function (data) {
        var message = "- For Sportbook, Net Turnover is Turnover which excludes the draw tickets and halves of the turnover of the half won, half lost tickets.";
        var height = 170;
        if (data.TooltipsForLiveCasino !== "") {
            message +=
                "<br/>" +
                    "<br/>" +
                    data.TooltipsForLiveCasino;
            height += 50;
        }
        Fanex.ExtAcc.Form.Alert("Net Turnover Information", message, 430, height);
    };
    SuperWinLossDetailByPT.prototype.CallSubmit = function (pageSize, pageIndex) {
        var _this = this;
        if (pageSize === void 0) { pageSize = 50; }
        if (pageIndex === void 0) { pageIndex = 1; }
        var dateTimeParams = this.dateTimeControls.GetFromDateToDate();
        this.groupName = $("#selectedGroupName").html();
        var dateInfo = dateTimeParams.fromdate === dateTimeParams.todate ?
            "on " + dateTimeParams.fromdate :
            "from " + dateTimeParams.fromdate + " to " + dateTimeParams.todate;
        var parameterUrl = "?Groupname=" + this.groupName
            + "&Fromdate=" + dateTimeParams.fromdate
            + "&Todate=" + dateTimeParams.todate
            + "&PageSize=" + pageSize
            + "&PageIndex=" + pageIndex
            + this.productControls.getParameterProducts()
            + this.productControls.getParameterProductDetails();
        Fanex.ExtAcc.BlockUI();
        this.$.ajax({
            url: resolveClientUrl("SuperWinLossDetailByPT/GetData") + parameterUrl,
            type: "post",
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data.errorCode) {
                    Fanex.ExtAcc.UnBlockUI();
                    Fanex.ExtAcc.Form.Alert("Error", data.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
                    return;
                }
                _this.fixedsortdata = new Array();
                _this.dataSource = data.WinLossByUsernames;
                _this.grandTotal = data.GrandTotal;
                _this.totalRows = data.WinLossByUsernames !== null ? data.WinLossByUsernames.length : 0;
                _this.totalRecords = data.TotalRecords;
                _this.pageSize = data.PageSize;
                _this.pageIndex = data.PageIndex;
                _this.fixedsortlist = [];
                _this.$("#mainGrid").css("display", _this.totalRows < 1 ? "none" : "block");
                _this.$("#mainGrid").css("visibility", _this.totalRows < 1 ? "hidden" : "visible");
                _this.$("#exporttoexcel").css("display", _this.totalRows < 1 ? "none" : "block");
                _this.$("#msgNoinfo").css("display", _this.totalRows < 1 ? "block" : "none");
                if (_this.totalRows > 0) {
                    _this.Render(dateInfo);
                    _this.$("#exporttoexcel").removeClass("hide");
                }
                else {
                    _this.$("#exporttoexcel").addClass("hide");
                }
                $("#netTurnover").unbind().bind("click", function () { return _this.showNetTurnoverInfo(data); });
                Fanex.ExtAcc.UnBlockUI();
            }
        });
    };
    return SuperWinLossDetailByPT;
}());
$(document).ready(function () {
    var superWinLossDetailByPT = new SuperWinLossDetailByPT($, window.top);
    superWinLossDetailByPT.Init();
});
//# sourceMappingURL=SuperWinLossDetailByPT.js.map