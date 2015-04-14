/* File Created: July 21, 2014 */
/*global language*/
if (fanex.liveCasinoSetting) {
    fanex.liveCasinoSetting.service('CustomerSettingService', ['$http', function ($http) {
        'use strict';
        var me = this;

        me.updateAllSetting = function (updateAllSettingUrl, settingContextForUpdate, scope) {
            $http({
                method: 'POST',
                url: updateAllSettingUrl,
                headers: { 'Content-Type': 'application/json' },
                data: settingContextForUpdate
            }).
            success(function (mainResult) {
                scope.afterServerReturnSuccess(mainResult, false);
            }).
            error(function (data, status) {
                if (status === fanex.liveCasinoSetting.UN_AUTHORIZED_CODE) {
                    fanex.liveCasinoSetting.errorResult.CommandResultsList[0].Results[0].Message = language.getString('UnauthorizeMsg');
                }
                else {
                    fanex.liveCasinoSetting.errorResult.CommandResultsList[0].Results[0].Message = language.getString('lblUpdateFailed');
                }
                scope.mainResult = fanex.liveCasinoSetting.errorResult;
            });
        };
    } ]);
}