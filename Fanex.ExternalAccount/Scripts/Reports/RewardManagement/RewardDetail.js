var RewardDetail = /** @class */ (function () {
    function RewardDetail() {
        this.typeReport = "reward-detail";
        this.filterComponents = new RewardDetailFilter($);
        this.gridComponent = new RewardDetailGrid(isMobile);
    }
    RewardDetail.prototype.init = function () {
        this.initDateRangePicker();
        this.initProgramDropdown(JSON.parse($("#inp-programs").val()));
        this.initPaymentStatusDropdown([{ Id: 1, Name: "Paid" }, { Id: 2, Name: "Unpaid" }]);
    };
    RewardDetail.prototype.initDateRangePicker = function () {
        this.filterComponents.renderCalendarPicker({
            selectorId: "#" + this.typeReport + "-date-range-picker"
        });
    };
    RewardDetail.prototype.initProgramDropdown = function (programs) {
        console.log("programs", programs);
        var maxLength = 0;
        var mappedDataDropDown = programs.map(function (e) {
            if (e.Name.length > maxLength) {
                maxLength = e.Name.length;
            }
            return {
                id: e.Id,
                value: e.Id,
                text: e.Name,
                checked: true,
                enable: true,
            };
        });
        maxLength *= 11;
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-programs",
            dropdownName: "programs",
            dropdownOptions: mappedDataDropDown,
            minWidth: maxLength > 200 ? maxLength : 200
        });
    };
    RewardDetail.prototype.initPaymentStatusDropdown = function (paymentStatus) {
        var maxLength = 0;
        var mappedDataDropDown = paymentStatus.map(function (e) {
            if (e.Name.length > maxLength) {
                maxLength = e.Name.length;
            }
            return {
                id: e.Id,
                value: e.Id,
                text: e.Name,
                checked: true,
                enable: true,
            };
        });
        maxLength *= 11;
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-payment-status",
            dropdownName: "payment-status",
            dropdownOptions: mappedDataDropDown,
            minWidth: maxLength > 200 ? maxLength : 200
        });
    };
    return RewardDetail;
}());
$(function () {
    new RewardDetail().init();
});
//# sourceMappingURL=RewardDetail.js.map