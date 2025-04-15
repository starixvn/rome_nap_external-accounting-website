var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var DailyWinLossFilters = /** @class */ (function (_super) {
    __extends(DailyWinLossFilters, _super);
    function DailyWinLossFilters() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.dateFormat = "MM/DD/YYYY";
        return _this;
    }
    DailyWinLossFilters.prototype.renderCalendarPicker = function (options) {
        var _this = this;
        var dateRangePicker = this.$(options.selectorId);
        this.fromDate = dateRangePicker.data("fromdate");
        this.toDate = dateRangePicker.data("todate");
        dateRangePicker
            .dateRangePicker({
            format: this.dateFormat,
            separator: " - ",
            startDate: dateRangePicker.data("mindate"),
            endDate: dateRangePicker.data("maxdate"),
            showShortcuts: true,
            shortcuts: null,
            customShortcuts: this.getCalenderDefinedShortcuts(dateRangePicker),
            duration: 0,
            singleMonth: true,
            monthSelect: true,
            yearSelect: true
        }).bind("datepicker-change", function (_, obj) {
            _this.fromDate = moment(obj.date1).format(_this.dateFormat);
            _this.toDate = moment(obj.date2).format(_this.dateFormat);
        })
            .data("dateRangePicker").setDateRange(this.fromDate, this.toDate, false);
    };
    DailyWinLossFilters.prototype.getCalenderDefinedShortcuts = function (dateRangePicker) {
        var today = moment(dateRangePicker.data("currentdate"), this.dateFormat);
        var todayValue = today.toDate();
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
                    var yesterday = moment(today).subtract(1, "d");
                    var yesterdayValue = yesterday.toDate();
                    return [yesterdayValue, yesterdayValue];
                }
            },
            {
                name: dateRangePicker.data("label-thisweek"),
                dates: function () {
                    return [thisWeekStartOfDay.toDate(), todayValue];
                }
            },
            {
                name: dateRangePicker.data("label-lastweek"),
                dates: function () {
                    var lastWeekStartOfDay = moment(thisWeekStartOfDay).subtract(1, "w");
                    return [lastWeekStartOfDay.toDate(), moment(thisWeekStartOfDay).subtract(1, "d").toDate()];
                }
            },
            {
                name: dateRangePicker.data("label-thismonth"),
                dates: function () {
                    var thisMonthStartOfDay = moment(today).startOf("M");
                    return [thisMonthStartOfDay.toDate(), todayValue];
                }
            },
            {
                name: dateRangePicker.data("label-lastmonth"),
                dates: function () {
                    var lastMonthEndOfDay = moment(lastMonthStartOfDay).endOf("M").toDate();
                    return [lastMonthStartOfDay.toDate(), lastMonthEndOfDay];
                }
            },
            {
                name: dateRangePicker.data("label-sincelastmonth"),
                dates: function () {
                    return [lastMonthStartOfDay.toDate(), todayValue];
                }
            }
        ];
    };
    return DailyWinLossFilters;
}(BaseReportFilters));
var SettlementWinLossFilters = /** @class */ (function (_super) {
    __extends(SettlementWinLossFilters, _super);
    function SettlementWinLossFilters() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    SettlementWinLossFilters.prototype.renderCalendarPicker = function (options) {
        var _this = this;
        var fromMonthPicker = this.$(options[0].selectorId).MonthPicker({
            Button: '<span class="icon-calendar" />',
            SelectedMonth: options[0].currenSelectedMonth,
            MinMonth: '-12m',
            MaxMonth: '0',
            Animation: 'none',
            HideAnim: 'none',
            ShowAnim: 'none',
            OnAfterChooseMonth: function (value) { return _this.fromDate = _this.formatDate(value); }
        });
        this.fromDate = this.formatDate(fromMonthPicker.MonthPicker('GetSelectedDate'));
        var toMonthPicker = this.$(options[1].selectorId).MonthPicker({
            Button: '<span class="icon-calendar" />',
            SelectedMonth: options[1].currenSelectedMonth,
            MinMonth: '-12m',
            MaxMonth: '0',
            Animation: 'none',
            HideAnim: 'none',
            ShowAnim: 'none',
            OnAfterChooseMonth: function (value) { return _this.toDate = _this.formatDate(value); }
        });
        this.toDate = this.formatDate(toMonthPicker.MonthPicker('GetSelectedDate'));
    };
    SettlementWinLossFilters.prototype.formatDate = function (date) {
        return moment(date).format('MM/DD/YYYY');
    };
    return SettlementWinLossFilters;
}(BaseReportFilters));
//# sourceMappingURL=WhiteLabelWinLossFilters.js.map