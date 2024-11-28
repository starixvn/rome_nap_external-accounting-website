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
var MatchWinLossReportDetail = /** @class */ (function () {
    function MatchWinLossReportDetail() {
        this.gridComponentConfig = {
            reportId: "detail",
            isMobile: isMobile,
            gridId: "detail-jqgrid",
            gridContainerId: "detail-mainGrid",
            selectColumnsId: "detail-ddl-columns",
            excelBtnId: "detail-excel",
            pageSizeOptions: [500, 1000, 1500]
        };
        this.typeReport = "detail";
        this.cachePageSize = 10;
        this.cachePage = 1;
        this.sortBy = "";
        this.sortType = 1;
        this.baseCurrency = "";
        this.isMY = false;
        this.filterComponents = new MatchWinLossFilter($);
        this.gridComponent = new DetailGrid(this.gridComponentConfig);
        this.isMY = $("#inp-isMY").val().toLowerCase() === "true";
    }
    MatchWinLossReportDetail.prototype.init = function () {
        var _this = this;
        var initSport = JSON.parse($("#inp-sports").val());
        this.sportState = new StateManager(initSport[0].SportType);
        this.datesState = new StateManager({ fromDate: null, toDate: null });
        this.leagueIdsState = new StateManager("");
        this.initDateRangePicker();
        this.initSportsDropdown(initSport);
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
        // $(`#${this.typeReport}-submit-btn`).click(() => this.submit());
        $("#" + this.typeReport + "-submit-btn").click(function () { return _this.clickSubmit(1, _this.gridComponentConfig.pageSizeOptions[0]); });
    };
    MatchWinLossReportDetail.prototype.initDateRangePicker = function () {
        var _self = this;
        this.filterComponents.renderSingleDatePicker({
            selectorId: "#" + this.typeReport + "-date-range-picker",
            onClosed: function (value) {
                _self.datesState.setValue(value);
            }
        });
        var initDate = this.filterComponents.getCalendarValues();
        _self.datesState.setValue(initDate);
        var updateLeaguesEvents = function (_a) {
            var fromDate = _a.fromDate, toDate = _a.toDate;
            _self.getLeaguesAndEvents(_self.sportState.getValue(), fromDate, toDate, function (_a) {
                var leagues = _a.leagues, events = _a.events;
                _self.initLeaguesDropdown(leagues);
                _self.initEventsDropdown(events);
            });
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
        var updateLeaguesEvents = function (sportID) {
            var _a = _self.datesState.getValue(), fromDate = _a.fromDate, toDate = _a.toDate;
            _self.getLeaguesAndEvents(sportID, fromDate, toDate, function (_a) {
                var leagues = _a.leagues, events = _a.events;
                _self.initLeaguesDropdown(leagues);
                _self.initEventsDropdown(events);
            });
        };
        _self.sportState.subscribe(updateLeaguesEvents);
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
    MatchWinLossReportDetail.prototype.initLeaguesDropdown = function (leagues) {
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
        var _self = this;
        var leagueIDs = leagues.map(function (e) { return e.LeagueId; }).join(",");
        this.leagueIdsState.setValue(leagueIDs);
        var updateEvents = function (leagueIDs) {
            _self.getEvents(_self.sportState.getValue(), leagueIDs, _self.datesState.getValue().fromDate);
        };
        this.leagueIdsState.subscribe(updateEvents);
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-leagues",
            dropdownName: "ddl-leagues",
            dropdownType: "fanexMultiSelect",
            dropdownOptions: items,
            allowRerender: true,
            onClosed: function (leagueIDs) {
                _self.leagueIdsState.setValue(leagueIDs);
            }
        });
        // this.leagueDDL = this.$(`#${this.typeReport}-ddl-leagues`).fanexMultiSelect({
        //     showFilter: true,
        //     // toggleWidth: isMobile.phone ? 243 : 193,
        //     // minWidth: isMobile.phone ? 300 : 500,
        //     checkAllText: "Select all",
        //     noneSelectedText: "Select Leagues",
        //     useCustomCheckAllText: false,
        //     customCheckAllText: "All Leagues",
        //     isCheckAll: true,
        //     listData: items,
        //     // multiple: false,
        //     onLeave: function () {
        //         const leagueIDs = _self.leagueDDL.fanexMultiSelect("getItems", true).toString();
        //         _self.leagueIdsState.setValue(leagueIDs);
        //     },
        //     onOpen: function () {
        //         _self.$("#fanex-multiselect-options-ddl-leagues #fanex-multiselect-filter-input").trigger("focus")
        //     },
        //     showTooltip: true
        // });
    };
    MatchWinLossReportDetail.prototype.initEventsDropdown = function (events) {
        var items = [];
        events.forEach(function (e, i) {
            items.push({
                id: e.MatchId,
                value: e.MatchId,
                text: e.HomeName + " vs " + e.AwayName,
                checked: false,
                enable: true,
                items: []
            });
        });
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-events",
            dropdownName: "ddl-events",
            dropdownType: "fanexMultiSelect",
            dropdownOptions: items
        });
    };
    MatchWinLossReportDetail.prototype.initBetTypesDropdown = function (betTypes) {
        if (!betTypes)
            return;
        var items = [];
        betTypes.forEach(function (e, i) {
            items.push({
                id: e.BetTypeId,
                value: e.BetTypeId,
                text: e.BetTypeName,
                checked: false,
                enable: true,
                items: []
            });
        });
        var _self = this;
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-bettypes",
            dropdownName: "ddl-bettypes",
            dropdownType: "fanexMultiSelect",
            dropdownOptions: items
        });
        // this.bettypeDDL = this.$(`#${this.typeReport}-ddl-bettypes`).fanexMultiSelect({
        //     showFilter: true,
        //     // toggleWidth: isMobile.phone ? 243 : 193,
        //     // minWidth: isMobile.phone ? 300 : 500,
        //     checkAllText: "Select all",
        //     noneSelectedText: "Select bettype",
        //     useCustomCheckAllText: false,
        //     customCheckAllText: "All Bettypes",
        //     isCheckAll: true,
        //     listData: items,
        //     // multiple: false,
        //     onCheck: function () {
        //         //$this.getEvents();
        //     },
        //     onOpen: function () {
        //         _self.$("#fanex-multiselect-options-ddl-leagues #fanex-multiselect-filter-input").trigger("focus")
        //     }
        // });
    };
    MatchWinLossReportDetail.prototype.initCurrencyDropDown = function (currencies) {
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
    MatchWinLossReportDetail.prototype.initBaseCurrencyDropDown = function (baseCurrencies) {
        if (!baseCurrencies)
            return;
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-base-currencies",
            dropdownName: "ddl-base-currencies",
            dropdownType: "select2",
            dropdownOptions: baseCurrencies
        });
    };
    MatchWinLossReportDetail.prototype.clickSubmit = function (page, pageSize) {
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
                url: resolveClientUrl("MatchWinLossReports/GetDetailReport" + this.getQueryString(page, pageSize)),
                type: "post",
                dataType: "json",
                cache: false,
                success: function (result) { return _this.submitSuccess(result); }
            });
        }
    };
    MatchWinLossReportDetail.prototype.submitSuccess = function (result) {
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
    MatchWinLossReportDetail.prototype.initExportExcelBtn = function () {
        var _this = this;
        var isValid = this.validateQueryString();
        if (isValid) {
            $("#" + this.gridComponentConfig.excelBtnId).off().click(function () {
                var url = resolveClientUrl("MatchWinLossReports/ExportDetailReport" + _this.getQueryString());
                window.top.exportExcel(url, document);
            });
        }
    };
    MatchWinLossReportDetail.prototype.reportTitle = function () {
        var _a = this.filterComponents.getCalendarValues(), fromDate = _a.fromDate, toDate = _a.toDate;
        var formatedFromDate = moment(fromDate, 'MM/DD/YYYY').format('MMM DD, YYYY');
        var timeInfo = fromDate === toDate
            ? "on " + formatedFromDate
            : "from " + formatedFromDate + " to " + moment(toDate, 'MM/DD/YYYY').format('MMM DD, YYYY');
        return "Match Win Loss Report Detail " + timeInfo;
    };
    MatchWinLossReportDetail.prototype.renderGrid = function (data) {
        var _this = this;
        try {
            var gridOptions = this.gridComponent.getGridOptions();
            gridOptions.titleInfo.text = this.reportTitle();
            gridOptions.bodyRows = this.gridComponent.buildBodyRows(data);
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
    MatchWinLossReportDetail.prototype.validateQueryString = function () {
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
    MatchWinLossReportDetail.prototype.getQueryString = function (page, pageSize) {
        var params = this.filterComponents.getAllFilterValues();
        var dropdownParams = params.dropdownParams, calendarParams = params.calendarParams;
        var queryString = [
            '?FromDate=', calendarParams.fromDate,
            '&ToDate=', calendarParams.toDate,
            '&Sport=', dropdownParams[0].currentValue,
            '&Leagues=', dropdownParams[1].currentValue,
            '&Matches=', dropdownParams[2].currentValue,
            '&BetTypes=', dropdownParams[3].currentValue,
            '&Page=', page,
            '&PageSize=', pageSize,
            '&SortBy=', this.sortBy,
            '&SortType=', this.sortType
        ];
        if (this.isMY) {
            this.baseCurrency = dropdownParams[5].currentValue;
            queryString.push([
                '&Currency=', dropdownParams[4].currentValue,
                '&BaseCurrency=', dropdownParams[5].currentValue
            ]);
        }
        return queryString.join('');
    };
    MatchWinLossReportDetail.prototype.getLeaguesAndEvents = function (sportID, fromDate, toDate, callback) {
        Fanex.ExtAcc.BlockUI();
        $.ajax({
            url: resolveClientUrl("MatchWinLossReports/GetLeaguesAndEvents?&sportId=" + sportID + "&fromDate=" + fromDate + "&toDate=" + toDate),
            type: "post",
            dataType: "json",
            cache: false,
            success: function (result) {
                callback(result);
                Fanex.ExtAcc.UnBlockUI();
            }
        });
    };
    MatchWinLossReportDetail.prototype.getEvents = function (sportID, leagueIDs, fromDate) {
        var _this = this;
        Fanex.ExtAcc.BlockUI();
        $.ajax({
            url: resolveClientUrl("MatchWinLossReports/GetEvents?&sportId=" + sportID + "&leagueIds=" + leagueIDs + "&fromDate=" + fromDate),
            type: "post",
            dataType: "json",
            cache: false,
            success: function (result) {
                _this.initEventsDropdown(result);
                Fanex.ExtAcc.UnBlockUI();
            }
        });
    };
    return MatchWinLossReportDetail;
}());
$(document).ready(function () {
    new MatchWinLossReportDetail().init();
});
//# sourceMappingURL=MatchWinLossReportDetailNew.js.map