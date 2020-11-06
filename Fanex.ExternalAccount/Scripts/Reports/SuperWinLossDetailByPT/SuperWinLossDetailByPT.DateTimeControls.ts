class DateTimeControls {
    private $: JQueryStatic;

    constructor($: JQueryStatic) {
        this.$ = $;
    }

    SetRange() {
        return {
            minDate: new Date($("#MinDate").val()),
            maxDate: new Date($("#MaxDate").val())
        };
    }

    initMonthlyCalendar() {
        this.$("#monPickr1").monPickr({
            today: this.$("#MaxDate").val(),
            minDate: "2010/1/1",
            showDateRange: true,
            fromDayId: "fromDay1",
            toDayId: "toDay1"
        });

        const today = new Date(this.$("#MaxDate").val());
        this.$("#monPickr1").monPickr("setMonth", today);
        this.$("#monPickr1").monPickr("setToDay", today.getDate());
        this.$("#monPickr1").monPickr("setFromDay", 1);
    }

    getDayOfYear(date) {
        const day = date.getDate();
        const month = date.getMonth() + 1;
        const year = date.getFullYear();
        const totalDays: number = day + (2 * month) + (3 * (month + 1) / 5) + year + (year / 4) - (year / 100) + (year / 400) + 2;
        let dayNo = totalDays % 7;
        if (dayNo === 0) {
            dayNo = 7;
        }
        return dayNo;
    }

    GetFromDateToDate() {
        if (this.$(".from-to-date").css('display') !== 'none') {
            return {
                fromdate: this.$("#dtpFromDate").val(),
                todate: this.$("#dtpToDate").val()
            };
        }
        else {
            return {
                fromdate: this.$("#monPickr1").monPickr("getFromDate", "MM/dd/yyyy"),
                todate: this.$("#monPickr1").monPickr("getToDate", "MM/dd/yyyy")
            };
        }
    }
}