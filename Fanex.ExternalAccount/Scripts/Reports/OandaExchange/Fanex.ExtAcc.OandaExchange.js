var OandaExchange = /** @class */ (function () {
    function OandaExchange($, topWindow, dateRangePickerComponent, winLossDetailCopyHtml) {
        this.fromdate = moment(new Date()).format("MM/DD/YYYY");
        this.todate = moment(new Date()).format("MM/DD/YYYY");
        this.totalRows = 0;
        this.$ = $;
        this.topWindow = topWindow;
        this.dateRangePickerComponent = dateRangePickerComponent;
        this.winLossDetailCopyHtml = winLossDetailCopyHtml;
    }
    OandaExchange.prototype.Init = function () {
        var _this = this;
        this.dateRangePickerComponent.render(new DateRangePickerJQueryOption());
        this.$("#Submit").click(function () { return _this.CallSubmit(); });
        this.winLossDetailCopyHtml.init("tblContent_jqgrid");
        Fanex.ExtAcc.OnResize = this.ResizeReportGrid;
    };
    OandaExchange.prototype.BuildDefaultColumns = function () {
        return [
            { HeaderText: "Date", Name: "Date", Width: 130, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Currency Name", Name: "Currency", Width: 130, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Currency Code", Name: "CurrencyCode", Width: 130, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "MYR/Unit", Name: "MYRUnit", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNegative: "$" },
            { HeaderText: "CNY/Unit", Name: "CNYUnit", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNegative: "$" },
            { HeaderText: "THB/Unit", Name: "THBUnit", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNegative: "$" },
            { HeaderText: "IDR/Unit", Name: "IDRUnit", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNegative: "$" }
        ];
    };
    OandaExchange.prototype.BuildBodyData = function (dataSource) {
        var bodyData = [];
        this.totalRows = dataSource.length;
        for (var i = 0; i < dataSource.length; i++) {
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
    };
    OandaExchange.prototype.Render = function (data) {
        var gridOption = {
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
    };
    OandaExchange.prototype.CallSubmit = function () {
        var _this = this;
        var dateRange = this.dateRangePickerComponent.getDateRangeValues();
        this.fromdate = dateRange.fromDate;
        this.todate = dateRange.toDate;
        Fanex.ExtAcc.BlockUI();
        var parameterUrl = "?Groupname=" + $("#selectedGroupName").html() + "&GroupId=" + (Fanex.ExtAcc.Customer.GetGroupId() || '')
            + ("&Fromdate=" + this.fromdate + "&Todate=" + this.todate);
        this.$.ajax({
            url: resolveClientUrl("OandaExchange/GetExchangeRate") + parameterUrl,
            type: "post",
            dataType: "json",
            cache: false,
            success: function (data) { return _this.OnSuccess(data); }
        });
    };
    OandaExchange.prototype.OnSuccess = function (data) {
        if (data.errorCode) {
            Fanex.ExtAcc.UnBlockUI();
            Fanex.ExtAcc.Form.Alert("Error", data.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
            return;
        }
        var existedData = data !== null && data.length > 0;
        if (existedData) {
            this.$("#mainGrid").css("display", "block");
            this.$("#mainGrid").css("visibility", "visible");
            this.$("#msgNoinfo").css("display", "none");
            this.Render(data);
        }
        else {
            this.ShowNoData();
        }
        Fanex.ExtAcc.UnBlockUI();
    };
    OandaExchange.prototype.ShowNoData = function () {
        this.$("#mainGrid").css("display", "none");
        this.$("#mainGrid").css("visibility", "hidden");
        this.$("#msgNoinfo").css("display", "block");
    };
    OandaExchange.prototype.ResizeReportGrid = function () {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        }
        catch (err) {
            console.log(err);
        }
    };
    return OandaExchange;
}());
$(document).ready(function () {
    var oandaExchange = new OandaExchange($, window.top, new DateRangePickerJQueryComponent(), new WinLossDetailCopyHtml(Clipboard));
    oandaExchange.Init();
});
//# sourceMappingURL=Fanex.ExtAcc.OandaExchange.js.map