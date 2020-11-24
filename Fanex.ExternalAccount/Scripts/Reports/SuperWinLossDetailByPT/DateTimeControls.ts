class DateTimeControls {
    private $: JQueryStatic;

    constructor($: JQueryStatic) {
        this.$ = $;
    }

    public Init() {
        this.$("#DateList").combobox({
            selected: event => {
                var itemIndex = event.target.selectedIndex;
                this.$("#DateList").prop("selectedIndex", itemIndex);

                var fdate = new Date(this.$("#MaxDate").val());
                var tdate = new Date(this.$("#MaxDate").val());
                if (itemIndex === 1) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - 1);
                    tdate = fdate;
                } else if (itemIndex === 2) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - this.GetDayOfYear(tdate) + 1);
                } else if (itemIndex === 3) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - this.GetDayOfYear(tdate) - 6);
                    tdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - this.GetDayOfYear(tdate));
                } else if (itemIndex === 4) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), 1);
                    tdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate());
                } else if (itemIndex === 5) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth() - 1, 1);
                    tdate = new Date(tdate.getFullYear(), tdate.getMonth(), 0);
                } else if (itemIndex === 6) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth() - 1, 1);
                } else if (itemIndex === 7) {
                    return;
                }

                this.$("#dtpFromDate").val($.datepicker.formatDate("M dd, yy", fdate));
                this.$("#dtpToDate").val($.datepicker.formatDate("M dd, yy", tdate));
            }
        });

        this.$("#dtpFromDate").datepicker({
            onClose: () => { this.$("#userGroup input").val(this.$("#DateList option:last").val()); },
            showOn: "both",
            buttonImageOnly: true,
            buttonImage: resolveClientUrl("Content/Images/calendar.png"),
            buttonText: "",
            beforeShow: this.SetRange,
            dateFormat: "M dd, yy"
        });
        this.$("#dtpFromDate").val(this.$.datepicker.formatDate("M dd, yy", new Date($("#MaxDate").val())));

        this.$("#dtpToDate").datepicker({
            onClose: () => { this.$("#userGroup input").val(this.$("#DateList option:last").val()); },
            showOn: "both",
            buttonImageOnly: true,
            buttonImage: resolveClientUrl("Content/Images/calendar.png"),
            buttonText: "",
            beforeShow: this.SetRange,
            dateFormat: "M dd, yy"
        });
        this.$("#dtpToDate").val(this.$.datepicker.formatDate("M dd, yy", new Date(this.$("#MaxDate").val())));

        this.$(".ui-datepicker").draggable();

        this.$("#userGroup input").val(this.$("#DateList option:first").val());

        this.InitMonthlyCalendar();
    }

    public GetFromDateToDate() {
        return this.$(".from-to-date").css('display') !== 'none' ?
            {
                fromdate: this.$("#dtpFromDate").val(),
                todate: this.$("#dtpToDate").val()
            } :
            {
                fromdate: this.$("#monPickr1").monPickr("getFromDate", "MM/dd/yyyy"),
                todate: this.$("#monPickr1").monPickr("getToDate", "MM/dd/yyyy")
            };
    }

    private InitMonthlyCalendar() {
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

    private GetDayOfYear(date) {
        const day = date.getDate();
        const month = date.getMonth() + 1;
        const year = date.getFullYear();
        const totalDays = day + (2 * month) + Math.round(3 * (month + 1) / 5) + year + Math.round(year / 4) - (year / 100) + Math.round(year / 400) + 2;
        let dayNo = (totalDays % 7);

        if (dayNo === 0) {
            dayNo = 7;
        }

        return dayNo;
    }

    private SetRange() {
        return {
            minDate: new Date($("#MinDate").val()),
            maxDate: new Date($("#MaxDate").val())
        };
    }
}