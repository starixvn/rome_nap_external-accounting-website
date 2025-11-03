declare var Fanex: any;
declare var resolveClientUrl: any;
declare var isMobile: any;
declare var workaroundForGridOnPhone: any;
declare var setPagerOptionOnMobile: any;
declare var Promise: any;
declare var numeral: any;

interface Window {
    exportExcel(url: string, document: any);
}

class SettlementReport {
    private typeReport = "settlement-win-loss";

    private filterComponents: IReportFilters;
    private gridComponent: IReportGrid;
    
    private readonly gridComponentConfig: IGridComponentConfig = {
        reportId: "settlement-win-loss",
        isMobile: isMobile,
        gridId: "settlement-win-loss-jqgrid",
        gridContainerId: "settlement-win-loss-mainGrid",
        selectColumnsId: "settlement-win-loss-ddl-columns",
        selectColumnsContainerId: "settlement-win-loss-ddl-columns-container",
        msgNoInfoId: "settlement-win-loss-msgNoInfo",
        excelBtnId: "settlement-win-loss-excel",
        copyBtnId: "settlement-win-loss-copy",
        pageSizeOptions: []
    }
    
    constructor() {
        this.filterComponents = new SettlementWinLossFilters($);
        this.gridComponent = new SettlementWinLossGrid(this.gridComponentConfig);
    }

    init() {
        $("#tabs").tabs();
        this.initDateRangePicker();
        this.initWhiteLabelsDropdown(JSON.parse($("#inp-whitelabels").val()));
        this.initUsernameFilter();
        $(`#${this.typeReport}-submit-btn`).click(() => this.submit());
    }

    initDateRangePicker() {
        const currentDate = moment($('#hidden-today').val(), 'MM/DD/YYYY');
        const prevSelectedMonth = `${currentDate.month()}/${currentDate.year()}`;
        this.filterComponents.renderCalendarPicker([
            {selectorId: `#month-picker-from-date`, currenSelectedMonth: prevSelectedMonth},
            {selectorId: `#month-picker-to-date`, currenSelectedMonth: prevSelectedMonth}
        ]);
    }

    initWhiteLabelsDropdown(whiteLabels: { CustId: number; WhiteLabel: string; }[]) {
        let maxWhiteLabelLength = 0;
        const mappedDataDropDown = whiteLabels.map(function (e) {
            if (e.WhiteLabel.length > maxWhiteLabelLength) {
                maxWhiteLabelLength = e.WhiteLabel.length;
            }

            return {
                id: e.CustId,
                value: e.CustId,
                text: e.WhiteLabel,
                checked: true,
                enable: true,
            }
        });
        maxWhiteLabelLength *= 11;

        this.filterComponents.registerDropdownComponent({
            dropdownId: `${this.typeReport}-ddl-white-label`,
            dropdownName: "WhiteLabels",
            dropdownData: mappedDataDropDown,
            dropdownType: "fanexMultiSelect",
            minWidth: maxWhiteLabelLength > 200 ? maxWhiteLabelLength : 200
        });
    }

    initUsernameFilter() {
        const usernameInput = {
            inputName: "username",
            inputId: `${this.typeReport}-username-filter`,
        }
        this.filterComponents.registerInputComponent(usernameInput)
        $(`#${usernameInput.inputId}`).keyup(function (e) {
            $(e.target).val().length > 1 && Fanex.ExtAcc.InitSuggestion(`${usernameInput.inputId}`);
        });
    }

    submit() {
        Fanex.ExtAcc.BlockUI();
        if (this.unValidUserName()) {
            Fanex.ExtAcc.UnBlockUI();
            this.gridComponent.hideNoInfo(false);
        } else {
            $.ajax({
                url: resolveClientUrl(`WhiteLabelReport/GetSettlement${this.getQueryString()}`),
                type: "post",
                dataType: "json",
                cache: false,
                success: result => this.submitSuccess(result)
            });
        }
    }

    getQueryString() {
        const {inputParams, dropdownParams, calendarParams} = this.filterComponents.getAllFilterValues();
        return [
            '?FromDate=', calendarParams.fromDate,
            '&ToDate=', calendarParams.toDate,
            '&WhiteLabels=', dropdownParams[0].currentValue,
            '&Username=', inputParams[0].currentValue,
        ].join('');
    }

    unValidUserName() {
        const {inputParams} = this.filterComponents.getAllFilterValues();
        return inputParams[0].currentValue.includes("script");
    }

    submitSuccess(result) {
        if (result.errorCode) {
            Fanex.ExtAcc.UnBlockUI();
            Fanex.ExtAcc.Form.Alert("Error", result.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
            return;
        }

        if (result.Items.length) {
            this.gridComponent.hideNoInfo(true);
            this.renderGrid(result);
            this.initExportExcelBtn();
        } else {
            this.gridComponent.hideNoInfo(false);
        }

        Fanex.ExtAcc.UnBlockUI();
    }
    
    initExportExcelBtn() {
        $(`#${this.gridComponentConfig.excelBtnId}`).off().click(() => {
            const url = resolveClientUrl(`WhiteLabelReport/ExportSettlement${this.getQueryString()}`);
            window.top.exportExcel(url, document);
        });
    }

    private renderGrid(data) {
        try {
            const gridOptions = this.gridComponent.getGridOptions();
            gridOptions.titleInfo.text = this.reportTitle();
            gridOptions.bodyRows = this.gridComponent.buildBodyRows(data.Items);
            gridOptions.footerRows = this.gridComponent.buildFooterRows(data.RowTotal);
            setPagerOptionOnMobile(gridOptions);
            this.gridComponent.setGridOptions(gridOptions)
            this.gridComponent.fanexGrid(gridOptions);
            workaroundForGridOnPhone();
        } catch (e) {
            console.log(e)
        }
    }

    private reportTitle() {
        const dateTimeParams = this.filterComponents.getCalendarValues();
        const {fromDate, toDate} = dateTimeParams;
        const fromMonth = moment(fromDate, 'MM/DD/YYYY').format('MMM, YYYY');
        const month = fromDate === toDate
            ? `on ${fromMonth}`
            : `from ${fromMonth} to ${moment(toDate, 'MM/DD/YYYY').format('MMM, YYYY')}`
        return `White Label Settlement Report ${month}`;
    }
}

$(function () {
    new SettlementReport().init();
});
