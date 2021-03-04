class NexDatePickerHelper {
    private GetShortDateString(date): string {
        // month is 0 based (0 - 11)
        let strDate = this.GetDateStr(date.getMonth() + 1) + "/";
        strDate += this.GetDateStr(date.getDate()) + "/";
        strDate += this.GetDateStr(date.getFullYear());

        return strDate;
    }

    private GetDateStr(date): string {
        if (!date) {
            return "";
        }

        if (date < 10) {
            return "0" + date.toString();
        }

        return date.toString();
    }

    private GetBaseDateOfWeek(date): number {
        // Base date from Monday
        var day = date.getDay();            // Sun:0 -> Sat:6

        return day === 0                    // Sun:0: default of system
            ? date.getDate() - 6            // Sunday<-Mon: 6
            : date.getDate() - day + 1;     // other days<-Mon: date.getDay() - 1
    }

    public GetDateRangeOfCurrentWeek(today) {
        let date = new Date(today);
        const lastDate = new Date(today);
        const baseDate = this.GetBaseDateOfWeek(date);
        const startDate = new Date(date.setDate(baseDate));

        return [startDate, lastDate];
    }

    public GetDateRangeOfLastWeek(today) {
        const baseDateOfCurrentWeek = this.GetBaseDateOfWeek(new Date(today)); // Monday
        const baseDateOfLastWeek = baseDateOfCurrentWeek - 7; // Seek 7 days

        let lastWeek = new Date(today);
        lastWeek.setDate(baseDateOfLastWeek);
        const startDate = this.GetShortDateString(lastWeek);

        lastWeek.setDate(lastWeek.getDate() + 6); // Seek 6 days: Monday->Sunday
        const endDate = this.GetShortDateString(lastWeek);

        return [new Date(startDate), new Date(endDate)];
    }
}

class DateTimeControls {
    private $: JQueryStatic;

    constructor($: JQueryStatic) {
        this.$ = $;
    }

    public Init() {
        this.$("#DateList").combobox({
            selected: event => this.DateRangeSelected(event.target.selectedIndex)
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

        const optionSelected = this.$("#DateList option:selected");
        const optionSelectedIndex = optionSelected.index();
        this.$("#userGroup input").val(optionSelected.val());
        optionSelectedIndex > 0 && this.DateRangeSelected(optionSelectedIndex);

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

    private DateRangeSelected(itemIndex) {
        let dateRange;
    	const nexDatePickerHelper = new NexDatePickerHelper();
        this.$("#DateList").prop("selectedIndex", itemIndex);

        var fdate = new Date(this.$("#MaxDate").val());
        var tdate = new Date(this.$("#MaxDate").val());
        if (itemIndex === 1) {
            fdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - 1);
            tdate = fdate;
        } else if (itemIndex === 2) {
            dateRange = nexDatePickerHelper.GetDateRangeOfCurrentWeek(tdate);
            fdate = dateRange[0];
        } else if (itemIndex === 3) {
            dateRange = nexDatePickerHelper.GetDateRangeOfLastWeek(tdate);
            fdate = dateRange[0];
            tdate = dateRange[1];
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
}