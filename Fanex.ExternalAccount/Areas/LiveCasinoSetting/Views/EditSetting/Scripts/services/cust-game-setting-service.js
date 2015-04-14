/* File Created: July 21, 2014 */
/*global settingContext, language*/
if (fanex.liveCasinoSetting) {
    fanex.liveCasinoSetting.service('CustomerGameSettingService', ['$http', function ($http) {
        'use strict';
        var $this = this;

        $this.updateGameSetting = function (gameSetting, disableLiveCasino, settingContextScope) {
            var gameSettingModel = {
                settingContext: settingContext,
                gameSetting: gameSetting,
                disableLiveCasino: disableLiveCasino
            };

            $http({
                method: 'POST',
                url: $('#UpdateGameSettingUrl').val(),
                headers: { 'Content-Type': 'application/json' },
                data: gameSettingModel
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