declare var Fanex: any;
declare var workaroundForGridOnPhone: any;
declare var setPagerOptionOnMobile: any;
declare var isMobile: any;

class SuperWinLossDetailByPT {
    private $: JQueryStatic;
    private gridOption: any;
    private fixedsortdata: any;
    private fixedsortlist: any;
    private dataSource: any;
    private hideFilter: boolean;
    private dateTimeControls: DateTimeControls;
    private productControls: ProductControls;
    private totalRows: number;
    private sorters: any;
    private sortTable: any;
    private topWindow: any;

    constructor($: JQueryStatic, topWindow: any) {
        this.$ = $;
        this.topWindow = topWindow;

        this.gridOption = {};
        this.fixedsortdata= new Array();
        this.fixedsortlist= new Array();
        this.dataSource= [];
        this.hideFilter= false;
        this.dateTimeControls = new DateTimeControls($);
        this.productControls = new ProductControls($);
    }

    Init() {
        this.dateTimeControls.Init();

        this.$("#userGroup input").attr("readonly", "true");
        $("#userGroup input").bind("click", function () {
            $(this).autocomplete("search", "");
        });

        this.productControls.ProductsList($("#selectedGroupName").html());

        this.$("#Submit").click(() => { this.CallSubmit(); });

        this.$("#exporttoexcel").click(() => { this.exportExcel(); });

        this.sorters = this.$(".col-sorter");
        this.sorters.click(event => { this.sortTable(this.$(event.currentTarget));});

        Fanex.ExtAcc.OnResize = this.resizeReportGrid;
        this.$(".icon-filter").click(this.showHideFilter);
    }

    exportExcel() {
        const dateTimeParams = this.dateTimeControls.GetFromDateToDate();
        const params = "fromdate=" + dateTimeParams.fromdate +
            "&todate=" + dateTimeParams.todate +
            "&groupname=" + $("#selectedGroupName").html();

        const url = resolveClientUrl("SuperWinLossDetailByPT/ExportWinLossDetailToExcel?") + params +
            this.productControls.getParameterProducts() +
            this.productControls.getParameterProductDetails();

        this.topWindow.exportExcel(url, document);
    }

    resizeReportGrid() {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        }
        catch (err) {
            console.log(err);
        }
    }

    showHideFilter() {
        if (this.hideFilter) {
            $(".icon-filter").addClass("active");

            $(".filterbox").removeClass("hide");
        } else {
            $(".icon-filter").removeClass("active");
            $(".filterbox").addClass("hide");
        }

        this.resizeReportGrid();
        this.hideFilter = !this.hideFilter;
    }

    CustomUsernameSortFunction(cell1Value, cell2Value, sortDirection) {
        return sortDirection === 'asc' ?
            cell1Value.Username > cell2Value.Username ? 1 : -1 :
            cell1Value.Username < cell2Value.Username ? 1 : -1;
    }

    buildDefaultColumns() {
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
    }

    buildDefaultHeaderColumns() {
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
    }

    buildSubRowData(winLossByProductModel) {
        return {
            Columns: [
                {
                    HeaderText: "Username", Name: "Username",
                    Value: winLossByProductModel,
                    CustomCellRender: cell => { return this.RenderPositonTakingColumn(cell); }
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
    }

    buildBodyData(dataSource) {
        let bodyData = [];
        for (let i = 0; i < dataSource.length ; i++) {
            let subRows = [];
            for (let j = 0; j < dataSource[i].WinLossByProducts.length; j++) {
                subRows.push(this.buildSubRowData(dataSource[i].WinLossByProducts[j]));
            }

            let totalRow = this.dataSource[i].Total;
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
    }

    buildFooterData(grandTotal) {
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

        return `<span class="product-tooltip" title="${tooltip}">${cell.Value.PositionTakingText}<a>('${cell.Value.ProductNames.length})</a></span>`;
    }

    Render(data, dateTimeParams) {
        const dateInfo = dateTimeParams.fromdate === dateTimeParams.todate ?
            `on ${dateTimeParams.fromdate}` :
            `from ${dateTimeParams.fromdate} to ${dateTimeParams.todate}`;

        this.gridOption = {
            bodyRows: this.buildBodyData(data.WinLossByUsernames),
            footerRows: this.buildFooterData(data.GrandTotal),
            customizeHeader: true,
            groupHeaderRows: [{ Columns: this.buildDefaultHeaderColumns() }],
            columns: this.buildDefaultColumns(),
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
                pagerSelectCallBack: (currentPageIndex, itemsPerPage) => {
                    this.CallSubmit(itemsPerPage, currentPageIndex + 1);
                },
                showDetail: true
            },
            marginRight: 10,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            sortOption: {
                afterGridSorted: rows => { return this.customsortfunc(rows); }
            },
            numOfPaddingForGridExpandHeight: 40
        };

        this.InitSort();

        setPagerOptionOnMobile(this.gridOption);
        this.$("#jqgrid").fanexGrid(this.gridOption);
        this.$(".product-tooltip").tooltip();

        workaroundForGridOnPhone();
    }

    customsortfunc(rows) {
        const countFixedSort = this.fixedsortlist.length;
        if (countFixedSort > 0) {
            for (let i = 0; i < countFixedSort; i++) {
                for (let j = 0; j < rows.length; j++) {
                    if (this.fixedsortlist[i] === rows[j].Columns[0].Value.CustId) {
                        this.fixedsortdata.push(rows.slice(j, j + 1)[0]);
                        rows.unshift(rows.splice(j, 1)[0]);
                        break;
                    }
                }
            }
        }
        return rows;
    }

    InitSort() {
        let countFixedSort = this.fixedsortlist.length;
        if (countFixedSort > 0) {
            for (let i = 0; i < countFixedSort; i++) {
                for (let j = 0; j < this.dataSource.length; j++) {
                    if (this.fixedsortlist[i] === this.dataSource[j].CustId) {
                        this.fixedsortdata.push(this.dataSource.slice(j, j + 1)[0]);
                        this.dataSource.unshift(this.dataSource.splice(j, 1)[0]);
                        break;
                    }
                }
            }
        }
    }

    showNetTurnoverInfo(data: any) {
        let message = "- For Sportbook, Net Turnover is Turnover which excludes the draw tickets and halves of the turnover of the half won, half lost tickets.";
        let height = 170;

        if (data.TooltipsForLiveCasino !== "") {
            message +=
                "<br/>" +
                "<br/>" +
                data.TooltipsForLiveCasino;
            height += 50;
        }

        Fanex.ExtAcc.Form.Alert("Net Turnover Information", message, 430, height);
    }

    CallSubmit(pageSize = 50, pageIndex = 1) {
        Fanex.ExtAcc.BlockUI();

        const dateTimeParams = this.dateTimeControls.GetFromDateToDate();

        const parameterUrl = "?Groupname=" + $("#selectedGroupName").html()
            + "&Fromdate=" + dateTimeParams.fromdate
            + "&Todate=" + dateTimeParams.todate
            + "&PageSize=" + pageSize
            + "&PageIndex=" + pageIndex
            + this.productControls.getParameterProducts()
            + this.productControls.getParameterProductDetails();

        this.$.ajax({
            url: resolveClientUrl("SuperWinLossDetailByPT/GetData") + parameterUrl,
            type: "post",
            dataType: "json",
            cache: false,
            success: data => {
                if (data.errorCode) {
                    Fanex.ExtAcc.UnBlockUI();
                    Fanex.ExtAcc.Form.Alert("Error", data.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
                    return;
                }

                this.fixedsortdata = new Array();

                this.dataSource = data.WinLossByUsernames;
                this.totalRows = data.WinLossByUsernames !== null ? data.WinLossByUsernames.length : 0;
                this.fixedsortlist = [];

                this.$("#mainGrid").css("display", this.totalRows < 1 ? "none" : "block");
                this.$("#mainGrid").css("visibility", this.totalRows < 1 ? "hidden" : "visible");
                this.$("#exporttoexcel").css("display", this.totalRows < 1 ? "none" : "block");
                this.$("#msgNoinfo").css("display", this.totalRows < 1 ? "block" : "none");

                if (this.totalRows > 0) {
                    this.Render(data, dateTimeParams);
                    this.$("#exporttoexcel").removeClass("hide");
                } else {
                    this.$("#exporttoexcel").addClass("hide");
                }

                $("#netTurnover").unbind().bind("click", () => this.showNetTurnoverInfo(data));

                Fanex.ExtAcc.UnBlockUI();
            }
        });
    }
}

$(document).ready(function () {
    var superWinLossDetailByPT = new SuperWinLossDetailByPT($, window.top);
    superWinLossDetailByPT.Init();
});