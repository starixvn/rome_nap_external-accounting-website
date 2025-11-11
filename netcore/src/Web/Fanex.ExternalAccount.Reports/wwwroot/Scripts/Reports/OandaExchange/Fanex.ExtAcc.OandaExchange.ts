declare var Fanex: any;

class OandaExchange {
    private $: JQueryStatic;
    private dateRangePickerComponent: DateRangePickerJQueryComponent;
    private winLossDetailCopyHtml: WinLossDetailCopyHtml;
    
    private fromdate = moment(new Date()).format("MM/DD/YYYY");
    private todate = moment(new Date()).format("MM/DD/YYYY");
    private totalRows = 0;
    private topWindow: any;

    constructor($: JQueryStatic, topWindow: any,
        dateRangePickerComponent: DateRangePickerJQueryComponent,
        winLossDetailCopyHtml: WinLossDetailCopyHtml) {
        this.$ = $;
        this.topWindow = topWindow;
        this.dateRangePickerComponent = dateRangePickerComponent;
        this.winLossDetailCopyHtml = winLossDetailCopyHtml;
    }

    Init() {
        this.dateRangePickerComponent.render(new DateRangePickerJQueryOption());
        
        this.$("#Submit").click(() => this.CallSubmit());
        this.winLossDetailCopyHtml.init("tblContent_jqgrid");

        Fanex.ExtAcc.OnResize = this.ResizeReportGrid;
    }

    BuildDefaultColumns() {
        return [
            { HeaderText: "Date", Name: "Date", Width: 130, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Currency Name", Name: "Currency", Width: 130, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Currency Code", Name: "CurrencyCode", Width: 130, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "MYR/Unit", Name: "MYRUnit", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNegative: "$" },
            { HeaderText: "CNY/Unit", Name: "CNYUnit", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNegative: "$" },
            { HeaderText: "THB/Unit", Name: "THBUnit", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNegative: "$" },
            { HeaderText: "IDR/Unit", Name: "IDRUnit", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNegative: "$" }
        ];
    }

    BuildBodyData(dataSource) {
        let bodyData = [];
        this.totalRows = dataSource.length;
        for (let i = 0; i < dataSource.length; i++) {
            var exchangeByDate = dataSource[i];
            bodyData.push({
                Columns: [
                    { Name: "Date", Value: exchangeByDate.ExchangeDateString, MergeRowNumber: exchangeByDate.Rates.length },
                    { Name: "Currency", Value: exchangeByDate.Rates[0].CurrencyName },
                    { Name: "CurrencyCode", Value: exchangeByDate.Rates[0].CurrencyCode },
                    { Name: "MYRUnit", Value: parseFloat(exchangeByDate.Rates[0].MYRRate) },
                    { Name: "CNYUnit", Value: parseFloat(exchangeByDate.Rates[0].CNYRate) },
                    { Name: "THBUnit", Value: parseFloat(exchangeByDate.Rates[0].THBRate) },
                    { Name: "IDRUnit", Value: parseFloat(exchangeByDate.Rates[0].IDRRate) }
                ]
            });

            for (var j = 1; j < exchangeByDate.Rates.length; j++) {
                var rate = exchangeByDate.Rates[j];
                bodyData.push({
                    Columns: [
                        { Name: "Currency", Value: rate.CurrencyName },
                        { Name: "CurrencyCode", Value: rate.CurrencyCode },
                        { Name: "MYRUnit", Value: parseFloat(rate.MYRRate) },
                        { Name: "CNYUnit", Value: parseFloat(rate.CNYRate) },
                        { Name: "THBUnit", Value: parseFloat(rate.THBRate) },
                        { Name: "IDRUnit", Value: parseFloat(rate.IDRRate) }
                    ]
                });
            }
        }

        return bodyData;
    }
    
    Render(data) {
        
        const gridOption = {
            columns: this.BuildDefaultColumns(),
            bodyRows: this.BuildBodyData(data),
            fullHeight: !isMobile.phone,
            gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
            gridExpandHeight: 256,
            autoCalculateGridExpandHeight: true,
            numOfPaddingForGridExpandHeight: 40,
            marginRight: 10,
            bottomBorderWidth: 0
        };

        this.$("#jqgrid").fanexGrid(gridOption);
        workaroundForGridOnPhone();
    }
    
    CallSubmit() {
        const dateRange = this.dateRangePickerComponent.getDateRangeValues();
        this.fromdate = dateRange.fromDate;
        this.todate = dateRange.toDate;

        Fanex.ExtAcc.BlockUI();

        const parameterUrl = `?Groupname=${$("#selectedGroupName").html()}&GroupId=${Fanex.ExtAcc.Customer.GetGroupId() || ''}`
            + `&Fromdate=${this.fromdate}&Todate=${this.todate}`;

        this.$.ajax({
            url: resolveClientUrl("OandaExchange/GetExchangeRate") + parameterUrl,
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

        const existedData = data !== null && data.length > 0;


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
    
    ResizeReportGrid() {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        }
        catch (err) {
            console.log(err);
        }
    }
}

$(document).ready(function () {
    const oandaExchange = new OandaExchange($, window.top, new DateRangePickerJQueryComponent(),
        new WinLossDetailCopyHtml(Clipboard));
    oandaExchange.Init();
});