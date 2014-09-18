/*global settingContext, language*/
fanex.liveCasinoSetting.service('PtAndComSettingService', ['$http', function ($http) {
    'use strict';
    var me = this;

    me.updatePtAndComSetting = function (updatePtAndComUrl, commission, positionTaking, uplinePTOnMember, custSettingScope) {
        var ptAndComUpdateModel = {
            settingContext: settingContext,
            commission: commission,
            positionTaking: positionTaking,
            isApplyPositionTakingAllMembers: uplinePTOnMember.isApplyNewUplinePositionTaking,
            positionTakingOnMembers: uplinePTOnMember.newUplinePositionTaking 
        };

        $http({
            method: 'POST',
            url: updatePtAndComUrl,
            headers: { 'Content-Type': 'application/json' },
            data: ptAndComUpdateModel
        }).
            success(function (mainResult) {
                custSettingScope.afterServerReturnSuccess(mainResult, true);
            }).
            error(function (data, status) {
                if (status === fanex.liveCasinoSetting.UN_AUTHORIZED_CODE) {
                    fanex.liveCasinoSetting.errorResult.CommandResultsList[0].Results[0].Message = language.getString('UnauthorizeMsg');
                }
                else {
                    fanex.liveCasinoSetting.errorResult.CommandResultsList[0].Results[0].Message = language.getString('lblUpdateFailed');
                }
                custSettingScope.mainResult = fanex.liveCasinoSetting.errorResult;
            });
    };

} ]);