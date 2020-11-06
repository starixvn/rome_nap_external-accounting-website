var DateTimeControls = /** @class */ (function () {
    function DateTimeControls($) {
        this.$ = $;
    }
    DateTimeControls.prototype.Init = function () {
        var _this = this;
        this.$("#DateList").combobox({
            selected: function (event, ui) {
                var itemIndex = event.target.selectedIndex;
                _this.$("#DateList").prop("selectedIndex", itemIndex);
                var fdate = new Date(_this.$("#MaxDate").val());
                var tdate = new Date(_this.$("#MaxDate").val());
                if (itemIndex === 1) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - 1);
                    tdate = fdate;
                }
                else if (itemIndex === 2) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - _this.getDayOfYear(tdate) + 1);
                }
                else if (itemIndex === 3) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - _this.getDayOfYear(tdate) - 6);
                    tdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - _this.getDayOfYear(tdate));
                }
                else if (itemIndex === 4) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), 1);
                    tdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate());
                }
                else if (itemIndex === 5) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth() - 1, 1);
                    tdate = new Date(tdate.getFullYear(), tdate.getMonth(), 0);
                }
                else if (itemIndex === 6) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth() - 1, 1);
                }
                else if (itemIndex === 7) {
                    return;
                }
                _this.$("#dtpFromDate").val($.datepicker.formatDate("M dd, yy", fdate));
                _this.$("#dtpToDate").val($.datepicker.formatDate("M dd, yy", tdate));
            }
        });
        this.$("#dtpFromDate").datepicker({
            onClose: function () { _this.$("#userGroup input").val(_this.$("#DateList option:first").val()); },
            showOn: "both",
            buttonImageOnly: true,
            buttonImage: resolveClientUrl("Content/Images/calendar.png"),
            buttonText: "",
            beforeShow: this.SetRange,
            dateFormat: "M dd, yy"
        });
        this.$("#dtpFromDate").val(this.$.datepicker.formatDate("M dd, yy", new Date($("#MaxDate").val())));
        this.$("#dtpToDate").datepicker({
            onClose: function () { _this.$("#userGroup input").val(_this.$("#DateList option:last").val()); },
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
        this.initMonthlyCalendar();
    };
    DateTimeControls.prototype.SetRange = function () {
        return {
            minDate: new Date($("#MinDate").val()),
            maxDate: new Date($("#MaxDate").val())
        };
    };
    DateTimeControls.prototype.initMonthlyCalendar = function () {
        this.$("#monPickr1").monPickr({
            today: this.$("#MaxDate").val(),
            minDate: "2010/1/1",
            showDateRange: true,
            fromDayId: "fromDay1",
            toDayId: "toDay1"
        });
        var today = new Date(this.$("#MaxDate").val());
        this.$("#monPickr1").monPickr("setMonth", today);
        this.$("#monPickr1").monPickr("setToDay", today.getDate());
        this.$("#monPickr1").monPickr("setFromDay", 1);
    };
    DateTimeControls.prototype.getDayOfYear = function (date) {
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        var totalDays = day + (2 * month) + (3 * (month + 1) / 5) + year + (year / 4) - (year / 100) + (year / 400) + 2;
        var dayNo = totalDays % 7;
        if (dayNo === 0) {
            dayNo = 7;
        }
        return dayNo;
    };
    DateTimeControls.prototype.GetFromDateToDate = function () {
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
    };
    return DateTimeControls;
}());
//# sourceMappingURL=SuperWinLossDetailByPT.DateTimeControls.js.map