var DailyWinLossReport = /** @class */ (function () {
    function DailyWinLossReport() {
        this.typeReport = "daily-win-loss";
        this.filterComponents = new DailyWinLossFilters($);
        this.gridComponent = new DailyWinLossGrid(isMobile);
    }
    DailyWinLossReport.prototype.init = function () {
        var _this = this;
        this.initDateRangePicker();
        this.initWhiteLabelsDropdown(JSON.parse($("#inp-whitelabels").val()));
        this.initUsernameFilter();
        this.grid = $("#" + this.typeReport + "-jqgrid");
        $("#" + this.typeReport + "-submit-btn").click(function () { return _this.submit(); });
    };
    DailyWinLossReport.prototype.initDateRangePicker = function () {
        this.filterComponents.renderCalendarPicker({
            selectorId: "#" + this.typeReport + "-date-range-picker"
        });
    };
    DailyWinLossReport.prototype.initWhiteLabelsDropdown = function (whiteLabels) {
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
            dropdownOptions: mappedDataDropDown,
            minWidth: maxWhiteLabelLength > 200 ? maxWhiteLabelLength : 200
        });
    };
    DailyWinLossReport.prototype.initUsernameFilter = function () {
        var usernameInput = {
            inputName: "username",
            inputId: this.typeReport + "-username-filter",
        };
        this.filterComponents.registerInputComponent(usernameInput);
        $("#" + usernameInput.inputId).keyup(function (e) {
            $(e.target).val().length > 1 && Fanex.ExtAcc.InitSuggestion("" + usernameInput.inputId);
        });
    };
    DailyWinLossReport.prototype.submit = function () {
        var _this = this;
        Fanex.ExtAcc.BlockUI();
        if (this.unValidUserName()) {
            Fanex.ExtAcc.UnBlockUI();
            this.hideNoInfo(false);
        }
        else {
            $.ajax({
                url: resolveClientUrl("WhiteLabelReport/GetDailyWinLoss" + this.getQueryString()),
                type: "post",
                dataType: "json",
                cache: false,
                success: function (result) { return _this.submitSuccess(result); }
            });
        }
    };
    DailyWinLossReport.prototype.getQueryString = function () {
        var _a = this.filterComponents.getAllFilterValues(), inputParams = _a.inputParams, dropdownParams = _a.dropdownParams, calendarParams = _a.calendarParams;
        return [
            '?FromDate=', calendarParams.fromDate,
            '&ToDate=', calendarParams.toDate,
            '&WhiteLabels=', dropdownParams[0].currentValue,
            '&Username=', inputParams[0].currentValue,
        ].join('');
    };
    DailyWinLossReport.prototype.unValidUserName = function () {
        var inputParams = this.filterComponents.getAllFilterValues().inputParams;
        return inputParams[0].currentValue.includes("script");
    };
    DailyWinLossReport.prototype.submitSuccess = function (result) {
        if (result.errorCode) {
            Fanex.ExtAcc.UnBlockUI();
            Fanex.ExtAcc.Form.Alert("Error", result.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
            return;
        }
        if (result.Items.length) {
            this.hideNoInfo(true);
            this.renderGrid(result);
            this.registerExportExcelBtn();
        }
        else {
            this.hideNoInfo(false);
        }
        Fanex.ExtAcc.UnBlockUI();
    };
    DailyWinLossReport.prototype.registerExportExcelBtn = function () {
        var _this = this;
        $("#" + DailyWinLossGrid.ExcelBtnId).off().click(function () {
            var url = resolveClientUrl("WhiteLabelReport/ExportDailyWinLoss" + _this.getQueryString());
            window.top.exportExcel(url, document);
        });
    };
    DailyWinLossReport.prototype.hideNoInfo = function (isHide) {
        var msgNoInfo = $("#" + this.typeReport + "-msgNoinfo");
        var gridContainer = $("#" + this.typeReport + "-mainGrid");
        if (isHide) {
            msgNoInfo.hide();
            gridContainer.show();
        }
        else {
            msgNoInfo.show();
            gridContainer.hide();
        }
    };
    DailyWinLossReport.prototype.renderGrid = function (data) {
        try {
            var gridOptions = this.gridComponent.getGridOptions();
            gridOptions.titleInfo.text = this.reportTitle();
            gridOptions.bodyRows = this.gridComponent.buildBodyRows(data.Items);
            gridOptions.footerRows = this.gridComponent.buildFooterRows(data.RowTotal);
            this.gridOptions = gridOptions;
            setPagerOptionOnMobile(gridOptions);
            this.grid.fanexGrid(gridOptions);
            workaroundForGridOnPhone();
        }
        catch (e) {
            console.log(e);
        }
    };
    DailyWinLossReport.prototype.reportTitle = function () {
        var _a = this.filterComponents.getCalendarValues(), fromDate = _a.fromDate, toDate = _a.toDate;
        var formatedFromDate = moment(fromDate, 'MM/DD/YYYY').format('MMM DD, YYYY');
        var timeInfo = fromDate === toDate
            ? "on " + formatedFromDate
            : "from " + formatedFromDate + " to " + moment(toDate, 'MM/DD/YYYY').format('MMM DD, YYYY');
        return "White Label Daily Win Loss Report " + timeInfo;
    };
    return DailyWinLossReport;
}());
$(function () {
    new DailyWinLossReport().init();
});
//# sourceMappingURL=DailyWinLossReport.js.map