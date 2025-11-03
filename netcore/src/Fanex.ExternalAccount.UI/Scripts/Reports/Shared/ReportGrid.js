var defaultOptions = {
    reportId: "",
    gridId: "",
    gridContainerId: "",
    selectColumnsId: "",
    selectColumnsContainerId: "",
    msgNoInfoId: "",
    isMobile: { phone: false },
    excelBtnId: "",
    pageSizeOptions: [10, 20, 50]
};
var BaseReportGrid = /** @class */ (function () {
    function BaseReportGrid(options) {
        var _this = this;
        this.options = defaultOptions;
        this.gridBodyRowsCache = [];
        this.gridFooterRowsCache = [];
        this.options = options;
        Fanex.ExtAcc.OnResize = function () {
            try {
                if (_this.gridOptions) {
                    $("#" + _this.options.gridId).fanexGrid("resizeGrid");
                }
            }
            catch (err) {
                console.log(err);
            }
        };
    }
    BaseReportGrid.prototype.initSelectColumnsDropdown = function (columns, ignoreColumns) {
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
        $("#" + this.options.selectColumnsId).fanexMultiSelectSearchable({
            showFilter: true,
            checkAllText: "Select All",
            noneSelectedText: "None Selected",
            useCustomCheckAllText: true,
            customCheckAllText: "All",
            isCheckAll: true,
            listData: options,
            onLeave: function () { return _this.showHideColumns(); },
            toggleWidth: this.options.isMobile.phone ? 243 : 193,
            minWidth: this.options.isMobile.phone ? 250 : 200
        });
    };
    BaseReportGrid.prototype.showHideColumns = function () {
        var unSelectedColumns = $("#" + this.options.selectColumnsId).fanexMultiSelectSearchable("getItems", false);
        $("#" + this.options.gridId).fanexGrid("hideColumns", this.gridOptions, unSelectedColumns.map(function (colIndex) { return Number(colIndex); }));
    };
    BaseReportGrid.prototype.buildBodyRows = function (reportData) {
        return undefined;
    };
    BaseReportGrid.prototype.buildFooterRows = function (reportData) {
        return undefined;
    };
    BaseReportGrid.prototype.getDefaultColumns = function () {
        return undefined;
    };
    BaseReportGrid.prototype.getGridOptions = function () {
        return this.gridOptions;
    };
    BaseReportGrid.prototype.setGridOptions = function (gridOptions) {
        this.gridOptions = gridOptions;
    };
    BaseReportGrid.prototype.getGroupHeaderColumns = function () {
        return undefined;
    };
    BaseReportGrid.prototype.hideNoInfo = function (isHide) {
        var msgNoInfo = $("#" + this.options.msgNoInfoId);
        var gridContainer = $("#" + this.options.gridContainerId);
        var selectColumnsContainer = $("#" + this.options.selectColumnsContainerId);
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
    BaseReportGrid.prototype.getDateTime = function (date) {
        if (!date)
            return "";
        return moment(date).format("MM/DD/YYYY hh:mm A");
    };
    BaseReportGrid.prototype.getDate = function (date) {
        if (!date)
            return "";
        return moment(date).format("MM/DD/YYYY");
    };
    BaseReportGrid.prototype.fanexGrid = function (gridOptions) {
        $("#" + this.options.gridId).fanexGrid(gridOptions);
        if (this.options.copyBtnId) {
            this.initCopyBtn(this.options.copyBtnId);
        }
    };
    BaseReportGrid.prototype.initCopyBtn = function (copyBtnId) {
        var defaultColumns = this.getDefaultColumns();
        var groupHeaderColumns = this.getGroupHeaderColumns();
        var _self = this;
        function getHidedColumnsInGroup(groupLength, startIndex) {
            if (groupLength === 0 || startIndex >= defaultColumns.length || startIndex < 0)
                return [];
            var hideColumns = $("#" + _self.options.selectColumnsId).fanexMultiSelect("getItems", false);
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
        function formatValue(value, options) {
            var currentValue = value;
            if (options.FormatNumber && !isNaN(currentValue) && currentValue !== '') {
                currentValue = numeral(currentValue).format(options.FormatNumber);
            }
            if (options.FormatNegative) {
                if (value < 0) {
                    currentValue = '<span style="color:red">' + options.FormatNegative.replace('$', currentValue) + '</span>';
                }
            }
            return currentValue;
        }
        function createHeader(hideColumnsName) {
            var firstHeaderRow = ['<tr>'];
            var _loop_1 = function (col) {
                if (col.MergeColNumber) {
                    var startIndex = defaultColumns.findIndex(function (e) { return e.Name === col.Name; });
                    var hidedColumnsInGroup = getHidedColumnsInGroup(col.MergeColNumber, startIndex);
                    if (col.MergeColNumber === hidedColumnsInGroup.length)
                        return "continue";
                    var colSpan = " colspan=" + (col.MergeColNumber - hidedColumnsInGroup.length);
                    firstHeaderRow.push("<th " + colSpan + ">" + col.HeaderText + "</th>");
                }
                else if (col.MergeRowNumber) {
                    if (hideColumnsName.includes(col.Name))
                        return "continue";
                    var rowSpan = " rowspan=" + col.MergeRowNumber;
                    firstHeaderRow.push("<th " + rowSpan + ">" + col.HeaderText + "</th>");
                }
            };
            for (var _i = 0, groupHeaderColumns_1 = groupHeaderColumns; _i < groupHeaderColumns_1.length; _i++) {
                var col = groupHeaderColumns_1[_i];
                _loop_1(col);
            }
            firstHeaderRow.push('</tr>');
            var secondHeaderRow = ['<tr>'];
            var filteredColumns = defaultColumns.filter(function (e) { return !hideColumnsName.includes(e.Name); });
            var _loop_2 = function (col) {
                var groupHeaderInfo = groupHeaderColumns.find(function (e) { return e.Name === col.Name; });
                if (groupHeaderInfo && groupHeaderInfo.MergeRowNumber)
                    return "continue";
                secondHeaderRow.push("<th>" + col.HeaderText + "</th>");
            };
            for (var _a = 0, filteredColumns_1 = filteredColumns; _a < filteredColumns_1.length; _a++) {
                var col = filteredColumns_1[_a];
                _loop_2(col);
            }
            secondHeaderRow.push('</tr>');
            return firstHeaderRow.join("") + secondHeaderRow.join("");
        }
        function createBody(hideColumnsName) {
            var bodyRows = [];
            var _loop_3 = function (row) {
                var tds = [];
                var cols = row.Columns.filter(function (col) { return !hideColumnsName.includes(col.Name); });
                cols.forEach(function (col) {
                    var definedCol = defaultColumns.find(function (e) { return e.Name === col.Name; });
                    tds.push("<td>" + formatValue(col.Value, definedCol) + "</td>");
                });
                bodyRows.push("<tr>" + tds.join("") + "</tr>");
            };
            for (var _i = 0, _a = _self.gridBodyRowsCache; _i < _a.length; _i++) {
                var row = _a[_i];
                _loop_3(row);
            }
            return bodyRows.join("");
        }
        function createFooter(hideColumnsName) {
            var footerRows = [];
            var _loop_4 = function (row) {
                var tds = [];
                var cols = row.Columns.filter(function (col) { return !hideColumnsName.includes(col.Name); });
                cols.forEach(function (col) {
                    var startIndex = defaultColumns.findIndex(function (e) { return e.Name === col.Name; });
                    var hidedColumnsInGroup = getHidedColumnsInGroup(col.MergeColNumber, startIndex);
                    var colSpan = col.MergeColNumber ? " colspan=\"" + (col.MergeColNumber - hidedColumnsInGroup.length) + "\"" : '';
                    tds.push("<td " + colSpan + ">" + formatValue(col.Value, defaultColumns[startIndex]) + "</td>");
                });
                footerRows.push("<tr>" + tds.join("") + "</tr>");
            };
            for (var _i = 0, _a = _self.gridFooterRowsCache; _i < _a.length; _i++) {
                var row = _a[_i];
                _loop_4(row);
            }
            return footerRows.join("");
        }
        function createTable() {
            var hideColumns = $("#" + _self.options.selectColumnsId).fanexMultiSelect("getItems", false);
            var hideColumnsName = hideColumns.map(function (colIndex) { return defaultColumns[colIndex].Name; });
            var theadContent = "<thead>" + createHeader(hideColumnsName) + "</thead>";
            var tbodyContent = _self.gridBodyRowsCache.length > 0 ? "<tbody>" + createBody(hideColumnsName) + "</tbody>" : '';
            var tFooterContent = _self.gridFooterRowsCache.length > 0 ? "<tfoot>" + createFooter(hideColumnsName) + "</tfoot>" : '';
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
        var copyBtn = $("#" + copyBtnId).tooltip({ content: "Copy" }).off("click").on("click", function () {
            var tableHtml = createTable();
            var copyToClipboard = function (text) {
                if (window.navigator["clipboard"] && window.isSecureContext) {
                    return window.navigator["clipboard"].writeText(text);
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
                copyBtn.tooltip({ content: "Copied" }).tooltip("open");
                setTimeout(function () { return copyBtn.tooltip({ content: "Copy" }).tooltip("close"); }, 2000);
            }).catch(function (err) { return console.error("Copy failed:", err); });
        });
    };
    return BaseReportGrid;
}());
//# sourceMappingURL=ReportGrid.js.map