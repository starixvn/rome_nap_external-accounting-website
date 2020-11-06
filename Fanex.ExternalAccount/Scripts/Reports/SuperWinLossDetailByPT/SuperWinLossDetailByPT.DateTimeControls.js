class DateTimeControls {
    constructor() {
    }
    SetRange() {
        return {
            minDate: new Date($("#MinDate").val()),
            maxDate: new Date($("#MaxDate").val())
        };
    }
    SwitchCalendarType(showType) {
        if (showType === 1) {
            this.ProductsList(false);
            $('.from-to-date').css('display', 'none');
            $('.group-month-day').css('display', 'inline-block');
        }
        else {
            this.ProductsList(true);
            $('.from-to-date').css('display', 'inline-block');
            $('.group-month-day').css('display', 'none');
        }
    }
    initMonthlyCalendar() {
        $("#monPickr1").monPickr({
            today: $("#MaxDate").val(),
            minDate: "2010/1/1",
            showDateRange: true,
            fromDayId: "fromDay1",
            toDayId: "toDay1",
            log: function (msg) {
                if (typeof console !== "undefined")
                    console.warn(msg);
            }
        });

        var today = new Date($("#MaxDate").val());
        $("#monPickr1").monPickr("setMonth", today);
        $("#monPickr1").monPickr("setToDay", today.getDate());
        $("#monPickr1").monPickr("setFromDay", 1);
    }
    getDayOfYear(date) {
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        var totalDays = day + (2 * month) + parseInt(3 * (month + 1) / 5) + year + parseInt(year / 4) - parseInt(year / 100) + parseInt(year / 400) + 2;
        var dayNo = totalDays % 7;
        if (dayNo === 0) {
            dayNo = 7;
        }
        return dayNo;
    }
    GetFromDateToDate() {
        if ($(".from-to-date").css('display') !== 'none') {
            return {
                fromdate: $("#dtpFromDate").val(),
                todate: $("#dtpToDate").val()
            };
        }
        else {
            return {
                fromdate: $("#monPickr1").monPickr("getFromDate", "MM/dd/yyyy"),
                todate: $("#monPickr1").monPickr("getToDate", "MM/dd/yyyy")
            };
        }
    }
}