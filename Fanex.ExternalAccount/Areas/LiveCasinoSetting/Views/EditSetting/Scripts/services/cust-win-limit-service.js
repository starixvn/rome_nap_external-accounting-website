/* File Created: July 21, 2014 */
/*global settingContext, language*/
if (fanex.liveCasinoSetting) {
    fanex.liveCasinoSetting.service('CustomerWinLimitService', ['$http', function ($http) {
        'use strict';
        var $this = this;

        $this.getCurrentMemberWinLimit = function (custId) {
            return $http({
                method: 'GET',
                url: $('#memberWinLimitLink').val() + '?custId=' + custId,
                headers: { 'Content-Type': 'application/json' }
            });
        };

        $this.updateWinLimitSetting = function (winLimitModel, settingContextScope) {
            
            var winLimitUpdate = {
                settingContext: settingContext,
                model: winLimitModel
            };

            $http({
                method: 'POST',
                url: $('#UpdateWinLimitUrl').val(),
                headers: { 'Content-Type': 'application/json' },
                data: winLimitUpdate
            }).success(function (mainResult) {
                settingContextScope.afterServerReturnSuccess(mainResult, true);
            }).
            error(function (data, status) {
                if (status === fanex.liveCasinoSetting.UN_AUTHORIZED_CODE) {
                    fanex.liveCasinoSetting.errorResult.CommandResultsList[0].Results[0].Message = language.getString('UnauthorizeMsg');
                }
                else {
                    fanex.liveCasinoSetting.errorResult.CommandResultsList[0].Results[0].Message = language.getString('lblUpdateFailed');
                }
                settingContextScope.mainResult = fanex.liveCasinoSetting.errorResult;
            });
        };
    } ]);
}