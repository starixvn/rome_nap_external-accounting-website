var MatchWinLossReportDetailOld = /** @class */ (function () {
    function MatchWinLossReportDetailOld($, topWindow) {
        this.typeReport = "detail";
        this.$ = $;
        this.topWindow = topWindow;
        this.dateRangePickerComponent = new DateRangePickerWrapper();
        this.sportDDL = new SingleSelectionJQueryComponent();
        this.currencyDDL = new SingleSelectionJQueryComponent();
        this.baseCurrencyDDL = new SingleSelectionJQueryComponent();
        this.gridBuilder = new DetailGridBuilder(isMobile);
        this.datesState = new StateManager({ fromDate: null, toDate: null });
        this.leagueIdsState = new StateManager("");
    }
    MatchWinLossReportDetailOld.prototype.init = function () {
        var _this = this;
        this.cacheElements();
        var initSport = JSON.parse(this.$("#inp-sports").val());
        this.sportState = new StateManager(initSport[0].SportType);
        this.initDateRangePicker();
        this.initSportsDropdown(initSport);
        this.initLeaguesDropdown(JSON.parse(this.$("#inp-leagues").val()));
        this.initEventsDropdown(JSON.parse(this.$("#inp-events").val()));
        this.initBetTypesDropdown(JSON.parse(this.$("#inp-bettypes").val()));
        if (this.cacheFormData.isMY) {
            this.initCurrencyDropDown(JSON.parse(this.$("#inp-currencies").val()));
            this.initBaseCurrencyDropDown([
                { id: "MYR", value: "MYR", text: "MYR", selected: true },
                { id: "CNY", value: "CNY", text: "CNY", selected: false },
                { id: "THB", value: "THB", text: "THB", selected: false },
                { id: "IDR", value: "IDR", text: "IDR", selected: false }
            ]);
        }
        this.$("#" + this.typeReport + "-submit-btn").click(function () { return _this.submit(); });
        this.initSelectColumnsDropDown(this.gridBuilder.getDefaultColumns(), [0, 1, 2]);
    };
    MatchWinLossReportDetailOld.prototype.cacheElements = function () {
        // this.iconFilter = this.$("#span-iconFilter");
        // this.filterContainer = this.$("#filter-container");
        // this.ddlSubGroup = this.$("#ddl-subGroup");
        // this.btnExportExcel = this.$("#exporttoexcel");
        this.gridContainer = this.$("#" + this.typeReport + "-mainGrid");
        this.grid = this.$("#" + this.typeReport + "-jqgrid");
        this.msgNoInfo = this.$("#" + this.typeReport + "-msgNoinfo");
        this.cacheFormData = {
            pageSize: 100,
            page: 1,
            groupId: this.$("#hidden-groupId").val(),
            groupName: this.$("#hidden-groupName").val(),
            sortBy: "",
            sortType: "",
            selectedCurrency: "",
            isMY: this.$("#inp-isMY").val().toLowerCase() === "true"
        };
    };
    MatchWinLossReportDetailOld.prototype.initDateRangePicker = function () {
        var _self = this;
        var initDate = this.dateRangePickerComponent.renderSingleDatePicker({
            selectorId: "#" + this.typeReport + "-date-range-picker",
            onClosed: function (value) {
                _self.datesState.setValue(value);
            }
        });
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
    MatchWinLossReportDetailOld.prototype.initSportsDropdown = function (sports) {
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
        var options = {
            selectorId: "#" + this.typeReport + "-ddl-sport",
            data: items,
            onClosed: function (sportID) {
                _self.sportState.setValue(sportID);
            }
        };
        this.sportDDL.render(options);
    };
    MatchWinLossReportDetailOld.prototype.initLeaguesDropdown = function (leagues) {
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
        this.leagueDDL = this.$("#" + this.typeReport + "-ddl-leagues").fanexMultiSelect({
            showFilter: true,
            // toggleWidth: isMobile.phone ? 243 : 193,
            // minWidth: isMobile.phone ? 300 : 500,
            checkAllText: "Select all",
            noneSelectedText: "Select Leagues",
            useCustomCheckAllText: false,
            customCheckAllText: "All Leagues",
            isCheckAll: true,
            listData: items,
            // multiple: false,
            onLeave: function () {
                var leagueIDs = _self.leagueDDL.fanexMultiSelect("getItems", true).toString();
                _self.leagueIdsState.setValue(leagueIDs);
            },
            onOpen: function () {
                _self.$("#fanex-multiselect-options-ddl-leagues #fanex-multiselect-filter-input").trigger("focus");
            },
            showTooltip: true
        });
    };
    MatchWinLossReportDetailOld.prototype.initEventsDropdown = function (events) {
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
        var _self = this;
        this.eventsDDL = this.$("#" + this.typeReport + "-ddl-events").fanexMultiSelect({
            showFilter: true,
            // toggleWidth: isMobile.phone ? 243 : 193,
            // minWidth: isMobile.phone ? 300 : 500,
            checkAllText: "Select all",
            noneSelectedText: "Select Events",
            useCustomCheckAllText: false,
            customCheckAllText: "All Events",
            isCheckAll: true,
            listData: items,
            // multiple: false,
            onCheck: function () {
                //$this.getEvents();
            },
            onOpen: function () {
                _self.$("#fanex-multiselect-options-ddl-leagues #fanex-multiselect-filter-input").trigger("focus");
            }
        });
    };
    MatchWinLossReportDetailOld.prototype.initBetTypesDropdown = function (betTypes) {
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
        this.bettypeDDL = this.$("#" + this.typeReport + "-ddl-bettypes").fanexMultiSelect({
            showFilter: true,
            // toggleWidth: isMobile.phone ? 243 : 193,
            // minWidth: isMobile.phone ? 300 : 500,
            checkAllText: "Select all",
            noneSelectedText: "Select bettype",
            useCustomCheckAllText: false,
            customCheckAllText: "All Bettypes",
            isCheckAll: true,
            listData: items,
            // multiple: false,
            onCheck: function () {
                //$this.getEvents();
            },
            onOpen: function () {
                _self.$("#fanex-multiselect-options-ddl-leagues #fanex-multiselect-filter-input").trigger("focus");
            }
        });
    };
    MatchWinLossReportDetailOld.prototype.initCurrencyDropDown = function (sports) {
        if (!sports)
            return;
        var items = [
            {
                id: "All",
                value: "All",
                text: "All",
                selected: false
            }
        ];
        sports.forEach(function (e, i) {
            items.push({
                id: e.CurrencyId,
                value: e.CurrencyId,
                text: e.CurrencyName,
                selected: false
            });
        });
        var options = {
            selectorId: "#" + this.typeReport + "-ddl-currencies",
            data: items
        };
        this.currencyDDL.render(options);
    };
    MatchWinLossReportDetailOld.prototype.initBaseCurrencyDropDown = function (baseCurrencies) {
        var options = {
            selectorId: "#" + this.typeReport + "-ddl-base-currencies",
            data: baseCurrencies
        };
        this.baseCurrencyDDL.render(options);
    };
    MatchWinLossReportDetailOld.prototype.initSelectColumnsDropDown = function (columns, ignoreColumns) {
        var options = [];
        var numberOfColumns = columns.length;
        for (var i = 0; i < numberOfColumns; i++) {
            if (ignoreColumns.includes(i))
                continue;
            options.push({
                id: i,
                value: i,
                text: columns[i].ColumnName2,
                checked: false,
                enable: true,
                items: []
            });
        }
        var _self = this;
        this.columnsDDL = this.$("#" + this.typeReport + "-ddl-columns").fanexMultiSelect({
            showFilter: false,
            // toggleWidth: isMobile.phone ? 243 : 193,
            // minWidth: isMobile.phone ? 300 : 500,
            checkAllText: "Select all",
            noneSelectedText: "Select Columns",
            useCustomCheckAllText: false,
            customCheckAllText: "All Columns",
            isCheckAll: true,
            listData: options,
            // multiple: false,
            onLeave: function () {
                _self.showHideColumns();
            }
        });
    };
    MatchWinLossReportDetailOld.prototype.showHideColumns = function () {
        var unSelectedColumns = this.columnsDDL.fanexMultiSelect("getItems", false);
        this.grid.fanexGrid("hideColumns", this.gridOptions, unSelectedColumns.map(function (colIndex) { return Number(colIndex); }));
    };
    MatchWinLossReportDetailOld.prototype.getFilterValues = function () {
        var dateTimeParams = this.dateRangePickerComponent.getDateRangeValues();
        var selectedSport = this.sportDDL.selectedOption();
        var selectedLeague = this.leagueDDL.fanexMultiSelect("getItems", true).toString();
        var selectedEvents = this.eventsDDL.fanexMultiSelect("getItems", true).toString();
        var selectedBetTypes = this.bettypeDDL.fanexMultiSelect("getItems", true).toString();
        var timeFilter = "&FromDate=" + dateTimeParams.fromDate + "&ToDate=" + dateTimeParams.toDate;
        var sportFilter = "&Sport=" + selectedSport;
        var leagueFilter = "&Leagues=" + selectedLeague;
        var eventsFilter = "&Matches=" + selectedEvents;
        var betTypeFilter = "&BetTypes=" + selectedBetTypes;
        var pageFilter = "&PageSize=" + this.cacheFormData.pageSize + "&Page=" + this.cacheFormData.page;
        var sortFilter = "&SortBy=" + this.cacheFormData.sortBy + "&SortType=" + this.cacheFormData.sortType;
        var isValidLeague = selectedLeague.length > 0;
        if (!isValidLeague) {
            Fanex.ExtAcc.Form.Alert("Alert", "Please select at least one league", 300, 120, "alertWLD");
            return;
        }
        var isValidEvents = selectedEvents.length > 0;
        if (!isValidEvents) {
            Fanex.ExtAcc.Form.Alert("Alert", "Please select at least one event", 300, 120, "alertWLD");
            return;
        }
        var isValidBetType = selectedBetTypes.length > 0;
        if (!isValidBetType) {
            Fanex.ExtAcc.Form.Alert("Alert", "Please select at least one bet type", 300, 120, "alertWLD");
            return;
        }
        var parameterUrl = "";
        if (this.cacheFormData.isMY) {
            var selectedCurrency = this.currencyDDL.selectedOption();
            var selectedBaseCurrency = this.baseCurrencyDDL.selectedOption();
            var currencyFilter = "&Currencies=" + selectedCurrency;
            var baseCurrencyFilter = "&BaseCurrency=" + selectedBaseCurrency;
            parameterUrl = "" + timeFilter + pageFilter + sportFilter + leagueFilter + eventsFilter + betTypeFilter + currencyFilter + baseCurrencyFilter + sortFilter;
            this.cacheFormData.selectedCurrency = selectedCurrency;
        }
        else {
            parameterUrl = "" + timeFilter + pageFilter + sportFilter + leagueFilter + eventsFilter + betTypeFilter + sortFilter;
        }
        var result = {
            parameterUrl: parameterUrl,
            dateTimeParams: dateTimeParams,
        };
        return result;
    };
    MatchWinLossReportDetailOld.prototype.getEvents = function (sportID, leagueIDs, fromDate) {
        var _this = this;
        Fanex.ExtAcc.BlockUI();
        this.$.ajax({
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
    MatchWinLossReportDetailOld.prototype.getLeaguesAndEvents = function (sportID, fromDate, toDate, callback) {
        Fanex.ExtAcc.BlockUI();
        this.$.ajax({
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
    MatchWinLossReportDetailOld.prototype.submit = function () {
        var _this = this;
        var filterValues = this.getFilterValues();
        if (filterValues) {
            Fanex.ExtAcc.BlockUI();
            this.$.ajax({
                url: resolveClientUrl("MatchWinLossReports/GetDetailReport?" + filterValues.parameterUrl),
                type: "post",
                dataType: "json",
                cache: false,
                success: function (result) {
                    if (result.errorCode) {
                        Fanex.ExtAcc.Form.Alert("Error", result.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
                        return;
                    }
                    try {
                        if (result.Events.length > 0) {
                            _this.renderGrid(result, _this.datesState.getValue());
                        }
                        else {
                            _this.renderNoData();
                        }
                    }
                    catch (e) {
                        console.log(e);
                    }
                    workaroundForGridOnPhone();
                }
            });
            Fanex.ExtAcc.UnBlockUI();
        }
    };
    MatchWinLossReportDetailOld.prototype.exportExcel = function () {
        var filterValues = this.getFilterValues();
        if (filterValues) {
            Fanex.ExtAcc.BlockUI();
            var exportUrl = resolveClientUrl("MatchWinLossReports/ExportDetailReport?" + filterValues.parameterUrl);
            this.topWindow.exportExcel(exportUrl, document);
            Fanex.ExtAcc.UnBlockUI();
        }
    };
    MatchWinLossReportDetailOld.prototype.renderNoData = function () {
        this.gridContainer.removeAttr("style").addClass("hide");
        this.grid.removeAttr("style").html("");
        // this.btnExportExcel.addClass("hide");
        this.msgNoInfo.css("display", "block");
    };
    MatchWinLossReportDetailOld.prototype.reportTitle = function (dateTimeParams) {
        var fromDate = dateTimeParams.fromDate, toDate = dateTimeParams.toDate;
        var timeInfo = fromDate === toDate
            ? "on " + fromDate
            : "from " + fromDate + " to " + toDate;
        return "Match Win Loss Report Detail " + timeInfo;
    };
    MatchWinLossReportDetailOld.prototype.renderGrid = function (data, dateTimeParams) {
        var _this = this;
        var gridOptions = this.gridBuilder.getGridOptions();
        // setPagerOptionOnMobile(this.gridOptions); ?? lam gi
        gridOptions.titleInfo.text = this.reportTitle(dateTimeParams);
        gridOptions.columns = this.gridBuilder.getDefaultColumns();
        gridOptions.bodyRows = this.gridBuilder.buildBodyRows(data);
        gridOptions.footerRows = this.gridBuilder.buildFooterRows(data, this.cacheFormData.selectedCurrency, this.cacheFormData.isMY);
        gridOptions.pagerOption.pagerSelectCallBack = function (pageIndex, pageSize) {
            _this.cacheFormData.page = pageIndex + 1;
            _this.cacheFormData.pageSize = pageSize;
            _this.submit();
        };
        gridOptions.pagerOption.currentPage = this.cacheFormData.page - 1;
        gridOptions.pagerOption.itemsPerPage = this.cacheFormData.pageSize;
        gridOptions.pagerOption.totalItem = data.TotalRows;
        gridOptions.sortOption.serverSortFunction = function (colname, currentDirection) {
            _this.cacheFormData.sortType = currentDirection === 'desc' ? 1 : 0;
            _this.cacheFormData.sortBy = colname;
            _this.submit();
        };
        this.gridContainer.removeClass("hide");
        // this.btnExportExcel.removeClass("hide");
        this.msgNoInfo.removeAttr("style");
        this.gridOptions = gridOptions;
        setPagerOptionOnMobile(gridOptions);
        this.grid.fanexGrid(gridOptions);
        this.$("#" + DetailGridBuilder.ExcelBtnId).click(function () { return _this.exportExcel(); });
        workaroundForGridOnPhone();
    };
    return MatchWinLossReportDetailOld;
}());
$(document).ready(function () {
    var detail = new MatchWinLossReportDetailOld($, window.top);
    detail.init();
});
//# sourceMappingURL=MatchWinLossReportDetail.js.map