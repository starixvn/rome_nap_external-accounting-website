var __assign = (this && this.__assign) || Object.assign || function(t) {
    for (var s, i = 1, n = arguments.length; i < n; i++) {
        s = arguments[i];
        for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
            t[p] = s[p];
    }
    return t;
};
var MatchWinLossReportSummary = /** @class */ (function () {
    function MatchWinLossReportSummary() {
        this.typeReport = "summary";
        this.gridComponentConfig = {
            reportId: "summary",
            isMobile: isMobile,
            gridId: "summary-jqgrid",
            gridContainerId: "summary-mainGrid",
            selectColumnsId: "summary-ddl-columns",
            selectColumnsContainerId: "summary-ddl-columns-container",
            msgNoInfoId: "summary-msgNoInfo",
            excelBtnId: "summary-excel",
            pageSizeOptions: [500, 1000, 1500],
        };
        this.cachePageSize = this.gridComponentConfig.pageSizeOptions[0];
        this.cachePage = 1;
        this.sortBy = "";
        this.sortType = 0;
        this.baseCurrency = "";
        this.byPassLeagueValidate = false;
        this.paramsState = new StateManager({
            "FromDate": null,
            "ToDate": null,
            "Sport": null,
            "Leagues": null
        });
        this.datesState = new StateManager({ fromDate: null, toDate: null });
        this.sportState = new StateManager("");
        this.currencyState = new StateManager("All");
        this.filterComponents = new MatchWinLossFilter($);
        this.gridComponent = new SummaryGrid(this.gridComponentConfig);
        this.isMY = $("#inp-isMY").val().toLowerCase() === "true";
    }
    MatchWinLossReportSummary.prototype.init = function () {
        var _this = this;
        Fanex.ExtAcc.BlockUI();
        this.initDateRangePicker();
        this.initSportsDropdown(JSON.parse($("#inp-sports").val()));
        this.initLeaguesDropdown(JSON.parse($("#inp-leagues").val()));
        if (this.isMY) {
            this.initCurrencyDropDown(JSON.parse($("#inp-currencies").val()));
            this.initBaseCurrencyDropDown([
                { id: "MYR", value: "MYR", text: "MYR", selected: true },
                { id: "CNY", value: "CNY", text: "CNY", selected: false },
                { id: "THB", value: "THB", text: "THB", selected: false },
                { id: "IDR", value: "IDR", text: "IDR", selected: false }
            ]);
        }
        $("#" + this.typeReport + "-submit-btn").click(function () { return _this.handleClickSubmit(); });
        this.gridComponent.initSelectColumnsDropdown(SummaryGrid.DefaultColumns, [0, 1]);
        this.initExportExcelBtn();
        Fanex.ExtAcc.UnBlockUI();
    };
    MatchWinLossReportSummary.prototype.handleClickSubmit = function () {
        var _this = this;
        Fanex.ExtAcc.BlockUI();
        var isValid = this.validateParams();
        if (!isValid) {
            Fanex.ExtAcc.UnBlockUI();
            this.gridComponent.hideNoInfo(false);
        }
        else {
            var paramsData = this.getParams();
            paramsData.Page = 1;
            paramsData.PageSize = this.cachePageSize;
            paramsData.SortBy = "";
            paramsData.SortType = 0;
            this.paramsState.setValue(paramsData);
            $.ajax({
                url: resolveClientUrl("MatchWinLossReports/GetSummaryReport"),
                type: "post",
                dataType: "json",
                cache: false,
                data: paramsData,
                success: function (result) { return _this.submitSuccess(result); },
            });
        }
    };
    MatchWinLossReportSummary.prototype.handleClickSort = function (_a) {
        var _this = this;
        var sortBy = _a.sortBy, sortType = _a.sortType;
        Fanex.ExtAcc.BlockUI();
        var params = this.paramsState.getValue();
        params.SortBy = sortBy;
        params.SortType = sortType;
        this.paramsState.setValue(params);
        $.ajax({
            url: resolveClientUrl("MatchWinLossReports/GetSummaryReport"),
            type: "post",
            dataType: "json",
            cache: false,
            data: params,
            success: function (result) { return _this.submitSuccess(result); },
        });
    };
    MatchWinLossReportSummary.prototype.handleClickPage = function (_a) {
        var _this = this;
        var page = _a.page, pageSize = _a.pageSize;
        Fanex.ExtAcc.BlockUI();
        var params = this.paramsState.getValue();
        params.Page = page;
        params.PageSize = pageSize;
        this.paramsState.setValue(params);
        $.ajax({
            url: resolveClientUrl("MatchWinLossReports/GetSummaryReport"),
            type: "post",
            dataType: "json",
            cache: false,
            data: params,
            success: function (result) { return _this.submitSuccess(result); },
        });
    };
    MatchWinLossReportSummary.prototype.getLeagues = function (sportID, fromDate, toDate, callback) {
        var params = {
            sportId: sportID,
            fromDate: fromDate,
            toDate: toDate
        };
        $.ajax({
            url: resolveClientUrl("MatchWinLossReports/GetLeagues"),
            type: "post",
            dataType: "json",
            cache: false,
            data: params,
            success: function (leagues) {
                callback(leagues);
            }
        });
    };
    MatchWinLossReportSummary.prototype.submitSuccess = function (result) {
        if (result.errorCode) {
            Fanex.ExtAcc.UnBlockUI();
            Fanex.ExtAcc.Form.Alert("Error", result.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
            return;
        }
        try {
            if (result.Events.length || result.SystemParlayOutright.length) {
                this.gridComponent.hideNoInfo(true);
                this.renderGrid(result);
            }
            else {
                this.gridComponent.hideNoInfo(false);
            }
            Fanex.ExtAcc.UnBlockUI();
        }
        catch (e) {
            this.gridComponent.hideNoInfo(false);
            Fanex.ExtAcc.UnBlockUI();
        }
    };
    MatchWinLossReportSummary.prototype.initExportExcelBtn = function () {
        var isValid = this.validateParams();
        var _self = this;
        if (isValid) {
            $("#" + this.gridComponentConfig.excelBtnId).off().click(function () {
                var url = resolveClientUrl("MatchWinLossReports/ExportSummaryReport");
                window.top.exportExcel(url, document);
            });
        }
    };
    MatchWinLossReportSummary.prototype.renderGrid = function (data) {
        var _this = this;
        try {
            var gridOptions = this.gridComponent.getGridOptions();
            this.reportTitle();
            var totalGrandTotalSystemParlayOutright = this.gridComponent.buildFooterRows(__assign({}, data, { currency: this.isMY ? this.baseCurrency : "" }));
            if (data.Events.length > 0) {
                gridOptions.bodyRows = this.gridComponent.buildBodyRows(data);
                gridOptions.footerRows = totalGrandTotalSystemParlayOutright;
            }
            else {
                gridOptions.bodyRows = totalGrandTotalSystemParlayOutright;
                gridOptions.footerRows = [];
            }
            gridOptions.pagerOption.pagerSelectCallBack = function (pageIndex, pageSize) {
                _this.cachePage = pageIndex + 1;
                _this.cachePageSize = pageSize;
                _this.handleClickPage({
                    page: _this.cachePage,
                    pageSize: _this.cachePageSize,
                });
            };
            gridOptions.pagerOption.currentPage = this.cachePage - 1;
            gridOptions.pagerOption.itemsPerPage = this.cachePageSize;
            gridOptions.pagerOption.totalItem = data.TotalItems;
            gridOptions.sortOption.serverSortFunction = function (colname, currentDirection) {
                _this.sortType = currentDirection === 'desc' ? 1 : 0;
                _this.sortBy = colname;
                _this.handleClickSort({
                    sortBy: _this.sortBy,
                    sortType: _this.sortType
                });
            };
            setPagerOptionOnMobile(gridOptions);
            this.gridComponent.setGridOptions(gridOptions);
            this.gridComponent.fanexGrid(gridOptions);
            workaroundForGridOnPhone();
        }
        catch (e) {
            console.log(e);
        }
    };
    MatchWinLossReportSummary.prototype.reportTitle = function () {
        var params = this.paramsState.getValue();
        var fromDate = moment(params.FromDate, 'MM/DD/YYYY');
        var toDate = moment(params.ToDate, 'MM/DD/YYYY');
        var formattedFromDate = fromDate.format('MMM DD, YYYY');
        var formattedToDate = toDate.format('MMM DD, YYYY');
        var dateTitle = fromDate.isSame(toDate)
            ? "on " + formattedFromDate
            : "from " + formattedFromDate + " to " + formattedToDate;
        var title = "Match Win Loss " + dateTitle;
        if (this.isMY) {
            var currency = params.Currencies;
            var baseCurrency = params.BaseCurrency;
            if (currency) {
                title += ", Currency: " + currency;
            }
            if (baseCurrency) {
                title += ", Base Currency: " + baseCurrency;
            }
        }
        $("#" + this.typeReport + "-grid-title").text(title);
    };
    MatchWinLossReportSummary.prototype.initDateRangePicker = function () {
        var _self = this;
        this.filterComponents.renderCalendarPicker({
            selectorId: "#" + this.typeReport + "-date-range-picker",
            onClosed: function (value) {
                _self.datesState.setValue(value);
            }
        });
        this.initCalendarState();
    };
    MatchWinLossReportSummary.prototype.initCalendarState = function () {
        var _self = this;
        var initDate = this.filterComponents.getCalendarValues();
        _self.datesState.setValue(initDate);
        var updateLeagues = function (dateRange) {
            Fanex.ExtAcc.BlockUI();
            var fromDate = dateRange.fromDate, toDate = dateRange.toDate;
            _self.getLeagues(_self.sportState.getValue(), fromDate, toDate, function (leagues) {
                _self.initLeaguesDropdown(leagues);
            });
        };
        _self.datesState.subscribe(updateLeagues);
    };
    MatchWinLossReportSummary.prototype.validateParams = function () {
        var params = this.filterComponents.getAllFilterValues();
        var dropdownParams = params.dropdownParams;
        var isValidLeague = dropdownParams[1].currentValue.length > 0;
        if (!isValidLeague) {
            return this.byPassLeagueValidate;
        }
        return true;
    };
    MatchWinLossReportSummary.prototype.getParams = function () {
        var params = this.filterComponents.getAllFilterValues();
        var dropdownParams = params.dropdownParams, calendarParams = params.calendarParams;
        var obj = {
            "FromDate": calendarParams.fromDate,
            "ToDate": calendarParams.toDate,
            "Sport": dropdownParams[0].currentValue,
            "Leagues": dropdownParams[1].currentValue,
        };
        if (this.isMY) {
            var isCustomerCurrency = dropdownParams[3].currentValue === "Customer Currency";
            obj.Currencies = dropdownParams[2].currentValue === "All" ? "" : dropdownParams[2].currentValue;
            obj.BaseCurrency = isCustomerCurrency ? "" : dropdownParams[3].currentValue;
            this.baseCurrency = isCustomerCurrency ? obj["Currencies"] : obj["BaseCurrency"];
        }
        return obj;
    };
    MatchWinLossReportSummary.prototype.initSportsDropdown = function (sports) {
        if (!sports)
            return;
        var _self = this;
        var items = [];
        sports.forEach(function (e, i) {
            items.push({
                id: e.SportType,
                text: e.SportName,
                selected: i === 0,
            });
        });
        this.initSportState(sports[0].SportType);
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-sport",
            dropdownName: "ddl-sport",
            dropdownType: "select2",
            dropdownData: items,
            dropdownConfig: {
                noneSelectedText: "Select sport",
            },
            onClosed: function (sportID) {
                _self.sportState.setValue(sportID);
            }
        });
    };
    MatchWinLossReportSummary.prototype.initSportState = function (sportType) {
        var _self = this;
        _self.sportState.setValue(sportType);
        var updateLeagues = function (sportID) {
            Fanex.ExtAcc.BlockUI();
            var _a = _self.datesState.getValue(), fromDate = _a.fromDate, toDate = _a.toDate;
            _self.getLeagues(sportID, fromDate, toDate, function (leagues) {
                _self.initLeaguesDropdown(leagues);
            });
        };
        _self.sportState.subscribe(updateLeagues);
    };
    MatchWinLossReportSummary.prototype.initLeaguesDropdown = function (leagues) {
        this.byPassLeagueValidate = leagues.length === 0;
        var items = [];
        var maxLength = 0;
        leagues.forEach(function (e, i) {
            if ((e.LeagueName || "").length > maxLength) {
                maxLength = e.LeagueName.length;
            }
            items.push({
                id: e.LeagueId,
                value: e.LeagueId,
                text: e.LeagueName,
                checked: false,
                enable: true,
                items: []
            });
        });
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-leagues",
            dropdownName: "ddl-leagues",
            dropdownType: "fanexMultiSelectSearchable",
            dropdownData: items,
            dropdownConfig: {
                noneSelectedText: items.length ? "Select league" : "There is no League",
            },
            toggleWidth: isMobile.phone ? 243 : 193,
            minWidth: isMobile.phone ? 300 : 500,
        });
        Fanex.ExtAcc.UnBlockUI();
    };
    MatchWinLossReportSummary.prototype.initCurrencyDropDown = function (currencies) {
        if (!currencies)
            return;
        var _self = this;
        var items = [
            {
                id: "All",
                value: "All",
                text: "All",
                selected: false
            }
        ];
        currencies.forEach(function (e, i) {
            items.push({
                id: e.CurrencyName,
                value: e.CurrencyName,
                text: e.CurrencyName,
                selected: false
            });
        });
        this.initCurrencyState();
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-currencies",
            dropdownName: "ddl-currencies",
            dropdownType: "select2",
            dropdownData: items,
            onClosed: function (currencyName) {
                _self.currencyState.setValue(currencyName);
            }
        });
    };
    MatchWinLossReportSummary.prototype.initCurrencyState = function () {
        var _self = this;
        var updateBaseCurrency = function (currencyName) {
            Fanex.ExtAcc.BlockUI();
            var optionBaseCurrency = [];
            if (currencyName === "All") {
                optionBaseCurrency = [
                    { id: "MYR", value: "MYR", text: "MYR", selected: true },
                    { id: "CNY", value: "CNY", text: "CNY", selected: false },
                    { id: "THB", value: "THB", text: "THB", selected: false },
                    { id: "IDR", value: "IDR", text: "IDR", selected: false }
                ];
            }
            else {
                optionBaseCurrency = [
                    { id: "Customer Currency", value: "Customer Currency", text: "Customer Currency", selected: true },
                    { id: "MYR", value: "MYR", text: "MYR", selected: false },
                    { id: "CNY", value: "CNY", text: "CNY", selected: false },
                    { id: "THB", value: "THB", text: "THB", selected: false },
                    { id: "IDR", value: "IDR", text: "IDR", selected: false },
                ];
            }
            _self.initBaseCurrencyDropDown(optionBaseCurrency);
            Fanex.ExtAcc.UnBlockUI();
        };
        this.currencyState.subscribe(updateBaseCurrency);
    };
    MatchWinLossReportSummary.prototype.initBaseCurrencyDropDown = function (baseCurrencies) {
        if (!baseCurrencies)
            return;
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-base-currencies",
            dropdownName: "ddl-base-currencies",
            dropdownType: "select2",
            dropdownData: baseCurrencies
        });
    };
    return MatchWinLossReportSummary;
}());
$(document).ready(function () {
    new MatchWinLossReportSummary().init();
});
//# sourceMappingURL=MatchWinLossReportSummary.js.map