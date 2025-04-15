var __assign = (this && this.__assign) || Object.assign || function(t) {
    for (var s, i = 1, n = arguments.length; i < n; i++) {
        s = arguments[i];
        for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
            t[p] = s[p];
    }
    return t;
};
var MatchWinLossReportDetail = /** @class */ (function () {
    function MatchWinLossReportDetail() {
        this.gridComponentConfig = {
            reportId: "detail",
            isMobile: isMobile,
            gridId: "detail-jqgrid",
            gridContainerId: "detail-mainGrid",
            selectColumnsId: "detail-ddl-columns",
            selectColumnsContainerId: "detail-ddl-columns-container",
            msgNoInfoId: "detail-msgNoInfo",
            excelBtnId: "detail-excel",
            pageSizeOptions: [100, 150, 200]
        };
        this.typeReport = "detail";
        this.cachePageSize = this.gridComponentConfig.pageSizeOptions[0];
        this.cachePage = 1;
        this.sortBy = "";
        this.sortType = 0;
        this.baseCurrency = "";
        this.byPassValidateLeague = false;
        this.byPassValidateEvent = false;
        this.paramsState = new StateManager({
            "FromDate": null,
            "ToDate": null,
            "Sport": null,
            "Leagues": null,
            "Matches": null,
            "BetTypes": null
        });
        this.filterComponents = new MatchWinLossFilter($);
        this.gridComponent = new DetailGrid(this.gridComponentConfig);
        this.isMY = $("#inp-isMY").val().toLowerCase() === "true";
        this.sportState = new StateManager("");
        this.datesState = new StateManager({ fromDate: null, toDate: null });
        this.leagueIdsState = new StateManager("");
        this.currencyState = new StateManager("All");
    }
    MatchWinLossReportDetail.prototype.init = function () {
        var _this = this;
        Fanex.ExtAcc.BlockUI();
        this.initDateRangePicker();
        this.initSportsDropdown(JSON.parse($("#inp-sports").val()));
        this.initLeaguesDropdown(JSON.parse($("#inp-leagues").val()));
        this.initEventsDropdown(JSON.parse($("#inp-events").val()));
        this.initBetTypesDropdown(JSON.parse($("#inp-bettypes").val()));
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
        this.gridComponent.initSelectColumnsDropdown(DetailGrid.DefaultColumns, [0, 1, 2]);
        this.initExportExcelBtn();
        Fanex.ExtAcc.UnBlockUI();
    };
    MatchWinLossReportDetail.prototype.initDateRangePicker = function () {
        var _self = this;
        this.filterComponents.renderSingleDatePicker({
            selectorId: "#" + this.typeReport + "-date-range-picker",
            onClosed: function (value) {
                _self.datesState.setValue(value);
            }
        });
        this.initCalendarState();
    };
    MatchWinLossReportDetail.prototype.initCalendarState = function () {
        var _self = this;
        var initDate = this.filterComponents.getCalendarValues();
        _self.datesState.setValue(initDate);
        var updateLeaguesEvents = function (_a) {
            var fromDate = _a.fromDate, toDate = _a.toDate;
            Fanex.ExtAcc.BlockUI();
            _self.getLeaguesAndEvents(_self.sportState.getValue(), fromDate, toDate, function (_a) {
                var leagues = _a.leagues, events = _a.events;
                _self.initLeaguesDropdown(leagues, true);
                _self.initEventsDropdown(events);
            });
            Fanex.ExtAcc.UnBlockUI();
        };
        _self.datesState.subscribe(updateLeaguesEvents);
    };
    MatchWinLossReportDetail.prototype.initSportsDropdown = function (sports) {
        if (!sports)
            return;
        var _self = this;
        var items = [];
        sports.forEach(function (e, i) {
            items.push({
                id: e.SportType,
                text: e.SportName,
                selected: i === 0
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
    MatchWinLossReportDetail.prototype.initSportState = function (sportType) {
        var _self = this;
        this.sportState.setValue(sportType);
        var updateLeaguesEvents = function (sportID) {
            Fanex.ExtAcc.BlockUI();
            var _a = _self.datesState.getValue(), fromDate = _a.fromDate, toDate = _a.toDate;
            _self.getLeaguesAndEvents(sportID, fromDate, toDate, function (_a) {
                var leagues = _a.leagues, events = _a.events;
                _self.initLeaguesDropdown(leagues, true);
                _self.initEventsDropdown(events);
            });
            Fanex.ExtAcc.UnBlockUI();
        };
        _self.sportState.subscribe(updateLeaguesEvents);
    };
    MatchWinLossReportDetail.prototype.initLeaguesDropdown = function (leagues, isSkipNotify) {
        if (isSkipNotify === void 0) { isSkipNotify = false; }
        this.byPassValidateLeague = leagues.length === 0;
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
        var _self = this;
        !isSkipNotify && _self.initLeagueIdsState(leagues);
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-leagues",
            dropdownName: "ddl-leagues",
            dropdownType: "fanexMultiSelectSearchable",
            dropdownData: items,
            dropdownConfig: {
                noneSelectedText: items.length ? "Select league" : "There is no League",
            },
            allowRerender: true,
            toggleWidth: isMobile.phone ? 243 : 193,
            minWidth: isMobile.phone ? 300 : 500,
            onClosed: function (leagueIDs) {
                _self.leagueIdsState.setValue(leagueIDs);
            }
        });
    };
    MatchWinLossReportDetail.prototype.initLeagueIdsState = function (leagues) {
        var _self = this;
        var leagueIDs = leagues.map(function (e) { return e.LeagueId; }).join(",");
        this.leagueIdsState.setValue(leagueIDs);
        var updateEvents = function (leagueIDs) {
            Fanex.ExtAcc.BlockUI();
            _self.getEvents(_self.sportState.getValue(), leagueIDs, _self.datesState.getValue().fromDate);
            Fanex.ExtAcc.UnBlockUI();
        };
        this.leagueIdsState.subscribe(updateEvents);
    };
    MatchWinLossReportDetail.prototype.initEventsDropdown = function (events) {
        this.byPassValidateEvent = events.length === 0;
        var items = [];
        var maxLength = 0;
        events.forEach(function (e, i) {
            var textValue = e.HomeName + "-vs-" + e.AwayName;
            if ((e.textValue || "").length > maxLength) {
                maxLength = e.textValue.length;
            }
            items.push({
                id: e.MatchId,
                value: e.MatchId,
                text: textValue,
                checked: false,
                enable: true,
                items: []
            });
        });
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-events",
            dropdownName: "ddl-events",
            dropdownConfig: {
                noneSelectedText: items.length ? "Select event" : "There is no event"
            },
            dropdownType: "fanexMultiSelectSearchable",
            dropdownData: items,
            toggleWidth: isMobile.phone ? 243 : 193,
            minWidth: isMobile.phone ? 300 : 500,
        });
    };
    MatchWinLossReportDetail.prototype.initBetTypesDropdown = function (betTypes) {
        var items = [];
        betTypes.forEach(function (e, i) {
            items.push({
                id: e.BetTypeId.toString(),
                value: e.BetTypeId.toString(),
                text: e.BetTypeName,
                checked: false,
                enable: true,
                items: []
            });
        });
        var _self = this;
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-bet-types",
            dropdownName: "ddl-bet-types",
            dropdownConfig: {
                noneSelectedText: "Select bet type"
            },
            dropdownType: "fanexMultiSelectSearchable",
            dropdownData: items,
            toggleWidth: isMobile.phone ? 243 : 193,
            minWidth: isMobile.phone ? 250 : 300
        });
    };
    MatchWinLossReportDetail.prototype.initCurrencyDropDown = function (currencies) {
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
    MatchWinLossReportDetail.prototype.initCurrencyState = function () {
        var _self = this;
        var updateBaseCurrency = function (currencyID) {
            Fanex.ExtAcc.BlockUI();
            var optionBaseCurrency = [];
            if (currencyID === "All") {
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
    MatchWinLossReportDetail.prototype.initBaseCurrencyDropDown = function (baseCurrencies) {
        if (!baseCurrencies)
            return;
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-base-currencies",
            dropdownName: "ddl-base-currencies",
            dropdownType: "select2",
            dropdownData: baseCurrencies
        });
    };
    MatchWinLossReportDetail.prototype.handleClickSubmit = function () {
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
                url: resolveClientUrl("MatchWinLossReports/GetDetailReport"),
                type: "post",
                dataType: "json",
                cache: false,
                data: paramsData,
                success: function (result) { return _this.submitSuccess(result); }
            });
        }
    };
    MatchWinLossReportDetail.prototype.handleClickPage = function (_a) {
        var _this = this;
        var page = _a.page, pageSize = _a.pageSize;
        Fanex.ExtAcc.BlockUI();
        var paramsData = this.paramsState.getValue();
        paramsData.Page = page;
        paramsData.PageSize = pageSize;
        $.ajax({
            url: resolveClientUrl("MatchWinLossReports/GetDetailReport"),
            type: "post",
            dataType: "json",
            cache: false,
            data: paramsData,
            success: function (result) { return _this.submitSuccess(result); }
        });
    };
    MatchWinLossReportDetail.prototype.handleClickSort = function (_a) {
        var _this = this;
        var sortBy = _a.sortBy, sortType = _a.sortType;
        Fanex.ExtAcc.BlockUI();
        var paramsData = this.paramsState.getValue();
        paramsData.SortBy = sortBy;
        paramsData.SortType = sortType;
        $.ajax({
            url: resolveClientUrl("MatchWinLossReports/GetDetailReport"),
            type: "post",
            dataType: "json",
            cache: false,
            data: paramsData,
            success: function (result) { return _this.submitSuccess(result); }
        });
    };
    MatchWinLossReportDetail.prototype.getParams = function () {
        var params = this.filterComponents.getAllFilterValues();
        var dropdownParams = params.dropdownParams, calendarParams = params.calendarParams;
        var obj = {
            "FromDate": calendarParams.fromDate,
            "ToDate": calendarParams.toDate,
            "Sport": dropdownParams[0].currentValue,
            "Leagues": dropdownParams[1].currentValue,
            "Matches": dropdownParams[2].currentValue,
            "BetTypes": dropdownParams[3].currentValue
        };
        if (this.isMY) {
            var isCustomerCurrency = dropdownParams[5].currentValue === "Customer Currency";
            obj.Currencies = dropdownParams[4].currentValue === "All" ? "" : dropdownParams[4].currentValue;
            obj.BaseCurrency = isCustomerCurrency ? "" : dropdownParams[5].currentValue;
            this.baseCurrency = isCustomerCurrency ? obj["Currencies"] : obj["BaseCurrency"];
        }
        return obj;
    };
    MatchWinLossReportDetail.prototype.submitSuccess = function (result) {
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
    MatchWinLossReportDetail.prototype.initExportExcelBtn = function () {
        var isValid = this.validateParams();
        if (isValid) {
            $("#" + this.gridComponentConfig.excelBtnId).off().click(function () {
                var url = resolveClientUrl("MatchWinLossReports/ExportDetailReport");
                window.top.exportExcel(url, document);
            });
        }
    };
    MatchWinLossReportDetail.prototype.reportTitle = function () {
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
    MatchWinLossReportDetail.prototype.renderGrid = function (data) {
        var _this = this;
        try {
            var gridOptions = this.gridComponent.getGridOptions();
            this.reportTitle();
            var totalGrandTotalSystemParlayOutright = this.gridComponent.buildFooterRows(__assign({}, data, { currency: this.isMY ? this.baseCurrency : "" }));
            if (data.Events.length > 0) {
                gridOptions.bodyRows = this.gridComponent.buildBodyRows(__assign({}, data, { currency: this.isMY ? this.baseCurrency : "" }));
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
    MatchWinLossReportDetail.prototype.validateParams = function () {
        var params = this.filterComponents.getAllFilterValues();
        var dropdownParams = params.dropdownParams;
        var isValidLeague = dropdownParams.filter(function (e) { return e.name === "ddl-leagues"; })[0].currentValue.length > 0;
        if (!isValidLeague) {
            return this.byPassValidateLeague;
        }
        var isValidEvents = dropdownParams.filter(function (e) { return e.name === "ddl-events"; })[0].currentValue.length > 0;
        if (!isValidEvents) {
            return this.byPassValidateEvent;
        }
        var isValidBetType = dropdownParams.filter(function (e) { return e.name === "ddl-bet-types"; })[0].currentValue.length > 0;
        if (!isValidBetType) {
            return false;
        }
        return true;
    };
    MatchWinLossReportDetail.prototype.getLeaguesAndEvents = function (sportID, fromDate, toDate, callback) {
        Fanex.ExtAcc.BlockUI();
        var params = {
            sportId: sportID,
            fromDate: fromDate,
            toDate: toDate
        };
        $.ajax({
            url: resolveClientUrl("MatchWinLossReports/GetLeaguesAndEvents"),
            type: "post",
            dataType: "json",
            cache: false,
            data: params,
            success: function (result) {
                callback(result);
            }
        });
    };
    MatchWinLossReportDetail.prototype.getEvents = function (sportID, leagueIDs, fromDate) {
        var _this = this;
        var params = {
            sportId: sportID,
            leagueIds: leagueIDs,
            fromDate: fromDate
        };
        $.ajax({
            url: resolveClientUrl("MatchWinLossReports/GetEvents"),
            type: "post",
            dataType: "json",
            cache: false,
            data: params,
            success: function (result) {
                _this.initEventsDropdown(result);
            }
        });
    };
    return MatchWinLossReportDetail;
}());
$(document).ready(function () {
    new MatchWinLossReportDetail().init();
});
//# sourceMappingURL=MatchWinLossReportDetailNew.js.map