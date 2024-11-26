var SingleSportSelection = /** @class */ (function () {
    function SingleSportSelection() {
        // this._self = new SingleSelectionJQueryComponent();
    }
    SingleSportSelection.prototype.init = function (selectorId, sports) {
        var items = [];
        sports.forEach(function (e, i) {
            items.push({
                id: e.SportType,
                value: e.SportType,
                text: e.SportName,
                selected: i === 0,
                enable: true,
                items: []
            });
        });
        var multipleSelection = $(selectorId);
        this.self = multipleSelection.select2({
            placeholder: "Select",
            allowClear: false,
            data: items
        });
    };
    SingleSportSelection.prototype.selectedOption = function () {
        return this.self.val();
    };
    return SingleSportSelection;
}());
//# sourceMappingURL=FilterBuilder.js.map