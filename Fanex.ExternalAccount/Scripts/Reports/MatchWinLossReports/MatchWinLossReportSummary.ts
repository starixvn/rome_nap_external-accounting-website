declare var Fanex: any;
declare var resolveClientUrl: any;
declare var workaroundForGridOnPhone: any;
declare var isMobile: any;
declare var setPagerOptionOnMobile: any;

interface Window {
    exportExcel( url: string, document: any );
}

class MatchWinLossReportSummary {
    private readonly datesState: StateManager<{ fromDate: any, toDate: any }>;
    private readonly sportState: StateManager<string>;
    private readonly currencyState: StateManager<string>;
    private readonly paramsState: StateManager<ReportSummaryParams>;

    private readonly filterComponents: IReportFilters;
    private readonly gridComponent: IReportGrid;
    private readonly isMY: boolean;
    private readonly typeReport: string = "summary";
    private readonly gridComponentConfig: IGridComponentConfig = {
        reportId: "summary",
        isMobile: isMobile,
        gridId: "summary-jqgrid",
        gridContainerId: "summary-mainGrid",
        selectColumnsId: "summary-ddl-columns",
        selectColumnsContainerId: "summary-ddl-columns-container",
        msgNoInfoId: "summary-msgNoInfo",
        excelBtnId: "summary-excel",
        pageSizeOptions: [ 500, 1000, 1500 ],
    }

    private cachePageSize: number = this.gridComponentConfig.pageSizeOptions[0];
    private cachePage: number = 1;
    private sortBy: string = "";
    private sortType: number = 0;

    private baseCurrency: string = "";

    private byPassLeagueValidate: boolean = false;

    constructor() {
        this.paramsState = new StateManager({
            "FromDate": null,
            "ToDate": null,
            "Sport": null,
            "Leagues": null
        });
        this.datesState = new StateManager({ fromDate: null, toDate: null });
        this.sportState = new StateManager<string>("");
        this.currencyState = new StateManager<string>("All");

        this.filterComponents = new MatchWinLossFilter($);
        this.gridComponent = new SummaryGrid(this.gridComponentConfig);
        this.isMY = $("#inp-isMY").val().toLowerCase() === "true"
    }

    init() {
        Fanex.ExtAcc.BlockUI();
        this.initDateRangePicker();
        this.initSportsDropdown(JSON.parse($("#inp-sports").val()));
        this.initLeaguesDropdown(JSON.parse($("#inp-leagues").val()))

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
        this.gridComponent.initSelectColumnsDropdown(SummaryGrid.DefaultColumns, [ 0, 1 ]);
        this.initExportExcelBtn();
        Fanex.ExtAcc.UnBlockUI();
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
                url: resolveClientUrl(`MatchWinLossReports/GetSummaryReport`),
                type: "post",
                dataType: "json",
                cache: false,
                data: paramsData,
                success: result => this.submitSuccess(result),
            });
        }
    }

    handleClickSort( { sortBy, sortType }: SortInfo ) {
        Fanex.ExtAcc.BlockUI();
        const params: ReportSummaryParams = this.paramsState.getValue()
        params.SortBy = sortBy;
        params.SortType = sortType;
        this.paramsState.setValue(params);

        $.ajax({
            url: resolveClientUrl(`MatchWinLossReports/GetSummaryReport`),
            type: "post",
            dataType: "json",
            cache: false,
            data: params,
            success: result => this.submitSuccess(result),
        });
    }

    handleClickPage( { page, pageSize }: PageInfo ) {
        Fanex.ExtAcc.BlockUI();
        const params: ReportSummaryParams = this.paramsState.getValue()
        params.Page = page;
        params.PageSize = pageSize;
        this.paramsState.setValue(params);

        $.ajax({
            url: resolveClientUrl(`MatchWinLossReports/GetSummaryReport`),
            type: "post",
            dataType: "json",
            cache: false,
            data: params,
            success: result => this.submitSuccess(result),
        });
    }

    getLeagues( sportID, fromDate, toDate, callback ) {
        const params = {
            sportId: sportID,
            fromDate: fromDate,
            toDate: toDate
        }
        $.ajax({
            url: resolveClientUrl(`MatchWinLossReports/GetLeagues`),
            type: "post",
            dataType: "json",
            cache: false,
            data: params,
            success: ( leagues: any ) => {
                callback(leagues);
            }
        });
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
        const _self = this;
        if (isValid) {
            $(`#${this.gridComponentConfig.excelBtnId}`).off().click(() => {
                const url = resolveClientUrl(`MatchWinLossReports/ExportSummaryReport`);
                window.top.exportExcel(url, document);
            });
        }
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
                gridOptions.bodyRows = this.gridComponent.buildBodyRows(data);
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

    reportTitle() {
        const params: ReportSummaryParams = this.paramsState.getValue();
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

    initDateRangePicker() {
        const _self = this;
        this.filterComponents.renderCalendarPicker(
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
        const updateLeagues = function ( dateRange: { fromDate: any, toDate: any } ) {
            Fanex.ExtAcc.BlockUI();
            const { fromDate, toDate } = dateRange;
            _self.getLeagues(_self.sportState.getValue(), fromDate, toDate, ( leagues ) => {
                _self.initLeaguesDropdown(leagues);
            })
        }
        _self.datesState.subscribe(updateLeagues)
    }

    validateParams() {
        const params = this.filterComponents.getAllFilterValues();
        const { dropdownParams } = params;
        const isValidLeague = dropdownParams[1].currentValue.length > 0;
        if (!isValidLeague) {
            return this.byPassLeagueValidate;
        }
        return true;
    }

    getParams(): ReportSummaryParams {
        const params = this.filterComponents.getAllFilterValues();
        const { dropdownParams, calendarParams } = params;
        const obj: ReportSummaryParams = {
            "FromDate": calendarParams.fromDate,
            "ToDate": calendarParams.toDate,
            "Sport": dropdownParams[0].currentValue,
            "Leagues": dropdownParams[1].currentValue,
        }
        if (this.isMY) {
            const isCustomerCurrency = dropdownParams[3].currentValue === "Customer Currency";
            obj.Currencies = dropdownParams[2].currentValue === "All" ? "" : dropdownParams[2].currentValue;
            obj.BaseCurrency = isCustomerCurrency ? "" : dropdownParams[3].currentValue
            this.baseCurrency = isCustomerCurrency ? obj["Currencies"] : obj["BaseCurrency"];
        }
        return obj;
    }

    initSportsDropdown( sports ) {
        if (!sports) return;
        const _self = this;
        const items = [];
        sports.forEach(function ( e, i ) {
            items.push({
                id: e.SportType,
                text: e.SportName,
                selected: i === 0,
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
        })
    }

    initSportState( sportType ) {
        const _self = this;
        _self.sportState.setValue(sportType);
        const updateLeagues = function ( sportID ) {
            Fanex.ExtAcc.BlockUI();
            const { fromDate, toDate } = _self.datesState.getValue();
            _self.getLeagues(sportID, fromDate, toDate, ( leagues ) => {
                _self.initLeaguesDropdown(leagues);
            })
        }
        _self.sportState.subscribe(updateLeagues)
    }

    initLeaguesDropdown( leagues?: any ) {
        this.byPassLeagueValidate = leagues.length === 0;
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
        this.filterComponents.registerDropdownComponent({
            dropdownId: `${this.typeReport}-ddl-leagues`,
            dropdownName: "ddl-leagues",
            dropdownType: "fanexMultiSelectSearchable",
            dropdownData: items,
            dropdownConfig: {
                noneSelectedText: items.length ? "Select league" : "There is no League",
            },
            toggleWidth: isMobile.phone ? 243 : 193,
            minWidth: isMobile.phone ? 300 : 500,
        })
        Fanex.ExtAcc.UnBlockUI();
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
        const updateBaseCurrency = function ( currencyName ) {
            Fanex.ExtAcc.BlockUI();
            let optionBaseCurrency = []
            if (currencyName === "All") {
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
            _self.initBaseCurrencyDropDown(optionBaseCurrency)
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
}

$(document).ready(function () {
    new MatchWinLossReportSummary().init();
});