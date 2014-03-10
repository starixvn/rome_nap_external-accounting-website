if (fanex.customerSetting) {
    var commonService = fanex.customerSetting.commonService;

    fanex.customerSetting.positionTakingService = {
        getChangedData: function (positiongTakingSections, originalPositiongTakingSections) {
            var changedData = [];
            var self = this;

            for (var i = 0; i < positiongTakingSections.length; i++) {
                if (self.isDataChanged(positiongTakingSections[i].positionTaking, originalPositiongTakingSections[i].positionTaking)) {
                    changedData.push({
                        BetTypeId: positiongTakingSections[i].positionTaking.betTypeId,
                        SportTypeId: positiongTakingSections[i].positionTaking.sportTypeId,
                        Max: positiongTakingSections[i].positionTaking.max,
                        Min: positiongTakingSections[i].positionTaking.min
                    });
                }
            }

            return changedData;
        },
        isDataChanged: function (positionTaking, originalPositionTaking) {
            return positionTaking.min != originalPositionTaking.min
            || positionTaking.max != originalPositionTaking.max;
        },
        isValidData: function () {
            return true;
        },
        editSuperActions: function (positionTaking) {
            var self = this;

            return {
                changeMax: function () {
                    positionTaking.rangeOfMin = commonService.getRangeValue(positionTaking.max, 0, 0.01, 100);
                    if (positionTaking.min > positionTaking.max) {
                        positionTaking.min = positionTaking.max;
                    }
                }
            };
        },
        editMasterActions: function (positionTaking) {
            var self = this;

            return {
                changeMax: function () {
                    positionTaking.rangeOfMin = commonService.getRangeValue(positionTaking.max, 0, 0.01, 100);
                    if (positionTaking.min > positionTaking.max) {
                        positionTaking.min = positionTaking.max;
                    }
                }
            };
        },
        editAgentActions: function (positionTaking) {
            var self = this;

            return {
                changeMax: function () {
                    positionTaking.rangeOfMin = commonService.getRangeValue(positionTaking.max, 0, 0.01, 100);
                    if (positionTaking.min > positionTaking.max) {
                        positionTaking.min = positionTaking.max;
                    }
                }
            };
        },
        setAction: function (positionTaking, formType) {
            switch (formType) {
                case 0:
                    positionTaking.actions = this.editSuperActions(positionTaking);
                    break;
                case 1:
                    positionTaking.actions = this.editMasterActions(positionTaking);
                    break;
                case 2:
                    positionTaking.actions = this.editAgentActions(positionTaking);
                    break;
                default:
                    break;
            }
        }
    };
}