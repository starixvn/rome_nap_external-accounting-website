declare var Fanex: any;
declare var workaroundForGridOnPhone: any;
declare var setPagerOptionOnMobile: any;
declare var isMobile: any;

class SuperWinLossDetailByPT {
    private $: JQueryStatic;
    private dateTimeControls: DateTimeControls;
    private productControls: ProductControls;

    private fixedsortdata: any;
    private fixedsortlist: any;
    private hideFilter: boolean;

    private sortTable: any;
    private topWindow: any;

    constructor($: JQueryStatic, topWindow: any) {
        this.$ = $;
        this.topWindow = topWindow;

        this.fixedsortdata = [];
        this.fixedsortlist = [];
        this.hideFilter = false;
        this.dateTimeControls = new DateTimeControls($);
        this.productControls = new ProductControls($);
    }

    Init() {
        this.dateTimeControls.Init();
        this.productControls.Init();

        this.$(".icon-filter").click(() => this.ShowHideFilter());

        this.$("#userGroup input").attr("readonly", "true");
        this.$("#userGroup input").bind("click", event => this.$(event.currentTarget).autocomplete("search", ""));

        this.$("#Submit").click(() => this.CallSubmit());

        this.$("#exporttoexcel").click(() => this.ExportExcel());

        this.$(".col-sorter").click(event => this.sortTable(this.$(event.currentTarget)));

        Fanex.ExtAcc.OnResize = this.ResizeReportGrid;
    }

    BuildDefaultColumns() {
        return [
            { HeaderText: "Username", Name: "Username", Width: 180, Sort: true, HeaderAlign: "center", CellAlign: "left", ShowCollapseExpand: true },
            { HeaderText: "Turnover", Name: "Turnover", Width: 130, Sort: true, SortDirection: 'desc', HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
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
    }

    BuildDefaultHeaderColumns() {
        return [
            { HeaderText: "Username / PT / Product", Name: "Username", MergeRowNumber: 2 },
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

    BuildSubRowData(data) {
        return {
            Columns: [
                {
                    HeaderText: "Username", Name: "Username",
                    Value: data,
                    CustomCellRender: cell => { return this.RenderPositonTakingColumn(cell); }
                },
                { Name: "Turnover", Value: data.Turnover },
                { Name: "NetTurnover", Value: data.NetTurnover },
                { Name: "GrossComm", Value: data.GrossComm },
                { Name: "MemberWinlost", Value: data.MemberWinlost },
                { Name: "PlayerComm", Value: data.MemberComm },
                { Name: "MemberTotal", Value: data.MemberTotal },
                { Name: "AgWinlost", Value: data.AgWinlost },
                { Name: "AgComm", Value: data.AgComm },
                { Name: "AgTotal", Value: data.AgTotal },
                { Name: "MAgWinlost", Value: data.MAgWinlost },
                { Name: "MAgComm", Value: data.MAgComm },
                { Name: "MAgTotal", Value: data.MAgTotal },
                { Name: "SAgWinlost", Value: data.SAgWinlost },
                { Name: "SAgComm", Value: data.SAgComm },
                { Name: "SAgTotal", Value: data.SAgTotal },
                { Name: "Company", Value: data.Company }
            ]
        };
    }

    BuildBodyData(dataSource) {
        let bodyData = [];
        for (let i = 0; i < dataSource.length; i++) {
            let subRows = [];
            for (let j = 0; j < dataSource[i].WinLossByProducts.length; j++) {
                subRows.push(this.BuildSubRowData(dataSource[i].WinLossByProducts[j]));
            }

            let totalRow = dataSource[i].Total;
            bodyData.push({
                Columns: [
                    { Name: "Username", Value: totalRow.Username, CustomCellRender: () =>  totalRow.CustId === 0 ? this.RenderDormantAccountsCell() : totalRow.Username },
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
    }

    BuildFooterData(grandTotal) {
        let footerData = [];
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
    }

    RenderPositonTakingColumn(cell) {
        let tooltip = '';

        this.$.each(cell.Value.ProductNames, (index, value) => {
            tooltip += `<div>${value}</div>`;
        });

        return `<span class="product-tooltip" title="${tooltip}">${cell.Value.PositionTakingText}<a> (${cell.Value.ProductNames.length})</a></span>`;
    }

    Render(data, dateTimeParams) {
        const dateInfo = dateTimeParams.fromdate === dateTimeParams.todate ?
            `on ${dateTimeParams.fromdate}` :
            `from ${dateTimeParams.fromdate} to ${dateTimeParams.todate}`;

        const gridOption = {
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
                pagerSelectCallBack: (currentPageIndex, itemsPerPage) => this.CallSubmit(itemsPerPage, currentPageIndex + 1),
                showDetail: true
            },
            marginRight: 10,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            sortOption: {
                customGridSort: (rows, sortField, sortDirection, customSortFunction, sortType) =>
                    this.CustomGridSort(rows, sortField, sortDirection, customSortFunction, sortType),
                afterGridSorted: rows => { return this.Customsortfunc(rows); }
            },
            numOfPaddingForGridExpandHeight: 40
        };

        this.InitSort(data.WinLossByUsernames);
        setPagerOptionOnMobile(gridOption);

        this.$("#jqgrid").fanexGrid(gridOption);
        this.$(".product-tooltip").tooltip();
        this.$("#netTurnover").unbind().bind("click", () => this.ShowNetTurnoverInfo(data));
        this.$("#pt-infor").unbind().bind("click", () => this.ShowPTInformation());

        workaroundForGridOnPhone();
    }

    CustomUsernameSortFunction(cell1Value, cell2Value, sortDirection) {
        return sortDirection === 'asc' ?
            cell1Value.Username > cell2Value.Username ? 1 : -1 :
            cell1Value.Username < cell2Value.Username ? 1 : -1;
    }

    Customsortfunc(rows) {
        const countFixedSort = this.fixedsortlist.length;
        if (countFixedSort > 0) {
            for (let i = 0; i < countFixedSort; i++) {
                for (let j = 0; j < rows.length; j++) {
                    if (this.fixedsortlist[i] === rows[j].Columns[0].Value.Turnover) {
                        this.fixedsortdata.push(rows.slice(j, j + 1)[0]);
                        rows.unshift(rows.splice(j, 1)[0]);
                        break;
                    }
                }
            }
        }

        this.$(".product-tooltip").tooltip();
        return rows;
    }

    InitSort(dataSource) {
        let countFixedSort = this.fixedsortlist.length;
        if (countFixedSort > 0) {
            for (let i = 0; i < countFixedSort; i++) {
                for (let j = 0; j < dataSource.length; j++) {
                    if (this.fixedsortlist[i] === dataSource[j].Turnover) {
                        this.fixedsortdata.push(dataSource.slice(j, j + 1)[0]);
                        dataSource.unshift(dataSource.splice(j, 1)[0]);
                        break;
                    }
                }
            }
        }
    }

    ShowNetTurnoverInfo(data: any) {
        let message = "- For Sportsbook, Net Turnover is Turnover which excludes the draw tickets and halves of the turnover of the half won, half lost tickets.";
        let height = 170;

        if (data.TooltipsForLiveCasino !== "") {
            message += `<br/><br/>${data.TooltipsForLiveCasino}`;
            height += 50;
        }
        if (data.TooltipsForRNGCasino !== "") {
            message += `<br/><br/>${data.TooltipsForRNGCasino}`;
            height += 70;
        }
        Fanex.ExtAcc.Form.Alert("Net Turnover Information", message, 430, height);
    }

    ShowPTInformation() {
        const message = "The PT% displayed in the report is the Super Max PT given by House. It may not match with real taken PT which is used to calculate the report's figures.";
        Fanex.ExtAcc.Form.Alert("PT% Information", message, 430, 150);
    }

    CallSubmit(pageSize = 50, pageIndex = 1) {
        Fanex.ExtAcc.BlockUI();

        const dateTimeParams = this.dateTimeControls.GetFromDateToDate();

        const parameterUrl = `?Groupname=${$("#selectedGroupName").html()}&GroupId=${Fanex.ExtAcc.Customer.GetGroupId() || ''}`
            + `&Fromdate=${dateTimeParams.fromdate}&Todate=${dateTimeParams.todate}`
            + `&PageSize=${pageSize}&PageIndex=${pageIndex}`
            + this.productControls.GetParameterProducts()
            + this.productControls.GetParameterProductDetails();

        this.$.ajax({
            url: resolveClientUrl("SuperWinLossDetailByPT/GetData") + parameterUrl,
            type: "post",
            dataType: "json",
            cache: false,
            success: data => this.OnSuccess(data, dateTimeParams)
        });
    }

    OnSuccess(data, dateTimeParams) {
        if (data.errorCode) {
            Fanex.ExtAcc.UnBlockUI();
            Fanex.ExtAcc.Form.Alert("Error", data.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
            return;
        }

        const existedData = data.WinLossByUsernames !== null && data.WinLossByUsernames.length > 0;

        this.$("#mainGrid").css("display", existedData ? "block" : "none");
        this.$("#mainGrid").css("visibility", existedData ? "visible" : "hidden");
        this.$("#msgNoinfo").css("display", existedData ? "none" : "block");

        if (existedData) {
            this.Render(data, dateTimeParams);
            this.$("#exporttoexcel").removeClass("hide");
            this.$("#pt-infor").removeClass("hide");
        } else {
            this.$("#exporttoexcel").addClass("hide");
            this.$("#pt-infor").addClass("hide");
        }

        Fanex.ExtAcc.UnBlockUI();
    }

    ExportExcel() {
        const dateTimeParams = this.dateTimeControls.GetFromDateToDate(),
            params = `fromdate=${dateTimeParams.fromdate}&todate=${dateTimeParams.todate}&groupname=${this.$("#selectedGroupName").html()}&groupId=${Fanex.ExtAcc.Customer.GetGroupId() || ''}`,
            productParams = this.productControls.GetParameterProducts(),
            productDetailsParams = this.productControls.GetParameterProductDetails(),
            url = `${resolveClientUrl("SuperWinLossDetailByPT/ExportWinLossDetailToExcel?")}${params}${productParams}${productDetailsParams}`;

        this.topWindow.exportExcel(url, document);
    }

    ResizeReportGrid() {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        }
        catch (err) {
            console.log(err);
        }
    }

    ShowHideFilter() {
        if (this.hideFilter) {
            $(".icon-filter").addClass("active");
            $(".filterbox").removeClass("hide");
        } else {
            $(".icon-filter").removeClass("active");
            $(".filterbox").addClass("hide");
        }

        this.ResizeReportGrid();
        this.hideFilter = !this.hideFilter;
    }

    CustomGridSort(rows, sortField, sortDirection, customSortFunction, sortType) {
        if (!rows.length || !sortField) {
            return rows;
        }

        var sortColIndex = this.GetSortedColumnIndex(rows, sortField);
        if (sortColIndex === -1) {
            return rows;
        }

        rows.sort(function (row1, row2) {
            if (row1.Columns[0].Value === null) { // 'Dormant Accounts'
                return 1;
            }

            var num1 = row1.Columns[sortColIndex].Value;
            var num2 = row2.Columns[sortColIndex].Value;

            if (sortDirection === 'asc') {
                return num1 > num2 ? 1 : -1;
            }

            return num1 < num2 ? 1 : -1;
        });

        return rows;
    }

    GetSortedColumnIndex(rows, sortField) {
        var firstRowColumns = rows[0].Columns;

        for (var i = 0, max = firstRowColumns.length; i < max; i++) {
            if (firstRowColumns[i].Name === sortField) {
                return i;
            }
        }

        return -1;
    }

    RenderDormantAccountsCell() {
        return '<span>Dormant Accounts</span><span class="icon-alert-circle-outline product-tooltip" style="padding-left:3px" title="Summary of dormant accounts."></span>';
    }
}

$(document).ready(function () {
    const superWinLossDetailByPT = new SuperWinLossDetailByPT($, window.top);
    superWinLossDetailByPT.Init();
});