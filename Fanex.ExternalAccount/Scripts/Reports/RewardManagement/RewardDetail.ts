declare var Fanex: any;
declare var resolveClientUrl: any;
declare var isMobile: any;
declare var workaroundForGridOnPhone: any;
declare var setPagerOptionOnMobile: any;

interface Window {
    exportExcel( url: string, document: any );
}

class RewardDetail {
    private typeReport: string = "reward-detail";
    private filterComponents: IReportFilters;
    private gridComponent: IReportGrid;
 //   private gridOptions: any;
  //  private grid: JQuery;
    private cachePageSize: number = 10;
    private cachePage: number = 1;
    private readonly gridComponentConfig: IGridComponentConfig = {
        reportId: "reward-detail",
        isMobile: isMobile,
        gridId: "reward-detail-jqgrid",
        gridContainerId: "reward-detail-mainGrid",
        selectColumnsId: "reward-detail-ddl-columns",
        selectColumnsContainerId: "reward-detail-ddl-columns-container",
        msgNoInfoId: "reward-detail-msgNoInfo",
        excelBtnId: "reward-detail-excel",
        copyBtnId: "reward-detail-copy",
        pageSizeOptions: [100, 200, 500, 1000]
    }
   
    constructor() {
        this.filterComponents = new RewardDetailFilter($);
        this.gridComponent = new RewardDetailGrid(this.gridComponentConfig);
    }

    init() {
        this.initDateRangePicker();
        this.initProgramDropdown(JSON.parse($("#inp-programs").val()));
        this.initPaymentStatusDropdown();
        this.initUsernameInput();
        this.initSubmitBtn();
        // this.initGrid();
     
    }

    initDateRangePicker() {
        this.filterComponents.renderCalendarPicker({
            selectorId: `#${this.typeReport}-date-range-picker`
        });
    }

    initProgramDropdown( programs: any ) {
        let maxLength = 0;
        const mappedDataDropDown = programs.map(function ( e ) {
            if (e.ProgramName.length > maxLength) {
                maxLength = e.ProgramName.length;
            }

            return {
                id: e.ProgramId,
                value: e.ProgramId,
                text: e.ProgramName,
                checked: true,
                enable: true,
            }
        });
        maxLength *= 11;

        this.filterComponents.registerDropdownComponent({
            dropdownId: `${this.typeReport}-ddl-programs`,
            dropdownName: "programs",
            dropdownData: mappedDataDropDown,
            minWidth: maxLength > 200 ? maxLength : 200,
            dropdownType: "fanexMultiSelect"
        });
    }

    initPaymentStatusDropdown() {
        this.filterComponents.registerDropdownComponent({
            dropdownId: `${this.typeReport}-ddl-payment-status`,
            dropdownName: "payment-status",
            dropdownType: "combobox"
        });
    }

    initUsernameInput() {
        const usernameInput = {
            inputName: "username",
            inputId: `${this.typeReport}-username-filter`,
        }
        this.filterComponents.registerInputComponent(usernameInput)
        $(`#${usernameInput.inputId}`).keyup(function (e) {
            $(e.target).val().length > 1 &&  Fanex.ExtAcc.InitSuggestion(`${usernameInput.inputId}`);
        });
    }
    
    // initGrid(){
    //     this.grid = $(`#${this.typeReport}-jqgrid`);
    // }

    private submitSuccess( result ) {
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

    private clickSubmit(page?: number, pageSize?: number) {
        this.cachePage = page || this.cachePage;
        this.cachePageSize = pageSize || this.cachePageSize;
        Fanex.ExtAcc.BlockUI();
        const isValidProgram = this.validateProgramDDL();
        if (this.filterComponents.isUnValidInputs() || !isValidProgram) {
            Fanex.ExtAcc.UnBlockUI();
            this.gridComponent.hideNoInfo(false);
        } else {
            $.ajax({
                url: resolveClientUrl(`RewardManagement/GetReport${this.getQueryString(page, pageSize)}`),
                type: "post",
                dataType: "json",
                cache: false,
                success: result => this.submitSuccess(result)
            });
        }
    }
    validateProgramDDL() {
        const params = this.filterComponents.getAllFilterValues();
        const { dropdownParams } = params;
        return !!dropdownParams.filter(e => e.name === "programs")[0].currentValue.length;
    }
    
    initSubmitBtn() {
        $(`#${this.typeReport}-submit-btn`).click(() => this.clickSubmit(1, this.gridComponentConfig.pageSizeOptions[0]));
    }

    initExportExcelBtn() {
        $(`#${this.gridComponentConfig.excelBtnId}`).off().click(() => {
            const url = resolveClientUrl(`RewardManagement/ExportRewardDetail${this.getQueryString()}`);
            window.top.exportExcel(url, document);
        });
    }

    getQueryString(page?: number, pageSize?: number) {
        const params = this.filterComponents.getAllFilterValues();
        const { inputParams, dropdownParams, calendarParams } = params;
        return [
            '?FromDate=', calendarParams.fromDate,
            '&ToDate=', calendarParams.toDate,
            '&Page=', page,
            '&PageSize=', pageSize,
            '&Programs=', dropdownParams[0].currentValue,
            '&MemberPaymentStatus=', dropdownParams[1].currentValue - 1,
            '&Username=', inputParams[0].currentValue,
        ].join('');
    }
    
    private renderGrid( data ) {
        try {
            const gridOptions = this.gridComponent.getGridOptions();
            gridOptions.titleInfo.text = this.reportTitle();
            gridOptions.bodyRows = this.gridComponent.buildBodyRows(data.Items);
            gridOptions.footerRows = this.gridComponent.buildFooterRows(data.RowTotal);
            gridOptions.pagerOption.pagerSelectCallBack = ( pageIndex, pageSize ) => {
                this.cachePage = pageIndex + 1;
                this.cachePageSize = pageSize;
                this.clickSubmit(this.cachePage, this.cachePageSize);
            };
            gridOptions.pagerOption.currentPage = this.cachePage - 1;
            gridOptions.pagerOption.itemsPerPage = this.cachePageSize;
            gridOptions.pagerOption.totalItem = data.TotalItems;
            setPagerOptionOnMobile(gridOptions);
            this.gridComponent.setGridOptions(gridOptions)
            this.gridComponent.fanexGrid(gridOptions)
            workaroundForGridOnPhone();
        } catch (e) {
            console.log(e)
        }
    }

    private reportTitle() {
        const { fromDate, toDate } = this.filterComponents.getCalendarValues();
        const formatedFromDate = moment(fromDate, 'MM/DD/YYYY').format('MMM DD, YYYY');
        const timeInfo = fromDate === toDate
            ? `on ${formatedFromDate}`
            : `from ${formatedFromDate} to ${moment(toDate, 'MM/DD/YYYY').format('MMM DD, YYYY')}`
        return `Reward Detail Report ${timeInfo}`;
    }
}

$(function () {
    new RewardDetail().init();
});