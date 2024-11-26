class MatchWinLossFilter extends BaseReportFilters {
    private dateFormat = "MM/DD/YYYY";

    renderCalendarPicker( options: { selectorId: string, language?: string, onClosed: Function } ) {
        const dateRangePicker = $(options.selectorId);
        const today = moment(dateRangePicker.data("currentdate"));
        const todayValue = today.toDate();
        const yesterday = moment(today).subtract(1, "d");
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
            }).bind("datepicker-change", ( _, obj ) => {
            this.fromDate = moment(obj.date1).format(this.dateFormat);
            this.toDate = moment(obj.date2).format(this.dateFormat);
        })
            .bind("datepicker-closed", () => options.onClosed(this.getCalendarValues()))
            .data("dateRangePicker").setDateRange(this.fromDate, this.toDate, false);
    }

    renderSingleDatePicker(options: { selectorId: string, language?: string, onClosed: Function } ) {
        const dateRangePicker = $(options.selectorId);
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
            .bind("datepicker-change", (_, obj) => {
                const startDate = moment(obj.date1, "MM/dd/yyyy")
                const endDate = moment(obj.date2, "MM/dd/yyyy");
                //validate if needed
                this.fromDate = startDate.format("MM/DD/YYYY");
                this.toDate = startDate.format("MM/DD/YYYY");
            })
            .bind("datepicker-closed", () => options.onClosed(this.getCalendarValues()))
            .data("dateRangePicker").setDateRange(this.fromDate, this.toDate, false);
        return this.getCalendarValues();
    }
    
    private getCalenderDefinedShortcuts( dateRangePicker: JQuery ) {
        const minDate = moment(dateRangePicker.data("mindate"));
        const today = moment(dateRangePicker.data("currentdate"));
        const todayValue = today.toDate();
        const yesterday = moment(today).subtract(1, "d");
        const thisWeekStartOfDay = moment(today).startOf("isoWeek");
        return [
            {
                name: dateRangePicker.data("label-today"), dates: function () {
                    return [ todayValue, todayValue ];
                }
            },
            {
                name: dateRangePicker.data("label-yesterday"),
                dates: function () {
                    const yesterdayValue = yesterday.toDate();
                    return [ yesterdayValue, yesterdayValue ];
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
            }
        ];
    }
}