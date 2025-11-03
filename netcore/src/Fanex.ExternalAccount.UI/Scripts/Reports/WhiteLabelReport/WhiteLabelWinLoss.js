var WhiteLabelWinLoss = /** @class */ (function () {
    function WhiteLabelWinLoss($, topWindow) {
        this.typeReport = "white-label-win-loss";
        this.$ = $;
        this.topWindow = topWindow;
        this.filterComponents = new WhiteLabelWinLossFilters();
        this.gridComponent = new WhiteLabelWinLossGrid(isMobile);
    }
    WhiteLabelWinLoss.prototype.init = function () {
        var _this = this;
        this.gridContainer = this.$("#" + this.typeReport + "-mainGrid");
        this.grid = this.$("#" + this.typeReport + "-jqgrid");
        this.msgNoInfo = this.$("#" + this.typeReport + "-msgNoinfo");
        this.initDateRangePicker();
        this.initSuperFilter();
        this.initMasterFilter();
        this.initAgentFilter();
        this.$("#" + this.typeReport + "-submit-btn").click(function () { return _this.submit(); });
    };
    WhiteLabelWinLoss.prototype.initDateRangePicker = function () {
        var _self = this;
        this.filterComponents.renderCalendar({
            selectorId: "#" + this.typeReport + "-date-range-picker",
            onClosed: function (value) {
                console.log("on change picker", value);
                //_self.datesState.setValue(value);
            }
        });
    };
    WhiteLabelWinLoss.prototype.initSuperFilter = function () {
        this.filterComponents.registerInputComponent({
            inputName: "Super",
            inputId: this.typeReport + "-super-filter",
        });
    };
    WhiteLabelWinLoss.prototype.initMasterFilter = function () {
        this.filterComponents.registerInputComponent({
            inputName: "Master",
            inputId: this.typeReport + "-master-filter",
        });
    };
    WhiteLabelWinLoss.prototype.initAgentFilter = function () {
        this.filterComponents.registerInputComponent({
            inputName: "Agent",
            inputId: this.typeReport + "-agent-filter",
        });
    };
    WhiteLabelWinLoss.prototype.submit = function () {
        var _this = this;
        var parameters = this.getParameters();
        if (parameters) {
            Fanex.ExtAcc.BlockUI();
            this.$.ajax({
                url: resolveClientUrl("WhiteLabelWinLossReports/GetReport?" + parameters),
                type: "post",
                dataType: "json",
                cache: false,
                success: function (result) { return _this.submitSuccess(result); }
            });
            Fanex.ExtAcc.UnBlockUI();
        }
    };
    WhiteLabelWinLoss.prototype.submitSuccess = function (result) {
        console.log("result", result);
        if (result.errorCode) {
            Fanex.ExtAcc.Form.Alert("Error", result.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
            return;
        }
        try {
            if (result.data.Items.length > 0) {
                this.renderGrid(result);
            }
            else {
                this.renderNoData();
            }
        }
        catch (e) {
            console.log(e);
        }
        workaroundForGridOnPhone();
    };
    WhiteLabelWinLoss.prototype.getParameters = function () {
        var inputValues = this.filterComponents.getAllInputValues();
        console.log("inputValues", inputValues);
        var _a = this.filterComponents.getCalendarValues(), fromDate = _a.fromDate, toDate = _a.toDate;
        var _b = this.filterComponents.getPageValues(), pageSize = _b.pageSize, page = _b.page;
        var timeFilter = "&FromDate=" + fromDate + "&ToDate=" + toDate;
        var superFilter = "&Super=" + inputValues[0].currentValue;
        var masterFilter = "&Master=" + inputValues[1].currentValue;
        var agentFilter = "&Agent=" + inputValues[2].currentValue;
        var pageFilter = "&PageSize=" + pageSize + "&Page=" + page;
        var parameterUrl = "" + timeFilter + superFilter + masterFilter + agentFilter + pageFilter;
        return parameterUrl;
    };
    WhiteLabelWinLoss.prototype.renderGrid = function (data) {
        var _this = this;
        var gridOptions = this.gridComponent.getGridOptions();
        setPagerOptionOnMobile(this.gridOptions);
        gridOptions.titleInfo.text = "White Label Win/Loss Report";
        gridOptions.columns = this.gridComponent.getDefaultColumns();
        gridOptions.bodyRows = this.gridComponent.buildBodyRows(data.data.Items);
        gridOptions.footerRows = this.gridComponent.buildFooterRows(data.data);
        gridOptions.pagerOption.pagerSelectCallBack = function (pageIndex, pageSize) {
            _this.filterComponents.setPageValues({ pageSize: pageSize, page: pageIndex + 1 });
            _this.submit();
        };
        var _a = this.filterComponents.getPageValues(), pageSize = _a.pageSize, page = _a.page;
        gridOptions.pagerOption.currentPage = page - 1;
        gridOptions.pagerOption.itemsPerPage = pageSize;
        gridOptions.pagerOption.totalItem = data.data.Items.length;
        // gridOptions.sortOption.serverSortFunction = (colname, currentDirection) => {
        //     this.cacheFormData.sortType = currentDirection === 'desc' ? 1 : 0;
        //     this.cacheFormData.sortBy = colname;
        //     this.submit();
        // }
        this.gridContainer.removeClass("hide");
        // this.btnExportExcel.removeClass("hide");
        // this.msgNoInfo.removeAttr("style");
        this.gridOptions = gridOptions;
        setPagerOptionOnMobile(gridOptions);
        this.grid.fanexGrid(gridOptions);
        //this.$(`#${SummaryGridBuilder.ExcelBtnId}`).click(() => this.exportExcel());
        workaroundForGridOnPhone();
    };
    WhiteLabelWinLoss.prototype.renderNoData = function () {
        this.gridContainer.removeAttr("style").addClass("hide");
        this.grid.removeAttr("style").html("");
        //this.btnExportExcel.addClass("hide");
        this.msgNoInfo.css("display", "block");
    };
    return WhiteLabelWinLoss;
}());
$(document).ready(function () {
    var detail = new WhiteLabelWinLoss($, window.top);
    detail.init();
});
//# sourceMappingURL=WhiteLabelWinLoss.js.map