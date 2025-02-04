declare var Fanex: any;
declare var workaroundForGridOnPhone: any;
declare var setPagerOptionOnMobile: any;
declare var isMobile: any;

class SuperWinLossMCDetailByPT {
    private $: JQueryStatic;
    private productControls: ProductControls;
    private $multiSelectCurrency: any;
    private dateRangePickerComponent: DateRangePickerJQueryComponent;

    private fixedsortdata: any;
    private fixedsortlist: any;
    private hideFilter: boolean;
    private fromdate = moment(new Date()).format("MM/DD/YYYY");
    private todate = moment(new Date()).format("MM/DD/YYYY");
    private cachedProductAndCurParams = "";
    private totalRows = 0;

    private sortTable: any;
    private topWindow: any;

    constructor($: JQueryStatic, topWindow: any, dateRangePickerComponent: DateRangePickerJQueryComponent) {
        this.$ = $;
        this.topWindow = topWindow;

        this.fixedsortdata = [];
        this.fixedsortlist = [];
        this.hideFilter = false;
        this.productControls = new ProductControls($);
        this.dateRangePickerComponent = dateRangePickerComponent;
    }

    Init() {
        this.dateRangePickerComponent.render(new DateRangePickerJQueryOption());
        this.productControls.Init();
        this.InitCurrencyList();
        $("#ddl-base-currency").combobox();
        $("#base-currency input").attr('readonly', 'true').val(($("#ddl-base-currency")[0] as HTMLSelectElement).options[0].text).bind("click", function () {
            $(this).autocomplete("search", "");
        });

        $("#ckbBaseCurrency").change(() => {
            $("#base-currency").find("input, button").prop("disabled", !$("#ckbBaseCurrency").is(":checked"));
        });

        this.$(".icon-filter").click(() => this.ShowHideFilter());

        this.$("#userGroup input").attr("readonly", "true");
        this.$("#userGroup input").bind("click", event => this.$(event.currentTarget).autocomplete("search", ""));

        this.$("#Submit").click(() => this.CallSubmit());

        $("#notes").click(() => this.ShowInfo());

        this.$(".col-sorter").click(event => this.sortTable(this.$(event.currentTarget)));

        Fanex.ExtAcc.OnResize = this.ResizeReportGrid;
    }

    InitCurrencyList() {
        $.ajax({
            context: this,
            url: resolveClientUrl("Filter/CurrencyList"),
            type: "GET",
            data: { groupName: $("#selectedGroupName").html() },
            dataType: "json",
            cache: false,
            success: (result) => {
                var dataList = [];
                $.each(result, function (i, currency) {
                    dataList.push({
                        id: "rpt_cur_" + currency.CurrencyId,
                        value: currency.CurrencyId,
                        text: currency.CurrencyName,
                        checked: true,
                        enable: true
                    });
                });
                this.$multiSelectCurrency = $("#ddl-currency").fanexMultiSelect({
                    minWidth: 140,
                    checkAllText: "Select all",
                    noneSelectedText: "Select Currencies",
                    useCustomCheckAllText: true,
                    customCheckAllText: "All Currencies",
                    isCheckAll: true,
                    listData: dataList
                });
            }
        });
    }

    BuildDefaultColumns() {
        return [
            { HeaderText: "Username", Name: "Username", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "left", ShowCollapseExpand: true },
            { HeaderText: "Currency Name", Name: "Currency", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "left"},
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
            { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
            { HeaderText: "Currency Name", Name: "Currency", MergeRowNumber: 2 },
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
                    CustomCellRender: cell => { return this.RenderPositonTakingColumn(cell); },
                    MergeColNumber: 2
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
        this.totalRows = dataSource.length;
        for (let i = 0; i < dataSource.length; i++) {
            let subRows = [];
            for (let j = 0; j < dataSource[i].WinLossByProducts.length; j++) {
                subRows.push(this.BuildSubRowData(dataSource[i].WinLossByProducts[j]));
            }

            let totalRow = dataSource[i].Total;
            bodyData.push({
                Columns: [
                    { Name: "Username", Value: totalRow.Username },
                    { Name: "Currency", Value: totalRow.CurrencyName, CellAlign: "center" },
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

    BuildFooterData(dataSource) {
        let footerData = [];
        for (let i = 0; i < dataSource.length; i++) {
            let grandTotal = dataSource[i];
            footerData.push({
                Columns: [
                    { Name: "Username", Value: grandTotal.Username, MergeColNumber: 2, CellAlign: "right" },
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
        }

        return footerData;
    }

    RenderPositonTakingColumn(cell) {
        let tooltip = '';

        this.$.each(cell.Value.ProductNames, (index, value) => {
            tooltip += `<div>${value}</div>`;
        });

        return `<span class="product-tooltip" title="${tooltip}">${cell.Value.PositionTakingText}<a> (${cell.Value.ProductNames.length})</a></span>`;
    }

    Render(data) {
        const dateInfo = this.fromdate === this.todate ?
            `on ${moment(this.fromdate).format('MMM DD, yyyy')}` :
            `from ${moment(this.fromdate).format('MMM DD, yyyy')} to ${moment(this.todate).format('MMM DD, yyyy')}`;

        var currencyInfo = "";
        if ($("#ckbBaseCurrency").prop("checked")) {
            currencyInfo = " in " + $("#ddl-base-currency option:selected").text();
        } else {
            currencyInfo = " in customer currency";
        }

        const gridOption = {
            bodyRows: this.BuildBodyData(data.WinLossByUsernames),
            footerRows: this.BuildFooterData(data.TotalByCurrencies),
            customizeHeader: true,
            groupHeaderRows: [{ Columns: this.BuildDefaultHeaderColumns() }],
            columns: this.BuildDefaultColumns(),
            numberOfFrozenColumn: this.topWindow.$(top.window).width() < 768 ? 1 : 4,
            autoCalculateGridExpandHeight: true,
            fullHeight: !isMobile.phone,
            gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
            showTitle: true,
            titleInfo: {
                text: "Super Win Loss Detail By PT " + dateInfo + currencyInfo,
                customButtons: '<span id="exporttoexcelByCur" title="Export Excel" class="icon-excel"></span>' },
            showFooter: true,
            showTree: true,
            gridExpandHeight: 256,
            showPager: true,
            pagerOption: {
                itemsPerPageArray: [50, 100, 500],
                currentPage: 0,
                totalItem: this.totalRows,
                itemsPerPage: 50
            },
            marginRight: 10,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            sortOption: {
                afterGridSorted: rows => { return this.Customsortfunc(rows); }
            },
            numOfPaddingForGridExpandHeight: 40
        };

        this.InitSort(data.WinLossByUsernames);
        setPagerOptionOnMobile(gridOption);

        this.$("#jqgrid").fanexGrid(gridOption);
        this.$(".product-tooltip").tooltip();
        this.$("#netTurnover").unbind().bind("click", () => this.ShowNetTurnoverInfo(data));
        $("#exporttoexcelByCur").click(() => this.ExportExcel());
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

    GetParameterCurrency() {
        var currencyNames = this.$multiSelectCurrency.fanexMultiSelect("getSelectedTexts", true);
        return currencyNames.length ? "&currencies=" + currencyNames.join(",") : "";
    }

    GetParameterBaseCurrency() {
        if ($("#ckbBaseCurrency").prop("checked")) {
            return "&baseCurrency=" + $("#ddl-base-currency").val() + "&baseCurrencyName=" + $("#ddl-base-currency option:selected").text();
        } else {
            return "";
        }
    }

    CallSubmit() {
        const dateRange = this.dateRangePickerComponent.getDateRangeValues();
        this.fromdate = dateRange.fromDate;
        this.todate = dateRange.toDate;

        var selectedCurrency = this.GetParameterCurrency();
        if (selectedCurrency === '') {
            this.ShowNoData();
            return;
        }

        this.cachedProductAndCurParams = this.productControls.GetParameterProducts()
            + this.productControls.GetParameterProductDetails()
            + selectedCurrency
            + this.GetParameterBaseCurrency();

        const parameterUrl = `?Groupname=${$("#selectedGroupName").html()}&GroupId=${Fanex.ExtAcc.Customer.GetGroupId() || ''}`
            + `&Fromdate=${this.fromdate}&Todate=${this.todate}`
            + this.cachedProductAndCurParams;

        Fanex.ExtAcc.BlockUI();
        this.$.ajax({
            url: resolveClientUrl("SuperWinLossDetailMCByPT/GetData") + parameterUrl,
            type: "post",
            dataType: "json",
            cache: false,
            success: data => this.OnSuccess(data)
        });
    }

    OnSuccess(data) {
        if (data.errorCode) {
            Fanex.ExtAcc.UnBlockUI();
            Fanex.ExtAcc.Form.Alert("Error", data.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
            return;
        }

        const existedData = data.WinLossByUsernames !== null && data.WinLossByUsernames.length > 0;


        if (existedData) {
            this.$("#mainGrid").css("display", "block");
            this.$("#mainGrid").css("visibility", "visible");
            this.$("#msgNoinfo").css("display", "none");
            this.Render(data);
        } else {
            this.ShowNoData();
        }

        Fanex.ExtAcc.UnBlockUI();
    }

    ShowNoData() {
        this.$("#mainGrid").css("display", "none");
        this.$("#mainGrid").css("visibility", "hidden");
        this.$("#msgNoinfo").css("display", "block");
    }

    ExportExcel() {
        const params = `fromdate=${this.fromdate}&todate=${this.todate}&groupname=${this.$("#selectedGroupName").html()}&groupId=${Fanex.ExtAcc.Customer.GetGroupId() || ''}`,
        url = `${resolveClientUrl("SuperWinLossDetailMCByPT/ExportWinLossDetailToExcel?")}${params}${this.cachedProductAndCurParams}`;

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

    ShowInfo() {
        var message = $("#notes").data("info");
        Fanex.ExtAcc.Form.Alert('Note', message, 500, 220, null, null, 0);
    }
}

$(document).ready(function () {
    const superWinLossMCDetailByPT = new SuperWinLossMCDetailByPT($, window.top, new DateRangePickerJQueryComponent());
    superWinLossMCDetailByPT.Init();
});