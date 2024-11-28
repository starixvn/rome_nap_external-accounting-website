"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.CustomDateRangePickerJQueryComponent = exports.SingleSelectionJQueryComponent = void 0;
var SingleSelectionJQueryComponent = /** @class */ (function () {
    function SingleSelectionJQueryComponent() {
    }
    SingleSelectionJQueryComponent.prototype.render = function (options) {
        var multipleSelection = $("#" + options.selectorId);
        multipleSelection.select2({
            placeholder: "Select",
            allowClear: false
        });
    };
    return SingleSelectionJQueryComponent;
}());
exports.SingleSelectionJQueryComponent = SingleSelectionJQueryComponent;
//     constructor(public selectorId = "daterange-picker", public language = "default", public onClosed = Function()) {
//         this.onClosed = onClosed;
//         this.language = language;
//         this.selectorId = selectorId;
//     }
// }
var CustomDateRangePickerJQueryComponent = /** @class */ (function () {
    function CustomDateRangePickerJQueryComponent(options) {
        this.options = options;
    }
    CustomDateRangePickerJQueryComponent.prototype.render = function () {
        var _this = this;
        var dateRangePicker = $("#" + this.options.selectorId);
        this.historicPivotDateText = dateRangePicker.data("historicpivotdate");
        var historicPivotDate = moment(this.historicPivotDateText, "MM/dd/yyyy");
        this.fromDate = dateRangePicker.data("fromdate");
        this.toDate = dateRangePicker.data("todate");
        this.historyMaxDateRange = dateRangePicker.data("historymaxdaterange");
        this.historyMaxMonthRange = dateRangePicker.data("historymaxmonthrange");
        dateRangePicker
            .dateRangePicker({
            format: "MM/DD/YYYY",
            separator: " - ",
            language: this.options.language,
            startDate: dateRangePicker.data("mindate"),
            endDate: dateRangePicker.data("maxdate"),
            showShortcuts: true,
            shortcuts: null,
            customShortcuts: this.getPredefinedRanges(dateRangePicker),
            duration: 0,
            singleMonth: true,
            monthSelect: true,
            yearSelect: true
        }).bind("datepicker-change", function (_, obj) {
            var startDate = moment(obj.date1, "MM/dd/yyyy");
            var endDate = moment(obj.date2, "MM/dd/yyyy");
            if (endDate.isBefore(historicPivotDate)
                && (_this.historyMaxDateRange != undefined && endDate.diff(startDate, "d") > _this.historyMaxDateRange
                    || _this.historyMaxMonthRange != undefined && endDate.diff(moment(obj.date1, "MM/dd/yyyy").add(_this.historyMaxMonthRange, "months"), "d") > 0)) {
                try {
                    Fanex.ExtAcc.Form.Alert('Date Range Error', dateRangePicker.data("label-maxdataretrievehistoryerror"), 400, 150, null, null, 0);
                }
                catch (e) {
                    alert(dateRangePicker.data("label-maxdataretrievehistoryerror"));
                }
                dateRangePicker.data("dateRangePicker").setDateRange(_this.fromDate, _this.toDate, false);
                return;
            }
            if (startDate.isBefore(historicPivotDate) && endDate.isSameOrAfter(historicPivotDate)
                && (_this.historyMaxDateRange != undefined && endDate.diff(startDate, "d") > _this.historyMaxDateRange
                    || _this.historyMaxMonthRange != undefined && endDate.diff(moment(obj.date1, "MM/dd/yyyy").add(_this.historyMaxMonthRange, "months"), "d") > 0)) {
                try {
                    Fanex.ExtAcc.Form.Alert('Date Range Error', dateRangePicker.data("label-maxdataretrievehistoryerror"), 400, 150, null, null, 0);
                }
                catch (e) {
                    alert(dateRangePicker.data("label-maxdataretrievehistoryerror"));
                }
                dateRangePicker.data("dateRangePicker").setDateRange(_this.fromDate, _this.toDate, false);
                return;
            }
            _this.fromDate = startDate.format("MM/DD/YYYY");
            _this.toDate = endDate.format("MM/DD/YYYY");
        })
            .bind("datepicker-closed", function () { return _this.options.onClosed(_this.getDateRangeValues()); })
            .data("dateRangePicker").setDateRange(this.fromDate, this.toDate, false);
    };
    CustomDateRangePickerJQueryComponent.prototype.getPredefinedRanges = function (dateRangePicker) {
        var minDate = moment(dateRangePicker.data("mindate"));
        var today = moment(dateRangePicker.data("currentdate"));
        var todayValue = today.toDate();
        var yesterday = moment(today).subtract(1, "d");
        var thisWeekStartOfDay = moment(today).startOf("isoWeek");
        var lastMonthStartOfDay = moment(today).subtract(1, "M").startOf("M");
        return [
            {
                name: dateRangePicker.data("label-today"),
                dates: function () {
                    return [todayValue, todayValue];
                }
            },
            {
                name: dateRangePicker.data("label-yesterday"),
                dates: function () {
                    var yesterdayValue = yesterday.toDate();
                    return [yesterdayValue, yesterdayValue];
                }
            },
            {
                name: dateRangePicker.data("label-thisweek"),
                dates: function () {
                    return [
                        thisWeekStartOfDay.diff(minDate) >= 0 ? thisWeekStartOfDay.toDate() : minDate.toDate(),
                        todayValue
                    ];
                }
            },
            {
                name: dateRangePicker.data("label-lastweek"),
                dates: function () {
                    var lastWeekStartOfDay = moment(thisWeekStartOfDay).subtract(1, "w");
                    return [
                        lastWeekStartOfDay.diff(minDate) >= 0 ? lastWeekStartOfDay.toDate() : minDate.toDate(),
                        moment(thisWeekStartOfDay).subtract(1, "d").toDate()
                    ];
                }
            }
        ];
    };
    CustomDateRangePickerJQueryComponent.prototype.getCustomRangesModeSingle = function (dateRangePicker) {
        var minDate = moment(dateRangePicker.data("mindate"));
        var today = moment(dateRangePicker.data("currentdate"));
        var todayValue = today.toDate();
        var yesterday = moment(today).subtract(1, "d");
        var thisWeekStartOfDay = moment(today).startOf("isoWeek");
        var lastMonthStartOfDay = moment(today).subtract(1, "M").startOf("M");
        return [
            {
                name: dateRangePicker.data("label-today"),
                dates: function () {
                    return [todayValue, todayValue];
                }
            },
            {
                name: dateRangePicker.data("label-yesterday"),
                dates: function () {
                    var yesterdayValue = yesterday.toDate();
                    return [yesterdayValue, yesterdayValue];
                }
            }
        ];
    };
    CustomDateRangePickerJQueryComponent.prototype.renderSingleDatePicker = function (options) {
        var dateRangePicker = $("#" + options.selectorId);
        this.historicPivotDateText = dateRangePicker.data("historicpivotdate");
        var historicPivotDate = moment(this.historicPivotDateText, "MM/dd/yyyy");
        this.fromDate = dateRangePicker.data("fromdate");
        this.toDate = dateRangePicker.data("todate");
        this.historyMaxDateRange = dateRangePicker.data("historymaxdaterange");
        this.historyMaxMonthRange = dateRangePicker.data("historymaxmonthrange");
        dateRangePicker
            .dateRangePicker({
            format: "MM/DD/YYYY",
            separator: " - ",
            language: options.language,
            startDate: dateRangePicker.data("mindate"),
            endDate: dateRangePicker.data("maxdate"),
            showShortcuts: false,
            customShortcuts: this.getCustomRangesModeSingle(dateRangePicker),
            duration: 0,
            monthSelect: true,
            yearSelect: true,
            singleDate: true,
            singleMonth: true
        });
    };
    CustomDateRangePickerJQueryComponent.prototype.getDateRangeValues = function () {
        return { fromDate: this.fromDate, toDate: this.toDate };
    };
    CustomDateRangePickerJQueryComponent.prototype.getHistoricPivotDate = function () {
        return this.historicPivotDateText;
    };
    return CustomDateRangePickerJQueryComponent;
}());
exports.CustomDateRangePickerJQueryComponent = CustomDateRangePickerJQueryComponent;
//# sourceMappingURL=FilterComponents.js.map