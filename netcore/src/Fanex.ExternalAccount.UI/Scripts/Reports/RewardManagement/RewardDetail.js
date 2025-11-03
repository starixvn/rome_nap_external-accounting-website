var RewardDetail = /** @class */ (function () {
    function RewardDetail() {
        this.typeReport = "reward-detail";
        this.cachePageSize = 10;
        this.cachePage = 1;
        this.gridComponentConfig = {
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
        };
        this.filterComponents = new RewardDetailFilter($);
        this.gridComponent = new RewardDetailGrid(this.gridComponentConfig);
    }
    RewardDetail.prototype.init = function () {
        this.initDateRangePicker();
        this.initProgramDropdown(JSON.parse($("#inp-programs").val()));
        this.initPaymentStatusDropdown();
        this.initUsernameInput();
        this.initSubmitBtn();
    };
    RewardDetail.prototype.initDateRangePicker = function () {
        this.filterComponents.renderCalendarPicker({
            selectorId: "#" + this.typeReport + "-date-range-picker"
        });
    };
    RewardDetail.prototype.initProgramDropdown = function (programs) {
        var maxLength = 0;
        var mappedDataDropDown = programs.map(function (e) {
            if (e.ProgramName.length > maxLength) {
                maxLength = e.ProgramName.length;
            }
            return {
                id: e.ProgramId,
                value: e.ProgramId,
                text: e.ProgramName,
                checked: true,
                enable: true,
            };
        });
        maxLength *= 11;
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-programs",
            dropdownName: "programs",
            dropdownData: mappedDataDropDown,
            minWidth: maxLength > 200 ? maxLength : 200,
            dropdownType: "fanexMultiSelect"
        });
    };
    RewardDetail.prototype.initPaymentStatusDropdown = function () {
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-payment-status",
            dropdownName: "payment-status",
            dropdownType: "combobox"
        });
    };
    RewardDetail.prototype.initUsernameInput = function () {
        var usernameInput = {
            inputName: "username",
            inputId: this.typeReport + "-username-filter",
        };
        this.filterComponents.registerInputComponent(usernameInput);
        $("#" + usernameInput.inputId).keyup(function (e) {
            $(e.target).val().length > 1 && Fanex.ExtAcc.InitSuggestion("" + usernameInput.inputId);
        });
    };
    RewardDetail.prototype.submitSuccess = function (result) {
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
    RewardDetail.prototype.clickSubmit = function (page, pageSize) {
        var _this = this;
        this.cachePage = page || this.cachePage;
        this.cachePageSize = pageSize || this.cachePageSize;
        Fanex.ExtAcc.BlockUI();
        var isValidProgram = this.validateProgramDDL();
        if (this.filterComponents.isUnValidInputs() || !isValidProgram) {
            Fanex.ExtAcc.UnBlockUI();
            this.gridComponent.hideNoInfo(false);
        }
        else {
            $.ajax({
                url: resolveClientUrl("RewardManagement/GetReport" + this.getQueryString(page, pageSize)),
                type: "post",
                dataType: "json",
                cache: false,
                success: function (result) { return _this.submitSuccess(result); }
            });
        }
    };
    RewardDetail.prototype.validateProgramDDL = function () {
        var params = this.filterComponents.getAllFilterValues();
        var dropdownParams = params.dropdownParams;
        return !!dropdownParams.filter(function (e) { return e.name === "programs"; })[0].currentValue.length;
    };
    RewardDetail.prototype.initSubmitBtn = function () {
        var _this = this;
        $("#" + this.typeReport + "-submit-btn").click(function () { return _this.clickSubmit(1, _this.gridComponentConfig.pageSizeOptions[0]); });
    };
    RewardDetail.prototype.initExportExcelBtn = function () {
        var _this = this;
        $("#" + this.gridComponentConfig.excelBtnId).off().click(function () {
            var url = resolveClientUrl("RewardManagement/ExportRewardDetail" + _this.getQueryString());
            window.top.exportExcel(url, document);
        });
    };
    RewardDetail.prototype.getQueryString = function (page, pageSize) {
        var params = this.filterComponents.getAllFilterValues();
        var inputParams = params.inputParams, dropdownParams = params.dropdownParams, calendarParams = params.calendarParams;
        return [
            '?FromDate=', calendarParams.fromDate,
            '&ToDate=', calendarParams.toDate,
            '&Page=', page,
            '&PageSize=', pageSize,
            '&ProgramIds=', dropdownParams[0].currentValue,
            '&MemberPaymentStatus=', dropdownParams[1].currentValue - 1,
            '&Username=', inputParams[0].currentValue,
        ].join('');
    };
    RewardDetail.prototype.renderGrid = function (data) {
        var _this = this;
        try {
            var gridOptions = this.gridComponent.getGridOptions();
            gridOptions.titleInfo.text = this.reportTitle();
            gridOptions.bodyRows = this.gridComponent.buildBodyRows(data.Items);
            gridOptions.footerRows = this.gridComponent.buildFooterRows(data.RowTotal);
            gridOptions.pagerOption.pagerSelectCallBack = function (pageIndex, pageSize) {
                _this.cachePage = pageIndex + 1;
                _this.cachePageSize = pageSize;
                _this.clickSubmit(_this.cachePage, _this.cachePageSize);
            };
            gridOptions.pagerOption.currentPage = this.cachePage - 1;
            gridOptions.pagerOption.itemsPerPage = this.cachePageSize;
            gridOptions.pagerOption.totalItem = data.TotalItems;
            setPagerOptionOnMobile(gridOptions);
            this.gridComponent.setGridOptions(gridOptions);
            this.gridComponent.fanexGrid(gridOptions);
            workaroundForGridOnPhone();
        }
        catch (e) {
            console.log(e);
        }
    };
    RewardDetail.prototype.reportTitle = function () {
        var _a = this.filterComponents.getCalendarValues(), fromDate = _a.fromDate, toDate = _a.toDate;
        var formatedFromDate = moment(fromDate, 'MM/DD/YYYY').format('MMM DD, YYYY');
        var timeInfo = fromDate === toDate
            ? "on " + formatedFromDate
            : "from " + formatedFromDate + " to " + moment(toDate, 'MM/DD/YYYY').format('MMM DD, YYYY');
        return "Reward Detail Report " + timeInfo;
    };
    return RewardDetail;
}());
$(function () {
    new RewardDetail().init();
});
//# sourceMappingURL=RewardDetail.js.map