fanex.goldDeluxeSetting.service('PtAndComSettingService', ['$http', function ($http) {
    'use strict';
    var me = this;

    me.updatePtAndComSetting = function (updatePtAndComUrl, commission, positionTaking, uplinePTOnMember, custSettingScope) {

        settingViewModel.Setting.PositionTaking = positionTaking;
        settingViewModel.Setting.Commission.Commission = commission;
        settingViewModel.IsUpdatePTAllMembers = uplinePTOnMember.isApplyNewUplinePositionTaking,
        settingViewModel.PTOnMember = uplinePTOnMember.newUplinePositionTaking

        $http({
            method: 'POST',
            url: updatePtAndComUrl,
            headers: { 'Content-Type': 'application/json' },
            data: settingViewModel
        }).
            success(function (mainResult) {
                custSettingScope.afterServerReturnSuccess(mainResult, true);
            }).
            error(function (data, status) {
                if (status === fanex.goldDeluxeSetting.UN_AUTHORIZED_CODE) {
                    fanex.goldDeluxeSetting.errorResult.CommandResultsList[0].Results[0].Message = language.getString('UnauthorizeMsg');
                }
                else {
                    fanex.goldDeluxeSetting.errorResult.CommandResultsList[0].Results[0].Message = language.getString('lblUpdateFailed');
                }
                custSettingScope.mainResult = fanex.goldDeluxeSetting.errorResult;
            });
    };

}]);