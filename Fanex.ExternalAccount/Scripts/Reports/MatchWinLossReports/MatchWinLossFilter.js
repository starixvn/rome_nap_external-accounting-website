var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var MatchWinLossFilter = /** @class */ (function (_super) {
    __extends(MatchWinLossFilter, _super);
    function MatchWinLossFilter() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.dateFormat = "MM/DD/YYYY";
        return _this;
    }
    MatchWinLossFilter.prototype.renderCalendarPicker = function (options) {
        var _this = this;
        var dateRangePicker = $(options.selectorId);
        var today = moment(dateRangePicker.data("currentdate"));
        var todayValue = today.toDate();
        var yesterday = moment(today).subtract(1, "d");
        this.fromDate = dateRangePicker.data("fromdate");
        this.toDate = dateRangePicker.data("todate");
        dateRangePicker
            .dateRangePicker({
            format: this.dateFormat,
            separator: " - ",
            language: options.language,
            startDate: dateRangePicker.data("mindate"),
            endDate: dateRangePicker.data("maxdate"),
            showShortcuts: true,
            shortcuts: null,
            customShortcuts: this.getCalenderDefinedShortcuts(dateRangePicker),
            duration: 0,
            singleMonth: true,
            monthSelect: true,
            yearSelect: true,
            maxDays: 7
        }).bind("datepicker-change", function (_, obj) {
            _this.fromDate = moment(obj.date1).format(_this.dateFormat);
            _this.toDate = moment(obj.date2).format(_this.dateFormat);
        })
            .bind("datepicker-closed", function () { return options.onClosed(_this.getCalendarValues()); })
            .data("dateRangePicker").setDateRange(this.fromDate, this.toDate, false);
    };
    MatchWinLossFilter.prototype.renderSingleDatePicker = function (options) {
        var _this = this;
        var dateRangePicker = $(options.selectorId);
        this.fromDate = dateRangePicker.data("fromdate");
        this.toDate = dateRangePicker.data("todate");
        dateRangePicker
            .dateRangePicker({
            format: "MM/DD/YYYY",
            separator: " - ",
            language: options.language,
            startDate: dateRangePicker.data("mindate"),
            endDate: dateRangePicker.data("maxdate"),
            showShortcuts: false,
            duration: 0,
            monthSelect: true,
            yearSelect: false,
            singleDate: true,
            singleMonth: true,
            showTopbar: false
        })
            .bind("datepicker-change", function (_, obj) {
            var startDate = moment(obj.date1, "MM/dd/yyyy");
            var endDate = moment(obj.date2, "MM/dd/yyyy");
            //validate if needed
            _this.fromDate = startDate.format("MM/DD/YYYY");
            _this.toDate = startDate.format("MM/DD/YYYY");
        })
            .bind("datepicker-closed", function () { return options.onClosed(_this.getCalendarValues()); })
            .data("dateRangePicker").setDateRange(this.fromDate, this.toDate, false);
        return this.getCalendarValues();
    };
    MatchWinLossFilter.prototype.getCalenderDefinedShortcuts = function (dateRangePicker) {
        var minDate = moment(dateRangePicker.data("mindate"));
        var today = moment(dateRangePicker.data("currentdate"));
        var todayValue = today.toDate();
        var yesterday = moment(today).subtract(1, "d");
        var thisWeekStartOfDay = moment(today).startOf("isoWeek");
        return [
            {
                name: dateRangePicker.data("label-today"), dates: function () {
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
    return MatchWinLossFilter;
}(BaseReportFilters));
//# sourceMappingURL=MatchWinLossFilter.js.map