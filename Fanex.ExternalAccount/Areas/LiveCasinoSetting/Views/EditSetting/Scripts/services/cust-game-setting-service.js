/* File Created: July 21, 2014 */
/*global custSetting, language*/
if (fanex.liveCasinoSetting) {
    fanex.liveCasinoSetting.service('CustomerGameSettingService', ['$http', function ($http) {
        'use strict';
        var $this = this;

        $this.updateGameSetting = function (gameSetting, custSettingScope) {
            var gameSettingModel = {
                settingContext: custSetting.SettingContext,
                gameSetting: gameSetting
            };            

            $http({
                method: 'POST',
                url: $('#tblGameSetting').data('update-href'),
                headers: { 'Content-Type': 'application/json' },
                data: gameSettingModel
            }).success(function (mainResult) {
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
}