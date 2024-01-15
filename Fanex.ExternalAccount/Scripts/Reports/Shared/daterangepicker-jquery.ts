"use strict";

interface Window {
    bootboxAlert: any;
}

class DateRangePickerJQueryOption {
    constructor(
        public onClosed = Function(),
        public language = "default",
        public selectorId = "daterange-picker") {
        this.onClosed = onClosed;
        this.language = language;
        this.selectorId = selectorId;
    }
}

class DateRangePickerJQueryComponent {
    private historyMaxDateRange: number;
    private historyMaxMonthRange: number;
    private fromDate: string;
    private toDate: string;
    private historicPivotDateText: string;

    render(options: DateRangePickerJQueryOption) {
        const dateRangePicker = $("#" + options.selectorId);
        this.historicPivotDateText = dateRangePicker.data("historicpivotdate");
        const historicPivotDate = moment(this.historicPivotDateText, "MM/dd/yyyy");
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
                showShortcuts: true,
                shortcuts: null,
                customShortcuts: this.getPredefinedRanges(dateRangePicker),
                duration: 0,
                singleMonth: true,
                monthSelect: true,
                yearSelect: true
            }).bind("datepicker-change", (_, obj) => {
                const startDate = moment(obj.date1, "MM/dd/yyyy")
                const endDate = moment(obj.date2, "MM/dd/yyyy");
                if (endDate.isBefore(historicPivotDate)
                    && (this.historyMaxDateRange != undefined && endDate.diff(startDate, "d") > this.historyMaxDateRange
                    || this.historyMaxMonthRange != undefined && endDate.diff(moment(obj.date1, "MM/dd/yyyy").add(this.historyMaxMonthRange, "months"), "d") > 0 )) {
                    try {
                        Fanex.ExtAcc.Form.Alert('Date Range Error', dateRangePicker.data("label-maxdataretrievehistoryerror"), 400, 150, null, null, 0);
                    } catch (e) {
                        alert(dateRangePicker.data("label-maxdataretrievehistoryerror"));
                    }
                    dateRangePicker.data("dateRangePicker").setDateRange(this.fromDate, this.toDate, false);
                    return;
                }

                if (startDate.isBefore(historicPivotDate) && endDate.isSameOrAfter(historicPivotDate)
                    && (this.historyMaxDateRange != undefined && endDate.diff(startDate, "d") > this.historyMaxDateRange
                    || this.historyMaxMonthRange != undefined && endDate.diff(moment(obj.date1, "MM/dd/yyyy").add(this.historyMaxMonthRange, "months"), "d") > 0 )) {
                    try {
                        Fanex.ExtAcc.Form.Alert('Date Range Error', dateRangePicker.data("label-maxdataretrievehistoryerror"), 400, 150, null, null, 0);
                    } catch (e) {
                        alert(dateRangePicker.data("label-maxdataretrievehistoryerror"));
                    }
                    dateRangePicker.data("dateRangePicker").setDateRange(this.fromDate, this.toDate, false);
                    return;
                }

                this.fromDate = startDate.format("MM/DD/YYYY");
                this.toDate = endDate.format("MM/DD/YYYY");
            })
            .bind("datepicker-closed", () => options.onClosed(this.getDateRangeValues()))
            .data("dateRangePicker").setDateRange(this.fromDate, this.toDate, false);
    }

    private getPredefinedRanges(dateRangePicker: JQuery) {
        const minDate = moment(dateRangePicker.data("mindate"));
        const today = moment(dateRangePicker.data("currentdate"));
        const todayValue = today.toDate();
        const yesterday = moment(today).subtract(1, "d");
        const thisWeekStartOfDay = moment(today).startOf("isoWeek");
        const lastMonthStartOfDay = moment(today).subtract(1, "M").startOf("M")

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
                    const yesterdayValue = yesterday.toDate();
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
                    const lastWeekStartOfDay = moment(thisWeekStartOfDay).subtract(1, "w");
                    return [
                        lastWeekStartOfDay.diff(minDate) >= 0 ? lastWeekStartOfDay.toDate() : minDate.toDate(),
                        moment(thisWeekStartOfDay).subtract(1, "d").toDate()
                    ];
                }
            },
            {
                name: dateRangePicker.data("label-thismonth"),
                dates: function () {
                    const thisMonthStartOfDay = moment(today).startOf("M");
                    return [
                        thisMonthStartOfDay.diff(minDate) >= 0 ? thisMonthStartOfDay.toDate() : minDate.toDate(),
                        todayValue
                    ];
                }
            },
            {
                name: dateRangePicker.data("label-lastmonth"),
                dates: function () {
                    return [
                        lastMonthStartOfDay.diff(minDate) >= 0 ? lastMonthStartOfDay.toDate() : minDate.toDate(),
                        moment(lastMonthStartOfDay).endOf("M").toDate()
                    ];
                }
            },
            {
                name: dateRangePicker.data("label-sincelastmonth"),
                dates: function () {
                    return [
                        lastMonthStartOfDay.diff(minDate) >= 0 ? lastMonthStartOfDay.toDate() : minDate.toDate(),
                        todayValue
                    ];
                }
            }
        ];
    }

    getDateRangeValues() {
        return { fromDate: this.fromDate, toDate: this.toDate };
    }

    getHistoricPivotDate() {
        return this.historicPivotDateText;
    }
}
