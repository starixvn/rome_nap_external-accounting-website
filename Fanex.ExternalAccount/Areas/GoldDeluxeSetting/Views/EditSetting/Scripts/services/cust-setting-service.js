/* File Created: July 21, 2014 */
/*global language*/
if (fanex.goldDeluxeSetting) {
    fanex.goldDeluxeSetting.service('CustomerSettingService', ['$http', function ($http) {
        'use strict';
        var me = this;

        me.updateAllSetting = function (updateAllSettingUrl, settingViewModelForUpdate, scope) {
            $http({
                method: 'POST',
                url: updateAllSettingUrl,
                headers: { 'Content-Type': 'application/json' },
                data: settingViewModelForUpdate
            }).
            success(function (mainResult) {
                scope.afterServerReturnSuccess(mainResult, false);
            }).
            error(function (data, status) {
                if (status === fanex.goldDeluxeSetting.UN_AUTHORIZED_CODE) {
                    fanex.goldDeluxeSetting.errorResult.CommandResultsList[0].Results[0].Message = language.getString('UnauthorizeMsg');
                }
                else {
                    fanex.goldDeluxeSetting.errorResult.CommandResultsList[0].Results[0].Message = language.getString('lblUpdateFailed');
                }
                scope.mainResult = fanex.goldDeluxeSetting.errorResult;
            });
        };
    }]);
}