var BaseReportFilters = /** @class */ (function () {
    function BaseReportFilters($) {
        this.inputIds = [];
        this.dropdowns = [];
        this.$ = $;
    }
    BaseReportFilters.prototype.registerInputComponent = function (inputInfo) {
        if (!this.inputIds.filter(function (id) { return id.inputId === inputInfo.inputId; }).length) {
            this.inputIds.push(inputInfo);
            return true;
        }
        return false;
    };
    BaseReportFilters.prototype.registerDropdownComponent = function (dropdownInfo) {
        var isExist = this.dropdowns.filter(function (id) { return id.dropdownId === dropdownInfo.dropdownId; }).length;
        switch (dropdownInfo.dropdownType) {
            case "fanexMultiSelect":
                this.renderFanexMultiSelectDDL(dropdownInfo);
                !isExist && this.dropdowns.push(dropdownInfo);
                return true;
            case "fanexMultiSelectSearchable":
                this.renderFanexMultiSelectSearchableDDL(dropdownInfo);
                !isExist && this.dropdowns.push(dropdownInfo);
                return true;
            case "combobox":
                this.renderComboboxDDL(dropdownInfo);
                !isExist && this.dropdowns.push(dropdownInfo);
                return true;
            case "select2":
                this.renderSelect2DDL(dropdownInfo);
                !isExist && this.dropdowns.push(dropdownInfo);
                return true;
            default:
                throw new Error("Unknown dropdown type");
        }
    };
    BaseReportFilters.prototype.getAllFilterValues = function () {
        var self = this;
        var inputParams = this.inputIds.map(function (elem) {
            return {
                name: elem.inputName,
                currentValue: self.$("#" + elem.inputId).val(),
            };
        });
        var getSelectedValueDropdown = function (dropdownInfo) {
            switch (dropdownInfo.dropdownType) {
                case "fanexMultiSelect":
                    return self.$("#" + dropdownInfo.dropdownId).fanexMultiSelect("getItems", true).toString();
                case "fanexMultiSelectSearchable":
                    return self.$("#" + dropdownInfo.dropdownId).fanexMultiSelectSearchable("getItems", true).toString();
                case "combobox":
                case "select2":
                    return self.$("#" + dropdownInfo.dropdownId).find(":selected").val();
                default:
                    throw new Error("Unknown dropdown type");
            }
        };
        var getSelectedTextDropdown = function (dropdownInfo) {
            switch (dropdownInfo.dropdownType) {
                case "fanexMultiSelect":
                    return self.$("#" + dropdownInfo.dropdownId).fanexMultiSelect("getSelectedTexts", true).toString();
                case "fanexMultiSelectSearchable":
                    return self.$("#" + dropdownInfo.dropdownId).fanexMultiSelectSearchable("getSelectedTexts", true).toString();
                case "combobox":
                case "select2":
                    return self.$("#" + dropdownInfo.dropdownId).find(":selected").text();
                default:
                    throw new Error("Unknown dropdown type");
            }
        };
        var dropdownParams = this.dropdowns.map(function (elem) {
            return {
                name: elem.dropdownName,
                currentValue: getSelectedValueDropdown(elem),
                currentText: getSelectedTextDropdown(elem),
            };
        });
        return {
            inputParams: inputParams,
            dropdownParams: dropdownParams,
            calendarParams: this.getCalendarValues(),
        };
    };
    BaseReportFilters.prototype.isUnValidInputs = function () {
        var inputParams = this.getAllFilterValues().inputParams;
        return inputParams.some(function (e) { return e.currentValue.includes("script"); });
    };
    BaseReportFilters.prototype.getCalendarValues = function () {
        return { fromDate: this.fromDate, toDate: this.toDate };
    };
    BaseReportFilters.prototype.renderCalendarPicker = function (options) {
        throw new Error("Method not implemented.");
    };
    BaseReportFilters.prototype.renderSingleDatePicker = function (options) {
        throw new Error("Method not implemented.");
    };
    BaseReportFilters.prototype.renderFanexMultiSelectDDL = function (options) {
        if (!options.dropdownData || !options.dropdownId)
            return;
        var dropdownConfig = options.dropdownConfig;
        var _a = dropdownConfig || { checkAllText: "Select All", noneSelectedText: "None Selected" }, checkAllText = _a.checkAllText, noneSelectedText = _a.noneSelectedText;
        return this.$("#" + options.dropdownId).fanexMultiSelect({
            showFilter: true,
            checkAllText: checkAllText,
            noneSelectedText: noneSelectedText,
            useCustomCheckAllText: false,
            customCheckAllText: "All",
            isCheckAll: true,
            listData: options.dropdownData,
            minWidth: options.minWidth,
            toggleWidth: options.toggleWidth,
            onLeave: function () {
                var leagueIDs = $("#" + options.dropdownId).fanexMultiSelect("getItems", true).toString();
                if (options.onClosed) {
                    options.onClosed(leagueIDs);
                }
            },
        });
    };
    BaseReportFilters.prototype.renderFanexMultiSelectSearchableDDL = function (options) {
        if (!options.dropdownData || !options.dropdownId)
            return;
        var dropdownConfig = options.dropdownConfig;
        var _a = dropdownConfig || { checkAllText: "Select All", noneSelectedText: "None Selected" }, checkAllText = _a.checkAllText, noneSelectedText = _a.noneSelectedText;
        return this.$("#" + options.dropdownId).fanexMultiSelectSearchable({
            showFilter: true,
            checkAllText: checkAllText,
            noneSelectedText: noneSelectedText,
            useCustomCheckAllText: false,
            customCheckAllText: "All",
            isCheckAll: true,
            listData: options.dropdownData,
            minWidth: options.minWidth,
            toggleWidth: options.toggleWidth,
            onLeave: function () {
                var leagueIDs = $("#" + options.dropdownId).fanexMultiSelectSearchable("getItems", true).toString();
                if (options.onClosed) {
                    options.onClosed(leagueIDs);
                }
            },
        });
    };
    BaseReportFilters.prototype.renderComboboxDDL = function (options) {
        var _this = this;
        if (!options.dropdownId)
            return;
        var $dropdownList = this.$("#" + options.dropdownId).combobox();
        $dropdownList.next("input.ui-autocomplete-input")
            .val($dropdownList.find("option:selected").text())
            .attr("readonly", "true")
            .bind("click", function (event) { return _this.$(event.currentTarget).autocomplete("search", ""); });
    };
    BaseReportFilters.prototype.renderSelect2DDL = function (options) {
        var _this = this;
        if (!options.dropdownId)
            return;
        this.$("#" + options.dropdownId).empty().select2({
            placeholder: "Select",
            data: options.dropdownData,
            dropdownAutoWidth: true,
            templateSelection: function (data) {
                return data.text; // Customize how selected items are shown
            },
            sorter: function (data) {
                return data; // Keep original order, no sorting
            }
        }).bind("select2:select", function () {
            if (options.onClosed) {
                options.onClosed(_this.$("#" + options.dropdownId).val());
            }
        });
    };
    return BaseReportFilters;
}());
//# sourceMappingURL=ReportFilters.js.map