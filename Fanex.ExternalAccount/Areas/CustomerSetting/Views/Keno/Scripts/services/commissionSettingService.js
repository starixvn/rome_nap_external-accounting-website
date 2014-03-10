if (fanex.customerSetting) {
    var commonService = fanex.customerSetting.commonService;

    fanex.customerSetting.commissionSettingService = {
        getChangedData: function (positiongTakingSections, originalPositiongTakingSections) {
            var changedData = [];
            var self = this;

            for (var i = 0; i < positiongTakingSections.length; i++) {
                if (self.isDataChanged(positiongTakingSections[i].commissionSetting, originalPositiongTakingSections[i].commissionSetting)) {
                    changedData.push({
                        BetTypeId: positiongTakingSections[i].commissionSetting.betTypeId,
                        SportTypeId: positiongTakingSections[i].commissionSetting.sportTypeId,
                        Value: positiongTakingSections[i].commissionSetting.value
                    });
                }
            }

            return changedData;
        },
        isDataChanged: function (commissionSetting, originalCommissionSetting) {
            return commissionSetting.value != originalCommissionSetting.value;
        },
        isValidData: function () {
            return true;
        },
        actions: function (commissionSetting) {
            var self = this;

            return {
            };
        },
        setAction: function (commissionSetting, formType) {
            commissionSetting.actions = this.actions(commissionSetting);
        }
    };
}