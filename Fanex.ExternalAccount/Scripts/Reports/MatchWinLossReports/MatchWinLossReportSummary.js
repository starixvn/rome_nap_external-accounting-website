var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
var MatchWinLossReportSummary = /** @class */ (function () {
    function MatchWinLossReportSummary() {
        this.typeReport = "summary";
        this.cachePageSize = 10;
        this.cachePage = 1;
        this.sortBy = "";
        this.sortType = 1;
        this.baseCurrency = "";
        this.isMY = false;
        this.gridComponentConfig = {
            reportId: "summary",
            isMobile: isMobile,
            gridId: "summary-jqgrid",
            gridContainerId: "summary-mainGrid",
            selectColumnsId: "summary-ddl-columns",
            excelBtnId: "summary-excel",
            pageSizeOptions: [500, 1000, 1500]
        };
        this.datesState = new StateManager({ fromDate: null, toDate: null });
        this.filterComponents = new MatchWinLossFilter($);
        this.gridComponent = new SummaryGrid(this.gridComponentConfig);
        this.isMY = $("#inp-isMY").val().toLowerCase() === "true";
    }
    MatchWinLossReportSummary.prototype.init = function () {
        var _this = this;
        var initSport = JSON.parse($("#inp-sports").val());
        this.sportState = new StateManager(initSport[0].SportType);
        this.initDateRangePicker();
        this.initSportsDropdown(initSport);
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
        $("#" + this.typeReport + "-submit-btn").click(function () { return _this.clickSubmit(1, _this.gridComponentConfig.pageSizeOptions[0]); });
    };
    MatchWinLossReportSummary.prototype.clickSubmit = function (page, pageSize) {
        var _this = this;
        this.cachePage = page || this.cachePage;
        this.cachePageSize = pageSize || this.cachePageSize;
        Fanex.ExtAcc.BlockUI();
        var isValid = this.validateQueryString();
        if (this.filterComponents.isUnValidInputs() || !isValid) {
            Fanex.ExtAcc.UnBlockUI();
            //this.gridComponent.hideNoInfo(false);
        }
        else {
            $.ajax({
                url: resolveClientUrl("MatchWinLossReports/GetSummaryReport" + this.getQueryString(page, pageSize)),
                type: "post",
                dataType: "json",
                cache: false,
                success: function (result) { return _this.submitSuccess(result); }
            });
        }
    };
    MatchWinLossReportSummary.prototype.getLeagues = function (sportID, fromDate, toDate, callback) {
        $.ajax({
            url: resolveClientUrl("MatchWinLossReports/GetLeagues?&sportId=" + sportID + "&fromDate=" + fromDate + "&toDate=" + toDate),
            type: "post",
            dataType: "json",
            cache: false,
            success: function (result) {
                callback(result);
            }
        });
    };
    MatchWinLossReportSummary.prototype.submitSuccess = function (result) {
        if (result.errorCode) {
            Fanex.ExtAcc.UnBlockUI();
            Fanex.ExtAcc.Form.Alert("Error", result.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
            return;
        }
        if (result.Events.length) {
            this.gridComponent.hideNoInfo(true);
            this.renderGrid(result);
            this.initExportExcelBtn();
        }
        else {
            this.gridComponent.hideNoInfo(false);
        }
        Fanex.ExtAcc.UnBlockUI();
    };
    MatchWinLossReportSummary.prototype.initExportExcelBtn = function () {
        var _this = this;
        var isValid = this.validateQueryString();
        if (isValid) {
            $("#" + this.gridComponentConfig.excelBtnId).off().click(function () {
                var url = resolveClientUrl("MatchWinLossReports/ExportSummaryReport" + _this.getQueryString());
                window.top.exportExcel(url, document);
            });
        }
    };
    MatchWinLossReportSummary.prototype.renderGrid = function (data) {
        var _this = this;
        try {
            var gridOptions = this.gridComponent.getGridOptions();
            gridOptions.titleInfo.text = this.reportTitle();
            gridOptions.bodyRows = this.gridComponent.buildBodyRows(data.Events);
            gridOptions.footerRows = this.gridComponent.buildFooterRows(__assign({}, data, { baseCurrency: this.baseCurrency, isMYHeadline: this.isMY }));
            gridOptions.pagerOption.pagerSelectCallBack = function (pageIndex, pageSize) {
                _this.cachePage = pageIndex + 1;
                _this.cachePageSize = pageSize;
                _this.clickSubmit(_this.cachePage, _this.cachePageSize);
            };
            gridOptions.pagerOption.currentPage = this.cachePage - 1;
            gridOptions.pagerOption.itemsPerPage = this.cachePageSize;
            gridOptions.pagerOption.totalItem = data.TotalItems;
            gridOptions.sortOption.serverSortFunction = function (colname, currentDirection) {
                _this.sortType = currentDirection === 'desc' ? 1 : 0;
                _this.sortBy = colname;
                _this.clickSubmit(1, _this.gridComponentConfig.pageSizeOptions[0]);
            };
            setPagerOptionOnMobile(gridOptions);
            this.gridComponent.setGridOptions(gridOptions);
            this.gridComponent.renderGrid(gridOptions);
            workaroundForGridOnPhone();
        }
        catch (e) {
            console.log(e);
        }
    };
    MatchWinLossReportSummary.prototype.reportTitle = function () {
        var _a = this.filterComponents.getCalendarValues(), fromDate = _a.fromDate, toDate = _a.toDate;
        var formatedFromDate = moment(fromDate, 'MM/DD/YYYY').format('MMM DD, YYYY');
        var timeInfo = fromDate === toDate
            ? "on " + formatedFromDate
            : "from " + formatedFromDate + " to " + moment(toDate, 'MM/DD/YYYY').format('MMM DD, YYYY');
        return "Match Win Loss Report Summary " + timeInfo;
    };
    MatchWinLossReportSummary.prototype.initDateRangePicker = function () {
        var _self = this;
        this.filterComponents.renderCalendarPicker({
            selectorId: "#" + this.typeReport + "-date-range-picker",
            onClosed: function (value) {
                console.log("value", value);
                _self.datesState.setValue(value);
            }
        });
        var initDate = this.filterComponents.getCalendarValues();
        _self.datesState.setValue(initDate);
        var updateLeagues = function (dateRange) {
            var fromDate = dateRange.fromDate, toDate = dateRange.toDate;
            Fanex.ExtAcc.BlockUI();
            _self.getLeagues(_self.sportState.getValue(), fromDate, toDate, function (leagues) {
                _self.initLeaguesDropdown(leagues);
            });
            Fanex.ExtAcc.UnBlockUI();
        };
        _self.datesState.subscribe(updateLeagues);
    };
    MatchWinLossReportSummary.prototype.validateQueryString = function () {
        var params = this.filterComponents.getAllFilterValues();
        var dropdownParams = params.dropdownParams;
        console.log(" dropdownParams[1].currentValue", dropdownParams[1].currentValue);
        var isValidLeague = dropdownParams[1].currentValue.length > 0;
        if (!isValidLeague) {
            Fanex.ExtAcc.Form.Alert("Alert", "Please select at least one league", 300, 120, "alertWLD");
            return false;
        }
        return true;
    };
    MatchWinLossReportSummary.prototype.getQueryString = function (page, pageSize) {
        var params = this.filterComponents.getAllFilterValues();
        var dropdownParams = params.dropdownParams, calendarParams = params.calendarParams;
        var queryString = [
            '?FromDate=', calendarParams.fromDate,
            '&ToDate=', calendarParams.toDate,
            '&Sport=', dropdownParams[0].currentValue,
            '&Leagues=', dropdownParams[1].currentValue,
            '&Page=', page,
            '&PageSize=', pageSize,
            '&SortBy=', this.sortBy,
            '&SortType=', this.sortType
        ];
        if (this.isMY) {
            this.baseCurrency = dropdownParams[3].currentValue;
            queryString.push([
                '&Currency=', dropdownParams[2].currentValue,
                '&BaseCurrency=', dropdownParams[3].currentValue
            ]);
        }
        return queryString.join('');
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
        var updateLeagues = function (sportID) {
            var _a = _self.datesState.getValue(), fromDate = _a.fromDate, toDate = _a.toDate;
            _self.getLeagues(sportID, fromDate, toDate, function (leagues) {
                _self.initLeaguesDropdown(leagues);
            });
        };
        _self.sportState.subscribe(updateLeagues);
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-sport",
            dropdownName: "ddl-sport",
            dropdownType: "select2",
            dropdownOptions: items,
            onClosed: function (sportID) {
                _self.sportState.setValue(sportID);
            }
        });
    };
    MatchWinLossReportSummary.prototype.initLeaguesDropdown = function (leagues) {
        if (!leagues)
            return;
        var items = [];
        leagues.forEach(function (e, i) {
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
            dropdownType: "fanexMultiSelect",
            dropdownOptions: items,
            allowRerender: true
        });
    };
    MatchWinLossReportSummary.prototype.initCurrencyDropDown = function (currencies) {
        if (!currencies)
            return;
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
                id: e.CurrencyId,
                value: e.CurrencyId,
                text: e.CurrencyName,
                selected: false
            });
        });
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-currencies",
            dropdownName: "ddl-currencies",
            dropdownType: "select2",
            dropdownOptions: items
        });
    };
    MatchWinLossReportSummary.prototype.initBaseCurrencyDropDown = function (baseCurrencies) {
        if (!baseCurrencies)
            return;
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-base-currencies",
            dropdownName: "ddl-base-currencies",
            dropdownType: "select2",
            dropdownOptions: baseCurrencies
        });
    };
    return MatchWinLossReportSummary;
}());
$(document).ready(function () {
    new MatchWinLossReportSummary().init();
});
//# sourceMappingURL=MatchWinLossReportSummary.js.map