var SuperWinLossDetailByPT = /** @class */ (function () {
    function SuperWinLossDetailByPT($, topWindow) {
        this.$ = $;
        this.topWindow = topWindow;
        this.fixedsortdata = [];
        this.fixedsortlist = [];
        this.hideFilter = false;
        this.dateTimeControls = new DateTimeControls($);
        this.productControls = new ProductControls($);
    }
    SuperWinLossDetailByPT.prototype.Init = function () {
        var _this = this;
        this.dateTimeControls.Init();
        this.productControls.Init();
        this.$(".icon-filter").click(this.ShowHideFilter);
        this.$("#userGroup input").attr("readonly", "true");
        $("#userGroup input").bind("click", function () {
            $(this).autocomplete("search", "");
        });
        this.$("#Submit").click(function () { _this.CallSubmit(); });
        this.$("#exporttoexcel").click(function () { _this.ExportExcel(); });
        this.$(".col-sorter").click(function (event) { _this.sortTable(_this.$(event.currentTarget)); });
        Fanex.ExtAcc.OnResize = this.ResizeReportGrid;
    };
    SuperWinLossDetailByPT.prototype.BuildDefaultColumns = function () {
        return [
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
    };
    SuperWinLossDetailByPT.prototype.BuildDefaultHeaderColumns = function () {
        return [
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
    };
    SuperWinLossDetailByPT.prototype.BuildSubRowData = function (winLossByProductModel) {
        var _this = this;
        return {
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
        };
    };
    SuperWinLossDetailByPT.prototype.BuildBodyData = function (dataSource) {
        var bodyData = [];
        for (var i = 0; i < dataSource.length; i++) {
            var subRows = [];
            for (var j = 0; j < dataSource[i].WinLossByProducts.length; j++) {
                subRows.push(this.BuildSubRowData(dataSource[i].WinLossByProducts[j]));
            }
            var totalRow = dataSource[i].Total;
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
                SubRows: subRows,
                CssClass: 'sub-row-class'
            });
        }
        return bodyData;
    };
    SuperWinLossDetailByPT.prototype.BuildFooterData = function (grandTotal) {
        var footerData = [];
        footerData.push({
            Columns: [
                { Name: "Username", Value: "Grand Total" },
                { Name: "Turnover", Value: grandTotal.Turnover },
                { Name: "NetTurnover", Value: grandTotal.NetTurnover },
                { Name: "GrossComm", Value: grandTotal.GrossComm },
                { Name: "MemberWinlost", Value: grandTotal.MemberWinlost },
                { Name: "PlayerComm", Value: grandTotal.MemberComm },
                { Name: "MemberTotal", Value: grandTotal.MemberTotal },
                { Name: "AgWinlost", Value: grandTotal.AgWinlost },
                { Name: "AgComm", Value: grandTotal.AgComm },
                { Name: "AgTotal", Value: grandTotal.AgTotal },
                { Name: "MAgWinlost", Value: grandTotal.MAgWinlost },
                { Name: "MAgComm", Value: grandTotal.MAgComm },
                { Name: "MAgTotal", Value: grandTotal.MAgTotal },
                { Name: "SAgWinlost", Value: grandTotal.SAgWinlost },
                { Name: "SAgComm", Value: grandTotal.SAgComm },
                { Name: "SAgTotal", Value: grandTotal.SAgTotal },
                { Name: "Company", Value: grandTotal.Company }
            ]
        });
        return footerData;
    };
    SuperWinLossDetailByPT.prototype.RenderPositonTakingColumn = function (cell) {
        var tooltip = '';
        this.$.each(cell.Value.ProductNames, function (index, value) {
            tooltip += "<div>" + value + "</div>";
        });
        return "<span class=\"product-tooltip\" title=\"" + tooltip + "\">" + cell.Value.PositionTakingText + "<a>('" + cell.Value.ProductNames.length + ")</a></span>";
    };
    SuperWinLossDetailByPT.prototype.Render = function (data, dateTimeParams) {
        var _this = this;
        var dateInfo = dateTimeParams.fromdate === dateTimeParams.todate ?
            "on " + dateTimeParams.fromdate :
            "from " + dateTimeParams.fromdate + " to " + dateTimeParams.todate;
        var gridOption = {
            bodyRows: this.BuildBodyData(data.WinLossByUsernames),
            footerRows: this.BuildFooterData(data.GrandTotal),
            customizeHeader: true,
            groupHeaderRows: [{ Columns: this.BuildDefaultHeaderColumns() }],
            columns: this.BuildDefaultColumns(),
            numberOfFrozenColumn: this.topWindow.$(top.window).width() < 768 ? 1 : 3,
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
                currentPage: data.PageIndex - 1,
                totalItem: data.TotalRecords,
                itemsPerPage: data.PageSize,
                pagerSelectCallBack: function (currentPageIndex, itemsPerPage) {
                    _this.CallSubmit(itemsPerPage, currentPageIndex + 1);
                },
                showDetail: true
            },
            marginRight: 10,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            sortOption: {
                afterGridSorted: function (rows) { return _this.Customsortfunc(rows); }
            },
            numOfPaddingForGridExpandHeight: 40
        };
        this.InitSort(data.WinLossByUsernames);
        setPagerOptionOnMobile(gridOption);
        this.$("#jqgrid").fanexGrid(gridOption);
        this.$(".product-tooltip").tooltip();
        this.$("#netTurnover").unbind().bind("click", function () { return _this.ShowNetTurnoverInfo(data); });
        workaroundForGridOnPhone();
    };
    SuperWinLossDetailByPT.prototype.CustomUsernameSortFunction = function (cell1Value, cell2Value, sortDirection) {
        return sortDirection === 'asc' ?
            cell1Value.Username > cell2Value.Username ? 1 : -1 :
            cell1Value.Username < cell2Value.Username ? 1 : -1;
    };
    SuperWinLossDetailByPT.prototype.Customsortfunc = function (rows) {
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
    SuperWinLossDetailByPT.prototype.InitSort = function (dataSource) {
        var countFixedSort = this.fixedsortlist.length;
        if (countFixedSort > 0) {
            for (var i = 0; i < countFixedSort; i++) {
                for (var j = 0; j < dataSource.length; j++) {
                    if (this.fixedsortlist[i] === dataSource[j].CustId) {
                        this.fixedsortdata.push(dataSource.slice(j, j + 1)[0]);
                        dataSource.unshift(dataSource.splice(j, 1)[0]);
                        break;
                    }
                }
            }
        }
    };
    SuperWinLossDetailByPT.prototype.ShowNetTurnoverInfo = function (data) {
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
        Fanex.ExtAcc.BlockUI();
        var dateTimeParams = this.dateTimeControls.GetFromDateToDate();
        var parameterUrl = "?Groupname=" + $("#selectedGroupName").html()
            + "&Fromdate=" + dateTimeParams.fromdate
            + "&Todate=" + dateTimeParams.todate
            + "&PageSize=" + pageSize
            + "&PageIndex=" + pageIndex
            + this.productControls.GetParameterProducts()
            + this.productControls.GetParameterProductDetails();
        this.$.ajax({
            url: resolveClientUrl("SuperWinLossDetailByPT/GetData") + parameterUrl,
            type: "post",
            dataType: "json",
            cache: false,
            success: function (data) { return _this.OnSuccess(data, dateTimeParams); }
        });
    };
    SuperWinLossDetailByPT.prototype.OnSuccess = function (data, dateTimeParams) {
        if (data.errorCode) {
            Fanex.ExtAcc.UnBlockUI();
            Fanex.ExtAcc.Form.Alert("Error", data.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
            return;
        }
        var existedData = data.WinLossByUsernames !== null && data.WinLossByUsernames.length > 0;
        this.$("#mainGrid").css("display", existedData ? "block" : "none");
        this.$("#mainGrid").css("visibility", existedData ? "visible" : "hidden");
        this.$("#exporttoexcel").css("display", existedData ? "block" : "none");
        this.$("#msgNoinfo").css("display", existedData ? "none" : "block");
        if (existedData) {
            this.Render(data, dateTimeParams);
            this.$("#exporttoexcel").removeClass("hide");
        }
        else {
            this.$("#exporttoexcel").addClass("hide");
        }
        Fanex.ExtAcc.UnBlockUI();
    };
    SuperWinLossDetailByPT.prototype.ExportExcel = function () {
        var dateTimeParams = this.dateTimeControls.GetFromDateToDate();
        var params = "fromdate=" + dateTimeParams.fromdate +
            "&todate=" + dateTimeParams.todate +
            "&groupname=" + $("#selectedGroupName").html();
        var url = resolveClientUrl("SuperWinLossDetailByPT/ExportWinLossDetailToExcel?") + params +
            this.productControls.GetParameterProducts() +
            this.productControls.GetParameterProductDetails();
        this.topWindow.ExportExcel(url, document);
    };
    SuperWinLossDetailByPT.prototype.ResizeReportGrid = function () {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        }
        catch (err) {
            console.log(err);
        }
    };
    SuperWinLossDetailByPT.prototype.ShowHideFilter = function () {
        if (this.hideFilter) {
            $(".icon-filter").addClass("active");
            $(".filterbox").removeClass("hide");
        }
        else {
            $(".icon-filter").removeClass("active");
            $(".filterbox").addClass("hide");
        }
        this.ResizeReportGrid();
        this.hideFilter = !this.hideFilter;
    };
    return SuperWinLossDetailByPT;
}());
$(document).ready(function () {
    var superWinLossDetailByPT = new SuperWinLossDetailByPT($, window.top);
    superWinLossDetailByPT.Init();
});
//# sourceMappingURL=SuperWinLossDetailByPT.js.map