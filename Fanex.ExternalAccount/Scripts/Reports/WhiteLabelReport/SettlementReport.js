var SettlementReport = /** @class */ (function () {
    function SettlementReport() {
        this.typeReport = "settlement-win-loss";
        this.gridComponentConfig = {
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
        };
        this.filterComponents = new SettlementWinLossFilters($);
        this.gridComponent = new SettlementWinLossGrid(this.gridComponentConfig);
    }
    SettlementReport.prototype.init = function () {
        var _this = this;
        $("#tabs").tabs();
        this.initDateRangePicker();
        this.initWhiteLabelsDropdown(JSON.parse($("#inp-whitelabels").val()));
        this.initUsernameFilter();
        $("#" + this.typeReport + "-submit-btn").click(function () { return _this.submit(); });
    };
    SettlementReport.prototype.initDateRangePicker = function () {
        var currentDate = moment($('#hidden-today').val(), 'MM/DD/YYYY');
        var prevSelectedMonth = currentDate.month() + "/" + currentDate.year();
        this.filterComponents.renderCalendarPicker([
            { selectorId: "#month-picker-from-date", currenSelectedMonth: prevSelectedMonth },
            { selectorId: "#month-picker-to-date", currenSelectedMonth: prevSelectedMonth }
        ]);
    };
    SettlementReport.prototype.initWhiteLabelsDropdown = function (whiteLabels) {
        var maxWhiteLabelLength = 0;
        var mappedDataDropDown = whiteLabels.map(function (e) {
            if (e.WhiteLabel.length > maxWhiteLabelLength) {
                maxWhiteLabelLength = e.WhiteLabel.length;
            }
            return {
                id: e.CustId,
                value: e.CustId,
                text: e.WhiteLabel,
                checked: true,
                enable: true,
            };
        });
        maxWhiteLabelLength *= 11;
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-white-label",
            dropdownName: "WhiteLabels",
            dropdownData: mappedDataDropDown,
            dropdownType: "fanexMultiSelect",
            minWidth: maxWhiteLabelLength > 200 ? maxWhiteLabelLength : 200
        });
    };
    SettlementReport.prototype.initUsernameFilter = function () {
        var usernameInput = {
            inputName: "username",
            inputId: this.typeReport + "-username-filter",
        };
        this.filterComponents.registerInputComponent(usernameInput);
        $("#" + usernameInput.inputId).keyup(function (e) {
            $(e.target).val().length > 1 && Fanex.ExtAcc.InitSuggestion("" + usernameInput.inputId);
        });
    };
    SettlementReport.prototype.submit = function () {
        var _this = this;
        Fanex.ExtAcc.BlockUI();
        if (this.unValidUserName()) {
            Fanex.ExtAcc.UnBlockUI();
            this.gridComponent.hideNoInfo(false);
        }
        else {
            $.ajax({
                url: resolveClientUrl("WhiteLabelReport/GetSettlement" + this.getQueryString()),
                type: "post",
                dataType: "json",
                cache: false,
                success: function (result) { return _this.submitSuccess(result); }
            });
        }
    };
    SettlementReport.prototype.getQueryString = function () {
        var _a = this.filterComponents.getAllFilterValues(), inputParams = _a.inputParams, dropdownParams = _a.dropdownParams, calendarParams = _a.calendarParams;
        return [
            '?FromDate=', calendarParams.fromDate,
            '&ToDate=', calendarParams.toDate,
            '&WhiteLabels=', dropdownParams[0].currentValue,
            '&Username=', inputParams[0].currentValue,
        ].join('');
    };
    SettlementReport.prototype.unValidUserName = function () {
        var inputParams = this.filterComponents.getAllFilterValues().inputParams;
        return inputParams[0].currentValue.includes("script");
    };
    SettlementReport.prototype.submitSuccess = function (result) {
        if (result.errorCode) {
            Fanex.ExtAcc.UnBlockUI();
            Fanex.ExtAcc.Form.Alert("Error", result.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
            return;
        }
        if (result.Items.length) {
            this.gridComponent.hideNoInfo(true);
            this.renderGrid(result);
            this.initExportExcelBtn();
        }
        else {
            this.gridComponent.hideNoInfo(false);
        }
        Fanex.ExtAcc.UnBlockUI();
    };
    SettlementReport.prototype.initExportExcelBtn = function () {
        var _this = this;
        $("#" + this.gridComponentConfig.excelBtnId).off().click(function () {
            var url = resolveClientUrl("WhiteLabelReport/ExportSettlement" + _this.getQueryString());
            window.top.exportExcel(url, document);
        });
    };
    SettlementReport.prototype.renderGrid = function (data) {
        try {
            var gridOptions = this.gridComponent.getGridOptions();
            gridOptions.titleInfo.text = this.reportTitle();
            gridOptions.bodyRows = this.gridComponent.buildBodyRows(data.Items);
            gridOptions.footerRows = this.gridComponent.buildFooterRows(data.RowTotal);
            setPagerOptionOnMobile(gridOptions);
            this.gridComponent.setGridOptions(gridOptions);
            this.gridComponent.fanexGrid(gridOptions);
            workaroundForGridOnPhone();
        }
        catch (e) {
            console.log(e);
        }
    };
    SettlementReport.prototype.reportTitle = function () {
        var dateTimeParams = this.filterComponents.getCalendarValues();
        var fromDate = dateTimeParams.fromDate, toDate = dateTimeParams.toDate;
        var fromMonth = moment(fromDate, 'MM/DD/YYYY').format('MMM, YYYY');
        var month = fromDate === toDate
            ? "on " + fromMonth
            : "from " + fromMonth + " to " + moment(toDate, 'MM/DD/YYYY').format('MMM, YYYY');
        return "White Label Settlement Report " + month;
    };
    return SettlementReport;
}());
$(function () {
    new SettlementReport().init();
});
//# sourceMappingURL=SettlementReport.js.map