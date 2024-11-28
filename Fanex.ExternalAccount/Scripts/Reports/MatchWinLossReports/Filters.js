var SingleSelectionJQueryComponent = /** @class */ (function () {
    function SingleSelectionJQueryComponent() {
    }
    SingleSelectionJQueryComponent.prototype.render = function (options) {
        var _this = this;
        var multipleSelection = $(options.selectorId);
        this.self = multipleSelection.select2({
            placeholder: "Select",
            data: options.data,
        }).bind("select2:select", function () { return options.onClosed(_this.selectedOption()); });
        // .bind("change.select2", () =>{
        //     console.log("change select2", this.selectedOption())
        // });
    };
    SingleSelectionJQueryComponent.prototype.selectedOption = function () {
        return this.self.val();
    };
    return SingleSelectionJQueryComponent;
}());
var DateRangePickerWrapper = /** @class */ (function () {
    function DateRangePickerWrapper() {
    }
    DateRangePickerWrapper.prototype.render = function (options) {
        var _this = this;
        var dateRangePicker = $(options.selectorId);
        this.historicPivotDateText = dateRangePicker.data("historicpivotdate");
        this.fromDate = dateRangePicker.data("fromdate");
        this.toDate = dateRangePicker.data("todate");
        dateRangePicker
            .dateRangePicker({
            format: "MM/DD/YYYY",
            separator: " - ",
            language: options.language,
            startDate: dateRangePicker.data("mindate"),
            endDate: dateRangePicker.data("maxdate"),
            showShortcuts: true,
            shortcuts: null,
            customShortcuts: this.getPredefinedRanges(dateRangePicker),
            duration: 0,
            singleMonth: true,
            monthSelect: true,
            yearSelect: true,
            maxDays: 7
        }).bind("datepicker-change", function (_, obj) {
            var startDate = moment(obj.date1, "MM/dd/yyyy");
            var endDate = moment(obj.date2, "MM/dd/yyyy");
            // validate if needed
            _this.fromDate = startDate.format("MM/DD/YYYY");
            _this.toDate = endDate.format("MM/DD/YYYY");
        })
            .bind("datepicker-closed", function () { return options.onClosed(_this.getDateRangeValues()); })
            .data("dateRangePicker").setDateRange(this.fromDate, this.toDate, false);
        return this.getDateRangeValues();
    };
    DateRangePickerWrapper.prototype.getPredefinedRanges = function (dateRangePicker) {
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
    DateRangePickerWrapper.prototype.renderSingleDatePicker = function (options) {
        var _this = this;
        var dateRangePicker = $(options.selectorId);
        this.historicPivotDateText = dateRangePicker.data("historicpivotdate");
        var historicPivotDate = moment(this.historicPivotDateText, "MM/dd/yyyy");
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
            console.log("obj date detail", obj);
            _this.fromDate = startDate.format("MM/DD/YYYY");
            _this.toDate = startDate.format("MM/DD/YYYY");
        })
            .bind("datepicker-closed", function () { return options.onClosed(_this.getDateRangeValues()); })
            .data("dateRangePicker").setDateRange(this.fromDate, this.toDate, false);
        return this.getDateRangeValues();
    };
    DateRangePickerWrapper.prototype.getDateRangeValues = function () {
        return { fromDate: this.fromDate, toDate: this.toDate };
    };
    DateRangePickerWrapper.prototype.getHistoricPivotDate = function () {
        return this.historicPivotDateText;
    };
    return DateRangePickerWrapper;
}());
//# sourceMappingURL=Filters.js.map