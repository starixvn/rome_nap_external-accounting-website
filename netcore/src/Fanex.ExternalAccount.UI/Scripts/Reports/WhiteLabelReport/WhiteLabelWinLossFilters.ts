class DailyWinLossFilters extends BaseReportFilters {
    private dateFormat = "MM/DD/YYYY";

    renderCalendarPicker(options: { selectorId: string, language?: string, onClosed: Function }) {
        const dateRangePicker = this.$(options.selectorId);
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
            }).bind("datepicker-change", (_, obj) => {
                this.fromDate = moment(obj.date1).format(this.dateFormat);
                this.toDate = moment(obj.date2).format(this.dateFormat);
            })
            .data("dateRangePicker").setDateRange(this.fromDate, this.toDate, false);
    }

    private getCalenderDefinedShortcuts(dateRangePicker: JQuery) {
        const today = moment(dateRangePicker.data("currentdate"), this.dateFormat);
        const todayValue = today.toDate();
        const thisWeekStartOfDay = moment(today).startOf("isoWeek");
        const lastMonthStartOfDay = moment(today).subtract(1, "M").startOf("M");

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
                    const yesterday = moment(today).subtract(1, "d");
                    const yesterdayValue = yesterday.toDate();
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
                    const lastWeekStartOfDay = moment(thisWeekStartOfDay).subtract(1, "w");
                    return [lastWeekStartOfDay.toDate(), moment(thisWeekStartOfDay).subtract(1, "d").toDate()];
                }
            },
            {
                name: dateRangePicker.data("label-thismonth"),
                dates: function () {
                    const thisMonthStartOfDay = moment(today).startOf("M");
                    return [thisMonthStartOfDay.toDate(), todayValue];
                }
            },
            {
                name: dateRangePicker.data("label-lastmonth"),
                dates: function () {
                    const lastMonthEndOfDay = moment(lastMonthStartOfDay).endOf("M").toDate();
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
    }
}

class SettlementWinLossFilters extends BaseReportFilters {
    renderCalendarPicker(options: { selectorId: string, currenSelectedMonth: string }[]): void {
        const fromMonthPicker = this.$(options[0].selectorId).MonthPicker({
            Button: '<span class="icon-calendar" />',
            SelectedMonth: options[0].currenSelectedMonth,
            MinMonth: '-12m',
            MaxMonth: '0',
            Animation: 'none',
            HideAnim: 'none',
            ShowAnim: 'none',
            OnAfterChooseMonth: (value) => this.fromDate = this.formatDate(value)
        });
        this.fromDate = this.formatDate(fromMonthPicker.MonthPicker('GetSelectedDate'))
        const toMonthPicker = this.$(options[1].selectorId).MonthPicker({
            Button: '<span class="icon-calendar" />',
            SelectedMonth: options[1].currenSelectedMonth,
            MinMonth: '-12m',
            MaxMonth: '0',
            Animation: 'none',
            HideAnim: 'none',
            ShowAnim: 'none',
            OnAfterChooseMonth: (value) => this.toDate = this.formatDate(value)
        });
        this.toDate = this.formatDate(toMonthPicker.MonthPicker('GetSelectedDate'))
    }
    
    private formatDate(date: Date) {
        return moment(date).format('MM/DD/YYYY');
    }
}
