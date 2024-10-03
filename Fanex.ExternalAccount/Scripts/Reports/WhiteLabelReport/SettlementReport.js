var SettlementReport = /** @class */ (function () {
    function SettlementReport() {
        this.typeReport = "settlement-win-loss";
        this.filterComponents = new SettlementWinLossFilters($);
        this.gridComponent = new SettlementWinLossGrid(isMobile);
    }
    SettlementReport.prototype.init = function () {
        var _this = this;
        $("#tabs").tabs();
        this.initDateRangePicker();
        this.initWhiteLabelsDropdown(JSON.parse($("#inp-whitelabels").val()));
        this.initUsernameFilter();
        this.grid = $("#" + this.typeReport + "-jqgrid");
        $("#" + this.typeReport + "-submit-btn").click(function () { return _this.submit(); });
        this.initSelectColumnsDropdown(this.gridComponent.getDefaultColumns(), [0, 1]);
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
            dropdownOptions: mappedDataDropDown,
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
            this.hideNoInfo(false);
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
            this.hideNoInfo(true);
            this.renderGrid(result);
            this.registerExportExcelBtn();
            this.registerCopyGridBtn();
            this._cacheItems = result.Items;
            this._cacheRowTotal = result.RowTotal;
        }
        else {
            this.hideNoInfo(false);
        }
        Fanex.ExtAcc.UnBlockUI();
    };
    SettlementReport.prototype.registerExportExcelBtn = function () {
        var _this = this;
        var excelBtn = $("#" + SettlementWinLossGrid.ExcelBtnId);
        excelBtn.off().tooltip();
        excelBtn.click(function () {
            var url = resolveClientUrl("WhiteLabelReport/ExportSettlement" + _this.getQueryString());
            window.top.exportExcel(url, document);
        });
    };
    SettlementReport.prototype.hideNoInfo = function (isHide) {
        var msgNoInfo = $("#" + this.typeReport + "-msgNoinfo");
        var gridContainer = $("#" + this.typeReport + "-mainGrid");
        var selectColumnsContainer = $("#select-columns-container");
        if (isHide) {
            msgNoInfo.hide();
            gridContainer.show();
            selectColumnsContainer.show();
        }
        else {
            msgNoInfo.show();
            gridContainer.hide();
            selectColumnsContainer.hide();
        }
    };
    SettlementReport.prototype.renderGrid = function (data) {
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
    SettlementReport.prototype.reportTitle = function () {
        var dateTimeParams = this.filterComponents.getCalendarValues();
        var fromDate = dateTimeParams.fromDate, toDate = dateTimeParams.toDate;
        var fromMonth = moment(fromDate, 'MM/DD/YYYY').format('MMM, YYYY');
        var month = fromDate === toDate
            ? "on " + fromMonth
            : "from " + fromMonth + " to " + moment(toDate, 'MM/DD/YYYY').format('MMM, YYYY');
        return "White Label Settlement Report " + month;
    };
    SettlementReport.prototype.initSelectColumnsDropdown = function (columns, ignoreColumns) {
        var _this = this;
        var options = [];
        var numberOfColumns = columns.length;
        for (var i = 0; i < numberOfColumns; i++) {
            if (ignoreColumns.includes(i))
                continue;
            options.push({
                id: i.toString(),
                value: i.toString(),
                text: columns[i].ColumnName2,
                checked: true,
                enable: true
            });
        }
        this.columnsDDL = $("#" + this.typeReport + "-ddl-columns").fanexMultiSelect({
            showFilter: true,
            minWidth: 280,
            checkAllText: "Select All",
            noneSelectedText: "None Selected",
            useCustomCheckAllText: true,
            customCheckAllText: "All",
            isCheckAll: true,
            listData: options,
            onLeave: function () { return _this.showHideColumns(); }
        });
    };
    SettlementReport.prototype.showHideColumns = function () {
        var unSelectedColumns = this.columnsDDL.fanexMultiSelect("getItems", false);
        this.grid.fanexGrid("hideColumns", this.gridOptions, unSelectedColumns.map(function (colIndex) { return Number(colIndex); }));
    };
    SettlementReport.prototype.registerCopyGridBtn = function () {
        var defaultColumns = this.gridComponent.getDefaultColumns();
        var groupHeaderColumns = this.gridComponent.getGroupHeaderColumns();
        var _self = this;
        function getHidedColumnsInGroup(groupLength, startIndex) {
            if (groupLength === 0 || startIndex >= defaultColumns.length || startIndex < 0)
                return [];
            var hideColumns = _self.columnsDDL.fanexMultiSelect("getItems", false);
            var hideColumnsName = hideColumns.map(function (colIndex) { return defaultColumns[colIndex].Name; });
            var count = 0;
            var hidedColumnsInGroup = [];
            while (count < groupLength) {
                var columnName = defaultColumns[startIndex + count].Name;
                if (hideColumnsName.includes(columnName)) {
                    hidedColumnsInGroup.push(columnName);
                }
                count++;
            }
            return hidedColumnsInGroup;
        }
        function generateTableWithMerge() {
            var hideColumns = _self.columnsDDL.fanexMultiSelect("getItems", false);
            var hideColumnsName = hideColumns.map(function (colIndex) { return defaultColumns[colIndex].Name; });
            var firstHeaderRow = ['<tr>'];
            var _loop_1 = function (i) {
                var col = defaultColumns[i];
                var groupHeader = groupHeaderColumns.find(function (e) { return e.Name === col.Name; });
                if (!groupHeader)
                    return "continue";
                if (hideColumnsName.includes(col.Name))
                    return "continue";
                var hidedColumnsInGroup = getHidedColumnsInGroup(groupHeader.MergeRowNumber, i);
                var colSpan = groupHeader.MergeColNumber ? " colspan=\"" + (groupHeader.MergeColNumber - hidedColumnsInGroup.length) + "\"" : '';
                var rowSpan = groupHeader.MergeRowNumber ? " rowspan=\"" + groupHeader.MergeRowNumber + "\"" : '';
                firstHeaderRow.push("<th" + colSpan + rowSpan + ">" + (colSpan ? groupHeader.HeaderText : col.HeaderText) + "</th>");
            };
            for (var i = 0; i < defaultColumns.length; i++) {
                _loop_1(i);
            }
            firstHeaderRow.push('</tr>');
            var secondHeaderRow = ['<tr>'];
            var _loop_2 = function (i) {
                var col = defaultColumns[i];
                if (hideColumnsName.includes(col.Name))
                    return "continue";
                var groupHeaderInfo = groupHeaderColumns.find(function (e) { return e.Name === col.Name; });
                if (groupHeaderInfo && groupHeaderInfo.MergeRowNumber)
                    return "continue";
                secondHeaderRow.push("<th>" + col.HeaderText + "</th>");
            };
            for (var i = 0; i < defaultColumns.length; i++) {
                _loop_2(i);
            }
            secondHeaderRow.push('</tr>');
            var bodyRows = [];
            var gridBodyRows = _self.gridComponent.buildBodyRows(_self._cacheItems);
            var _loop_3 = function (i) {
                var row = gridBodyRows[i];
                var tds = [];
                var cols = row.Columns.filter(function (col) { return !hideColumnsName.includes(col.Name); });
                cols.forEach(function (col) { return tds.push("<td>" + col.Value + "</td>"); });
                bodyRows.push("<tr>" + tds.join("") + "</tr>");
            };
            for (var i = 0; i < gridBodyRows.length; i++) {
                _loop_3(i);
            }
            var footerRows = [];
            var gridFooterRows = _self.gridComponent.buildFooterRows(_self._cacheRowTotal);
            var _loop_4 = function (i) {
                var row = gridFooterRows[i];
                var tds = [];
                var cols = row.Columns.filter(function (col) { return !hideColumnsName.includes(col.Name); });
                cols.forEach(function (col) {
                    var startIndex = defaultColumns.findIndex(function (e) { return e.Name === col.Name; });
                    var hidedColumnsInGroup = getHidedColumnsInGroup(col.MergeColNumber, startIndex);
                    var colSpan = col.MergeColNumber ? " colspan=\"" + (col.MergeColNumber - hidedColumnsInGroup.length) + "\"" : '';
                    tds.push("<td " + colSpan + ">" + col.Value + "</td>");
                });
                footerRows.push("<tr>" + tds.join("") + "</tr>");
            };
            for (var i = 0; i < gridFooterRows.length; i++) {
                _loop_4(i);
            }
            var theadContent = "<thead>" + firstHeaderRow.join("") + secondHeaderRow.join("") + "</thead>";
            var tbodyContent = "<tbody>" + bodyRows.join("") + "</tbody>";
            var tFooterContent = "<tfoot>" + footerRows.join("") + "</tfoot>";
            return "<table>" + theadContent + tbodyContent + tFooterContent + "</table>";
        }
        function createHiddenTextArea(text) {
            var textArea = document.createElement("textarea");
            textArea.value = text;
            textArea.style.position = "absolute";
            textArea.style.left = "-9999px";
            document.body.appendChild(textArea);
            return textArea;
        }
        var copyBtn = $("#" + SettlementWinLossGrid.CopyBtnId).tooltip({ content: "Copy" }).off("click").on("click", function () {
            var tableHtml = generateTableWithMerge();
            var copyToClipboard = function (text) {
                if (navigator.clipboard && window.isSecureContext) {
                    return navigator.clipboard.writeText(text);
                }
                else {
                    var textArea = createHiddenTextArea(text);
                    textArea.select();
                    var successful = document.execCommand('copy');
                    textArea.remove();
                    return successful ? Promise.resolve() : Promise.reject();
                }
            };
            copyToClipboard(tableHtml).then(function () {
                copyBtn.tooltip({ content: "Copied!" }).tooltip("open");
                setTimeout(function () { return copyBtn.tooltip({ content: "Copy" }).tooltip("close"); }, 2000);
            }).catch(function (err) { return console.error("Copy failed:", err); });
        });
    };
    return SettlementReport;
}());
$(function () {
    new SettlementReport().init();
});
//# sourceMappingURL=SettlementReport.js.map