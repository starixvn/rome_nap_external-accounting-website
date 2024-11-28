declare var Fanex: any;
declare var resolveClientUrl: any;
declare var workaroundForGridOnPhone: any;
declare var isMobile: any;
declare var setPagerOptionOnMobile: any;

interface Window {
    exportExcel( url: string, document: any );
}

class MatchWinLossReportDetail {
    private readonly datesState: StateManager<{ fromDate: any, toDate: any }>;
    private readonly sportState: StateManager<string>;
    private readonly leagueIdsState: StateManager<string>;
    private readonly currencyState: StateManager<string>;
    private readonly paramsState: StateManager<ReportDetailParams>;

    private readonly gridComponentConfig: IGridComponentConfig = {
        reportId: "detail",
        isMobile: isMobile,
        gridId: "detail-jqgrid",
        gridContainerId: "detail-mainGrid",
        selectColumnsId: "detail-ddl-columns",
        selectColumnsContainerId: "detail-ddl-columns-container",
        msgNoInfoId: "detail-msgNoInfo",
        excelBtnId: "detail-excel",
        pageSizeOptions: [ 100, 150, 200 ]
    }
    private readonly filterComponents: IReportFilters;
    private readonly gridComponent: IReportGrid;
    private readonly typeReport: string = "detail";
    private readonly isMY: boolean;

    private cachePageSize: number = this.gridComponentConfig.pageSizeOptions[0];
    private cachePage: number = 1;

    private sortBy: string = "";
    private sortType: number = 0;

    private baseCurrency: string = "";
    private byPassValidateLeague: boolean = false;
    private byPassValidateEvent: boolean = false;

    constructor() {
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
        this.isMY = $("#inp-isMY").val().toLowerCase() === "true"
        this.sportState = new StateManager("");
        this.datesState = new StateManager({ fromDate: null, toDate: null });
        this.leagueIdsState = new StateManager<string>("");
        this.currencyState = new StateManager<string>("All");
    }

    init() {
        Fanex.ExtAcc.BlockUI();
        this.initDateRangePicker();
        this.initSportsDropdown(JSON.parse($("#inp-sports").val()));
        this.initLeaguesDropdown(JSON.parse($("#inp-leagues").val()))
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
        
        $(`#${this.typeReport}-submit-btn`).click(() => this.handleClickSubmit());
        this.gridComponent.initSelectColumnsDropdown(DetailGrid.DefaultColumns, [ 0, 1, 2 ]);
        this.initExportExcelBtn();
        Fanex.ExtAcc.UnBlockUI();
    }

    initDateRangePicker() {
        const _self = this;
        this.filterComponents.renderSingleDatePicker(
            {
                selectorId: `#${this.typeReport}-date-range-picker`,
                onClosed: function ( value ) {
                    _self.datesState.setValue(value);
                }
            }
        );
        this.initCalendarState();
    }

    initCalendarState() {
        const _self = this;
        const initDate = this.filterComponents.getCalendarValues();
        _self.datesState.setValue(initDate);
        const updateLeaguesEvents = function ( { fromDate, toDate } ) {
            Fanex.ExtAcc.BlockUI();
            _self.getLeaguesAndEvents(_self.sportState.getValue(), fromDate, toDate, ( { leagues, events } ) => {
                _self.initLeaguesDropdown(leagues, true);
                _self.initEventsDropdown(events);
            })
            Fanex.ExtAcc.UnBlockUI();
        }
        _self.datesState.subscribe(updateLeaguesEvents)
    }

    initSportsDropdown( sports: any ) {
        if (!sports) return;

        const _self = this;
        const items = [];
        sports.forEach(function ( e, i ) {
            items.push({
                id: e.SportType,
                text: e.SportName,
                selected: i === 0
            })
        });
        this.initSportState(sports[0].SportType);
        this.filterComponents.registerDropdownComponent({
            dropdownId: `${this.typeReport}-ddl-sport`,
            dropdownName: "ddl-sport",
            dropdownType: "select2",
            dropdownData: items,
            dropdownConfig: {
                noneSelectedText: "Select sport",
            },
            onClosed: function ( sportID ) {
                _self.sportState.setValue(sportID);
            }
        });
    }

    initSportState( sportType: any ) {
        const _self = this;
        this.sportState.setValue(sportType);
        const updateLeaguesEvents = function ( sportID ) {
            Fanex.ExtAcc.BlockUI();
            const { fromDate, toDate } = _self.datesState.getValue();
            _self.getLeaguesAndEvents(sportID, fromDate, toDate, ( { leagues, events } ) => {
                _self.initLeaguesDropdown(leagues, true);
                _self.initEventsDropdown(events);
            })
            Fanex.ExtAcc.UnBlockUI();
        }
        _self.sportState.subscribe(updateLeaguesEvents)
    }

    initLeaguesDropdown( leagues: any, isSkipNotify: boolean = false ) {
        this.byPassValidateLeague = leagues.length === 0;
        const items = [];
        let maxLength = 0;
        leagues.forEach(function ( e, i ) {
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
            })
        });
        const _self = this;
        !isSkipNotify && _self.initLeagueIdsState(leagues);

        this.filterComponents.registerDropdownComponent({
            dropdownId: `${this.typeReport}-ddl-leagues`,
            dropdownName: "ddl-leagues",
            dropdownType: "fanexMultiSelectSearchable",
            dropdownData: items,
            dropdownConfig: {
                noneSelectedText: items.length ? "Select league" : "There is no League",
            },
            allowRerender: true,
            toggleWidth: isMobile.phone ? 243 : 193,
            minWidth: isMobile.phone ? 300 : 500,
            onClosed: function ( leagueIDs ) {
                _self.leagueIdsState.setValue(leagueIDs);
            }
        })
    }

    initLeagueIdsState( leagues ) {
        const _self = this;
        const leagueIDs = leagues.map(e => e.LeagueId).join(",");
        this.leagueIdsState.setValue(leagueIDs);
        const updateEvents = function ( leagueIDs: any ) {
            Fanex.ExtAcc.BlockUI();
            _self.getEvents(_self.sportState.getValue(), leagueIDs, _self.datesState.getValue().fromDate);
            Fanex.ExtAcc.UnBlockUI();
        };
        this.leagueIdsState.subscribe(updateEvents)
    }

    initEventsDropdown( events: any ) {
        this.byPassValidateEvent = events.length === 0;
        const items = [];
        let maxLength = 0;
        events.forEach(function ( e, i ) {
            const textValue = e.HomeName + "-vs-" + e.AwayName;
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
            })
        });

        this.filterComponents.registerDropdownComponent({
            dropdownId: `${this.typeReport}-ddl-events`,
            dropdownName: "ddl-events",
            dropdownConfig: {
                noneSelectedText: items.length ? "Select event" : "There is no event"
            },
            dropdownType: "fanexMultiSelectSearchable",
            dropdownData: items,
            toggleWidth: isMobile.phone ? 243 : 193,
            minWidth: isMobile.phone ? 300 : 500,
        })
    }

    initBetTypesDropdown( betTypes: any ) {
        const items = [];
        betTypes.forEach(function ( e, i ) {
            items.push({
                id: e.BetTypeId.toString(),
                value: e.BetTypeId.toString(),
                text: e.BetTypeName,
                checked: false,
                enable: true,
                items: []
            })
        });
        const _self = this;
        this.filterComponents.registerDropdownComponent({
            dropdownId: `${this.typeReport}-ddl-bet-types`,
            dropdownName: "ddl-bet-types",
            dropdownConfig: {
                noneSelectedText: "Select bet type"
            },
            dropdownType: "fanexMultiSelectSearchable",
            dropdownData: items,
            toggleWidth: isMobile.phone ? 243 : 193,
            minWidth: isMobile.phone ? 250 : 300
        })
    }

    initCurrencyDropDown( currencies?: any ) {
        if (!currencies) return;
        const _self = this;
        const items = [
            {
                id: "All",
                value: "All",
                text: "All",
                selected: false
            }
        ];
        currencies.forEach(function ( e, i ) {
            items.push({
                id: e.CurrencyName,
                value: e.CurrencyName,
                text: e.CurrencyName,
                selected: false
            })
        });
        this.initCurrencyState();
        this.filterComponents.registerDropdownComponent({
            dropdownId: `${this.typeReport}-ddl-currencies`,
            dropdownName: "ddl-currencies",
            dropdownType: "select2",
            dropdownData: items,
            onClosed: function ( currencyName ) {
                _self.currencyState.setValue(currencyName);
            }
        })
    }

    initCurrencyState() {
        const _self = this
        const updateBaseCurrency = function ( currencyID ) {
            Fanex.ExtAcc.BlockUI();
            let optionBaseCurrency = []
            if (currencyID === "All") {
                optionBaseCurrency = [
                    { id: "MYR", value: "MYR", text: "MYR", selected: true },
                    { id: "CNY", value: "CNY", text: "CNY", selected: false },
                    { id: "THB", value: "THB", text: "THB", selected: false },
                    { id: "IDR", value: "IDR", text: "IDR", selected: false }
                ]
            } else {
                optionBaseCurrency = [
                    { id: "Customer Currency", value: "Customer Currency", text: "Customer Currency", selected: true },
                    { id: "MYR", value: "MYR", text: "MYR", selected: false },
                    { id: "CNY", value: "CNY", text: "CNY", selected: false },
                    { id: "THB", value: "THB", text: "THB", selected: false },
                    { id: "IDR", value: "IDR", text: "IDR", selected: false },
                ]
            }
            _self.initBaseCurrencyDropDown(optionBaseCurrency);
            Fanex.ExtAcc.UnBlockUI();
        };

        this.currencyState.subscribe(updateBaseCurrency)
    }

    initBaseCurrencyDropDown( baseCurrencies: any ) {
        if (!baseCurrencies) return;
        this.filterComponents.registerDropdownComponent({
            dropdownId: `${this.typeReport}-ddl-base-currencies`,
            dropdownName: "ddl-base-currencies",
            dropdownType: "select2",
            dropdownData: baseCurrencies
        })
    }

    handleClickSubmit() {
        Fanex.ExtAcc.BlockUI();
        const isValid = this.validateParams();
        if (!isValid) {
            Fanex.ExtAcc.UnBlockUI();
            this.gridComponent.hideNoInfo(false);
        } else {
            const paramsData = this.getParams();
            paramsData.Page = 1;
            paramsData.PageSize = this.cachePageSize;
            paramsData.SortBy = "";
            paramsData.SortType = 0

            this.paramsState.setValue(paramsData);
            $.ajax({
                url: resolveClientUrl(`MatchWinLossReports/GetDetailReport`),
                type: "post",
                dataType: "json",
                cache: false,
                data: paramsData,
                success: result => this.submitSuccess(result)
            });
        }
    }

    handleClickPage( { page, pageSize }: PageInfo ) {
        Fanex.ExtAcc.BlockUI();
        const paramsData = this.paramsState.getValue();
        paramsData.Page = page;
        paramsData.PageSize = pageSize;
        $.ajax({
            url: resolveClientUrl(`MatchWinLossReports/GetDetailReport`),
            type: "post",
            dataType: "json",
            cache: false,
            data: paramsData,
            success: result => this.submitSuccess(result)
        });
    }

    handleClickSort( { sortBy, sortType }: SortInfo ) {
        Fanex.ExtAcc.BlockUI();
        const paramsData = this.paramsState.getValue();
        paramsData.SortBy = sortBy;
        paramsData.SortType = sortType;
        $.ajax({
            url: resolveClientUrl(`MatchWinLossReports/GetDetailReport`),
            type: "post",
            dataType: "json",
            cache: false,
            data: paramsData,
            success: result => this.submitSuccess(result)
        });
    }

    getParams(): ReportDetailParams {
        const params = this.filterComponents.getAllFilterValues();
        const { dropdownParams, calendarParams } = params;
        const obj: ReportDetailParams = {
            "FromDate": calendarParams.fromDate,
            "ToDate": calendarParams.toDate,
            "Sport": dropdownParams[0].currentValue,
            "Leagues": dropdownParams[1].currentValue,
            "Matches": dropdownParams[2].currentValue,
            "BetTypes": dropdownParams[3].currentValue
        }
        if (this.isMY) {
            const isCustomerCurrency = dropdownParams[5].currentValue === "Customer Currency";
            obj.Currencies = dropdownParams[4].currentValue === "All" ? "" : dropdownParams[4].currentValue;
            obj.BaseCurrency = isCustomerCurrency ? "" : dropdownParams[5].currentValue
            this.baseCurrency = isCustomerCurrency ? obj["Currencies"] : obj["BaseCurrency"];
        }
        return obj;
    }

    submitSuccess( result ) {
        if (result.errorCode) {
            Fanex.ExtAcc.UnBlockUI();
            Fanex.ExtAcc.Form.Alert("Error", result.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
            return;
        }
        try {
            if (result.Events.length || result.SystemParlayOutright.length) {
                this.gridComponent.hideNoInfo(true);
                this.renderGrid(result);
            } else {
                this.gridComponent.hideNoInfo(false);
            }
            Fanex.ExtAcc.UnBlockUI();
        } catch (e) {
            this.gridComponent.hideNoInfo(false);
            Fanex.ExtAcc.UnBlockUI();
        }
    }

    initExportExcelBtn() {
        const isValid = this.validateParams();
        if (isValid) {
            $(`#${this.gridComponentConfig.excelBtnId}`).off().click(() => {
                const url = resolveClientUrl(`MatchWinLossReports/ExportDetailReport`);
                window.top.exportExcel(url, document);
            });
        }
    }

    reportTitle() {
        const params: ReportDetailParams = this.paramsState.getValue();
        const fromDate = moment(params.FromDate, 'MM/DD/YYYY');
        const toDate = moment(params.ToDate, 'MM/DD/YYYY');
        const formattedFromDate = fromDate.format('MMM DD, YYYY');
        const formattedToDate = toDate.format('MMM DD, YYYY');

        const dateTitle = fromDate.isSame(toDate)
            ? `on ${formattedFromDate}`
            : `from ${formattedFromDate} to ${formattedToDate}`;

        let title = `Match Win Loss ${dateTitle}`;

        if (this.isMY) {
            const currency = params.Currencies;
            const baseCurrency = params.BaseCurrency;

            if (currency) {
                title += `, Currency: ${currency}`;
            }
            if (baseCurrency) {
                title += `, Base Currency: ${baseCurrency}`;
            }
        }
        $(`#${this.typeReport}-grid-title`).text(title);
    }

    renderGrid( data ) {
        try {
            const gridOptions = this.gridComponent.getGridOptions();
            this.reportTitle();

            const totalGrandTotalSystemParlayOutright = this.gridComponent.buildFooterRows({
                ...data,
                currency: this.isMY ? this.baseCurrency : ""
            })

            if (data.Events.length > 0) {
                gridOptions.bodyRows = this.gridComponent.buildBodyRows({
                    ...data,
                    currency: this.isMY ? this.baseCurrency : ""
                });
                gridOptions.footerRows = totalGrandTotalSystemParlayOutright;
            } else {
                gridOptions.bodyRows = totalGrandTotalSystemParlayOutright;
                gridOptions.footerRows = [];
            }

            gridOptions.pagerOption.pagerSelectCallBack = ( pageIndex, pageSize ) => {
                this.cachePage = pageIndex + 1;
                this.cachePageSize = pageSize;
                this.handleClickPage({
                    page: this.cachePage,
                    pageSize: this.cachePageSize,
                });
            };
            gridOptions.pagerOption.currentPage = this.cachePage - 1;
            gridOptions.pagerOption.itemsPerPage = this.cachePageSize;
            gridOptions.pagerOption.totalItem = data.TotalItems;

            gridOptions.sortOption.serverSortFunction = ( colname, currentDirection ) => {
                this.sortType = currentDirection === 'desc' ? 1 : 0;
                this.sortBy = colname;
                this.handleClickSort({
                    sortBy: this.sortBy,
                    sortType: this.sortType
                });
            }

            setPagerOptionOnMobile(gridOptions);
            this.gridComponent.setGridOptions(gridOptions)
            this.gridComponent.fanexGrid(gridOptions)
            workaroundForGridOnPhone();

        } catch (e) {
            console.log(e)
        }
    }

    validateParams() {
        const params = this.filterComponents.getAllFilterValues();
        const { dropdownParams } = params;
        const isValidLeague = dropdownParams.filter(e => e.name === "ddl-leagues")[0].currentValue.length > 0;
        if (!isValidLeague) {
            return this.byPassValidateLeague;
        }

        const isValidEvents = dropdownParams.filter(e => e.name === "ddl-events")[0].currentValue.length > 0;
        if (!isValidEvents) {
            return this.byPassValidateEvent;
        }

        const isValidBetType = dropdownParams.filter(e => e.name === "ddl-bet-types")[0].currentValue.length > 0;
        if (!isValidBetType) {
            return false;
        }
        return true;
    }


    getLeaguesAndEvents( sportID: any, fromDate: any, toDate: any, callback: any ) {
        Fanex.ExtAcc.BlockUI();
        const params = {
            sportId: sportID,
            fromDate: fromDate,
            toDate: toDate
        };
        $.ajax({
            url: resolveClientUrl(`MatchWinLossReports/GetLeaguesAndEvents`),
            type: "post",
            dataType: "json",
            cache: false,
            data: params,
            success: ( result: any ) => {
                callback(result);
            }
        });
    }

    getEvents( sportID?: any, leagueIDs?: any, fromDate?: any ) {
        const params = {
            sportId: sportID,
            leagueIds: leagueIDs,
            fromDate: fromDate
        }
        $.ajax({
            url: resolveClientUrl(`MatchWinLossReports/GetEvents`),
            type: "post",
            dataType: "json",
            cache: false,
            data: params,
            success: ( result: any ) => {
                this.initEventsDropdown(result);
            }
        });
    }
}

$(document).ready(function () {
    new MatchWinLossReportDetail().init();
});